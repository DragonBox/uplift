using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;
using Uplift.Packages;

namespace Uplift.Schemas
{
    public partial class Upfile
    {
        // --- SINGLETON DECLARATION ---
        protected static Upfile instance;

        internal Upfile() { }

        public static Upfile Instance()
        {
            if (instance == null)
            {
                InitializeInstance();
            }

            return instance;
        }

        public static void ResetInstance()
        {
            instance = null;
            InitializeInstance();
        }

        internal static void InitializeInstance()
        {
            instance = null;
            if (!CheckForUpfile())
            {
                Debug.Log("No Upfile in this project");
                return;
            }

            instance = LoadXml();
            instance.CheckUnityVersion();
            instance.LoadPackageList();
        }

        // --- CLASS DECLARATION ---
        public static readonly string upfilePath = "Upfile.xml";
        public static readonly string globalOverridePath = ".Upfile.xml";

        public static bool CheckForUpfile()
        {
            return File.Exists(upfilePath);
        }

        internal static Upfile LoadXml()
        {
            return LoadXml(upfilePath);
        }

        internal static Upfile LoadXml(string path)
        {
            try
            {
                StrictXmlDeserializer<Upfile> deserializer = new StrictXmlDeserializer<Upfile>();

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    Upfile upfile = deserializer.Deserialize(fs);

                    upfile.MakePathConfigurationsOSFriendly();
                    upfile.LoadOverrides();

                    if (upfile.Repositories != null)
                    {
                        foreach (Repository repo in upfile.Repositories)
                        {
                            if (repo is FileRepository)
                                (repo as FileRepository).Path = Uplift.Common.FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
                        }
                    }

                    Debug.Log("Upfile was successfully loaded");
                    return upfile;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Uplift: Could not load Upfile", e);
            }
        }

        public virtual void LoadOverrides()
        {
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                               Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            string overrideFilePath = Path.Combine(homePath, globalOverridePath);

            try
            {
                LoadOverrides(overrideFilePath);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Uplift: Could not load Upfile overrides from "+overrideFilePath, e);
            }
        }

        internal virtual void LoadOverrides(string path)
        {
            // If we don't have override file, ignore
            if (!File.Exists(path)) return;

            StrictXmlDeserializer<UpfileOverride> deserializer = new StrictXmlDeserializer<UpfileOverride>();

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {

                try
                {
                    UpfileOverride upOverride = deserializer.Deserialize(fs);

                    foreach (Repository repo in upOverride.Repositories)
                    {
                        if (repo is FileRepository)
                            (repo as FileRepository).Path = Uplift.Common.FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
                    }

                    if (Repositories == null)
                    {
                        Repositories = upOverride.Repositories;
                    }
                    else {
                        int repositoriesSize = Repositories.Length + upOverride.Repositories.Length;

                        Repository[] newRepositoryArray = new Repository[repositoriesSize];
                        Array.Copy(Repositories, newRepositoryArray, Repositories.Length);
                        Array.Copy(upOverride.Repositories, 0, newRepositoryArray, Repositories.Length, upOverride.Repositories.Length);

                        Repositories = newRepositoryArray;
                    }
                }
                catch (InvalidOperationException)
                {
                    Debug.LogError(string.Format("Global Override file at {0} is not well formed", path));
                }
            }
        }


        public string GetPackagesRootPath()
        {
            return Configuration.RepositoryPath.Location;
        }

        public void LoadPackageList()
        {
            PackageList pList = PackageList.Instance();
            pList.LoadPackages(Repositories, true);
        }

        //FIXME: Prepare proper version checker
        public virtual void CheckUnityVersion()
        {
            string environmentVersion = Application.unityVersion;
            if(VersionParser.ParseUnityVersion(environmentVersion) < VersionParser.ParseUnityVersion(UnityVersion))
            {
                Debug.LogError(string.Format("Upfile.xml Unity Version ({0}) targets a higher version of Unity than you are currently using ({1})",
                    UnityVersion, environmentVersion));
            }
        }

        public void MakePathConfigurationsOSFriendly()
        {
            foreach(PathConfiguration path in PathConfigurations())
            {
                if (!(path == null))
                    path.Location = Uplift.Common.FileSystemUtil.MakePathOSFriendly(path.Location);
            }
        }

        public IEnumerable<PathConfiguration> PathConfigurations()
        {
            if (Configuration == null) yield break;
            yield return Configuration.BaseInstallPath;
            yield return Configuration.DocsPath;
            yield return Configuration.EditorPluginPath;
            yield return Configuration.ExamplesPath;
            yield return Configuration.GizmoPath;
            yield return Configuration.MediaPath;
            yield return Configuration.PluginPath;
            yield return Configuration.RepositoryPath;
        }

        public PathConfiguration GetDestinationFor(InstallSpec spec)
        {
            PathConfiguration PH;

            var specType = spec.Type;

            switch (specType)
            {
                case (InstallSpecType.Base):
                    PH = Configuration.BaseInstallPath;
                    break;

                case (InstallSpecType.Docs):
                    PH = Configuration.DocsPath;
                    break;

                case (InstallSpecType.EditorPlugin):
                    PH = new PathConfiguration()
                    {
                        Location = Configuration.EditorPluginPath.Location,
                        SkipPackageStructure = true // Plugins always skip package structure.

                    };
                    break;

                case (InstallSpecType.Examples):
                    PH = Configuration.ExamplesPath;
                    break;

                case (InstallSpecType.Gizmo):
                    PH = new PathConfiguration()
                    {
                        Location = Configuration.GizmoPath.Location,
                        SkipPackageStructure = true // Gizmo always skip package structure.
                    };
                    break;

                case (InstallSpecType.Media):
                    PH = Configuration.MediaPath;
                    break;

                case (InstallSpecType.Plugin):
                    PH = new PathConfiguration()
                    {
                        Location = Configuration.PluginPath.Location,
                        SkipPackageStructure = true // Plugins always skip package structure.
                    };

                    // Platform as string
                    string platformAsString;

                    switch (spec.Platform)
                    {
                        case (PlatformType.All): // It means, that we just need to point to "Plugins" folder.
                            platformAsString = "";
                            break;
                        case (PlatformType.iOS):
                            platformAsString = "ios";
                            break;
                        default:
                            platformAsString = "UNKNOWN";
                            break;
                    }
                    PH.Location = Path.Combine(PH.Location, platformAsString);
                    break;

                default:
                    PH = Configuration.BaseInstallPath;
                    break;
            }

            return PH;
        }

        public IEnumerable<Upset> ListPackages()
        {
            if (Repositories == null) yield break;
            foreach(Repository repository in Repositories)
            {
                foreach(Upset package in repository.ListPackages())
                {
                    yield return package;
                }
            }
        }
    }
}