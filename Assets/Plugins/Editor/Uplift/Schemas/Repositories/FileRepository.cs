using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;

namespace Uplift.Schemas {
    
    public partial class FileRepository {
        
        private const string formatPattern = "{0}{1}{2}";

        public override TemporaryDirectory DownloadPackage(Upset package) {
            string sourcePath = string.Format(formatPattern, Path, System.IO.Path.DirectorySeparatorChar, package.MetaInformation.dirName);
            TemporaryDirectory td = new TemporaryDirectory();

            //string destination = LocalHandler.GetLocalDirectory(package.PackageName, package.PackageVersion);
                
            try {
                FileSystemUtil.CopyDirectory(sourcePath, td.Path);
            } catch (DirectoryNotFoundException) {
                Debug.LogError(string.Format("Package {0} not found in specified version {1}", package.PackageName, package.PackageVersion));
                td.Dispose();
            }

            return td;
        }

        public override Upset[] ListPackages() {
            List<Upset> upsetList = new List<Upset>();

            AddExplodedDirectories(upsetList);
            AddUnityPackages(upsetList);

            return upsetList.ToArray();
        }

        private void AddExplodedDirectories(List<Upset> upsetList) {
            string[] directories =  Directory.GetDirectories(Path);
            foreach(string directoryName in directories) {
                string upsetPath = directoryName + System.IO.Path.DirectorySeparatorChar + UpsetFile;
                
                if (!File.Exists(upsetPath)) continue;
                
                XmlSerializer serializer = new XmlSerializer(typeof(Upset));

                using (FileStream file = new FileStream(upsetPath, FileMode.Open)) {
                    Upset upset = serializer.Deserialize(file) as Upset;
                    upset.MetaInformation.dirName = directoryName;
                    upsetList.Add(upset);
                }
            }
        }

        private void AddUnityPackages(List<Upset> upsetList) {
            string[] files =  Directory.GetFiles(Path, "*.unityPackage");

            foreach(string FileName in files)
            {
                // assume unityPackage doesn't contain an upset file for now. In the future we can support it
                Upset upset = InferUpsetFromUnityPackage(FileName);
                if (upset == null) continue;
                upsetList.Add(upset);
            }
        }

        private static Upset InferUpsetFromUnityPackage(string FileName)
        {
            string ShortFileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
            string[] split = ShortFileName.Split('-');
            if (split.Length != 2)
            {
                Debug.LogWarning("Skipping file " + FileName + " as it doesn't follow the pattern 'PackagName-PackageVersion.unityPackage'");
                return null;
            }
            string PackageName = split[0];
            string PackageVersion = split[1];
            string PackageLicense = "Unknown";
            string MinUnityVersion = "0.0.0";

            Upset upset = new Upset();
            upset.PackageLicense = PackageLicense;
            upset.PackageName = PackageName;
            upset.PackageVersion = PackageVersion;
            upset.UnityVersion = new VersionSpec();
            upset.UnityVersion.ItemElementName = ItemChoiceType.MinVersion;
            upset.UnityVersion.Item = MinUnityVersion;

            upset.MetaInformation.dirName = FileName;
            return upset;
        }
    }
}