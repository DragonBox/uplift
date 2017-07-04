using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
// Note - this is singleton
namespace Uplift
{
    public class UpfileHandler
    {
        private const bool debugMode = true;
        public const string upfilePath = "Upfile.xml";
        private Upfile Upfile;

        private static UpfileHandler instance;

        private UpfileHandler()
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

        public bool CheckForUpfile()
        {
            return File.Exists(upfilePath);
        }

        private void InternalLoadFile()
        {
            Upfile = LoadFile();
        }

        public Upfile LoadFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Upfile));

            using (FileStream fs = new FileStream(upfilePath, FileMode.Open))
            {
                return serializer.Deserialize(fs) as Upfile;
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
        public void CheckUnityVersion()
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

        public PathConfiguration GetDestinationFor(InstallSpecType specType)
        {

            PathConfiguration PH;

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
                    PH = Upfile.Configuration.PluginPath;
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