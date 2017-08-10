using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using Uplift.Extensions;

// Note - this is singleton
namespace Uplift
{
    public class UpfileHandler
    {
        private const bool debugMode = true;
        public const string upfilePath = "Upfile.xml";
        public const string globalOverridePath = ".Upfile.xml";
        protected Upfile Upfile;

        protected static UpfileHandler instance;

        protected UpfileHandler()
        {
        }

        public void Initialize()
        {
            if (CheckForUpfile())
            {
                InternalLoadFile();
                CheckUnityVersion();
                LoadPackageList();
            }
            else
            {
                Debug.Log("Upfile doesn't exist. Generate new one from menu if needed.");
            }
        }

        public static UpfileHandler Instance()
        {
            if (instance != null) return instance;

            UpfileHandler uph = new UpfileHandler();
            instance = uph;
            uph.Initialize();
            return uph;
        }

        public virtual bool CheckForUpfile()
        {
            return File.Exists(upfilePath);
        }

        internal void InternalLoadFile()
        {
            Upfile = LoadFile();
        }

        public virtual void LoadOverrides(ref Upfile upfile) {
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                               Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            string overrideFilePath = Path.Combine(homePath, globalOverridePath);

            // If we don't have override file, ignore
            if (!File.Exists(overrideFilePath)) {
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(UpfileOverride));

            using(FileStream fs = new FileStream(overrideFilePath, FileMode.Open)) {

                try {
                    UpfileOverride upOverride = serializer.Deserialize(fs) as UpfileOverride;

                    if(upfile.Repositories == null) {
                        upfile.Repositories = upOverride.Repositories;
                    } else {
                        int repositoriesSize = upfile.Repositories.Length + upOverride.Repositories.Length;

                        Repository[] newRepositoryArray = new Repository[repositoriesSize];
                        Array.Copy(upfile.Repositories, newRepositoryArray, upfile.Repositories.Length);
                        Array.Copy(upOverride.Repositories, 0, newRepositoryArray, upOverride.Repositories.Length, upfile.Repositories.Length);

                        upfile.Repositories = newRepositoryArray;
                    }
                } catch (InvalidOperationException) {
                    Debug.Log("Upfile.xml Global Override is not well formed");
                }
            }




        }

        public virtual Upfile LoadFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Upfile));

            using (FileStream fs = new FileStream(upfilePath, FileMode.Open))
            {
                Upfile raw =  serializer.Deserialize(fs) as Upfile;
                if(raw.Configuration != null)
                {
                    if (raw.Configuration.BaseInstallPath != null) { raw.Configuration.BaseInstallPath.Location = raw.Configuration.BaseInstallPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.DocsPath != null) { raw.Configuration.DocsPath.Location = raw.Configuration.DocsPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.ExamplesPath != null) { raw.Configuration.ExamplesPath.Location = raw.Configuration.ExamplesPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.MediaPath != null) { raw.Configuration.MediaPath.Location = raw.Configuration.MediaPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.PluginPath != null) { raw.Configuration.PluginPath.Location = raw.Configuration.PluginPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.RepositoryPath != null) { raw.Configuration.RepositoryPath.Location = raw.Configuration.RepositoryPath.Location.MakePathOSFriendly(); }
                }

                LoadOverrides(ref raw);

                if(raw.Repositories != null) {
                    foreach(Repository repo in raw.Repositories)
                    {
                        if(repo is FileRepository)
                        {
                            (repo as FileRepository).Path = (repo as FileRepository).Path.MakePathOSFriendly();
                        }
                    }
                }

                return raw;
            }
        }

        public string GetPackagesRootPath()
        {
            return Upfile.Configuration.RepositoryPath.Location;
        }

        public void LoadPackageList()
        {
            PackageList pList = PackageList.Instance();
            pList.LoadPackages(Upfile.Repositories, true);
        }

        public void InstallDependencies()
        {
            //FIXME: We should check for all repositories, not the first one
            //FileRepository rt = (FileRepository) Upfile.Repositories[0];

            PackageHandler pHandler = new PackageHandler();

            foreach (DependencyDefinition packageDefinition in Upfile.Dependencies)
            {
                PackageRepo result = pHandler.FindPackageAndRepository(packageDefinition);
                if (result.Repository != null)
                {
                    using (TemporaryDirectory td = result.Repository.DownloadPackage(result.Package))
                    {
                        LocalHandler.InstallPackage(result.Package, td);
                    }
                }
            }
        }

        internal void ListPackages()
        {
            foreach (Repository repository in Upfile.Repositories)
            {
                foreach (Upset package in repository.ListPackages())
                {
                    Debug.Log("Package: " + package.PackageName + " Version: " + package.PackageVersion);
                }
            }
        }

        internal void NukePackages()
        {
            Debug.LogWarning("Nuking all packages!");
            LocalHandler.NukeAllPackages();
        }



        //FIXME: Prepare proper version checker
        public virtual void CheckUnityVersion()
        {
            string upfileVersion = Upfile.UnityVersion;
            string unityVersion = Application.unityVersion;
            if (unityVersion != upfileVersion)
            {
                Debug.LogError(string.Format("Uplift: Upfile.xml Unity Version ({0}) doesn't match Unity's one  ({1}).",
                    upfileVersion, unityVersion));
            }
            else
            {
                Debug.Log("Upfile: Version check successful");
            }
        }

        public PathConfiguration GetDestinationFor(InstallSpec spec)
        {

            PathConfiguration PH;

            var specType = spec.Type;

            switch (specType)
            {
                case(InstallSpecType.Base):
                    PH = Upfile.Configuration.BaseInstallPath;
                    break;

                case(InstallSpecType.Docs):
                    PH = Upfile.Configuration.DocsPath;
                    break;

                case(InstallSpecType.Examples):
                    PH = Upfile.Configuration.ExamplesPath;
                    break;

                case(InstallSpecType.Plugin):
                    // TODO: Make additional check for platform
                    PH = new PathConfiguration()
                    {
                        Location = Upfile.Configuration.PluginPath.Location,
                        SkipPackageStructure = true // Plugins always skip package structure.

                    };

                    // Platform as string
                    string platformAsString;

                    switch (spec.Platform)
                    {
                        case(PlatformType.All): // It means, that we just need to point to "Plugins" folder.
                            platformAsString = "";
                            break;
                        case(PlatformType.iOS):
                            platformAsString = "ios";
                            break;
                        default:
                            platformAsString = "UNKNOWN";
                            break;
                    }
                    PH.Location = Path.Combine(PH.Location, platformAsString);
                    break;

                case(InstallSpecType.Media):
                    PH = Upfile.Configuration.MediaPath;
                    break;

                default:
                    PH = Upfile.Configuration.BaseInstallPath;
                    break;
            }

            return PH;


        }
    }
}
