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
            string[] directories =  Directory.GetDirectories(Path);
            List<Upset> upsetList = new List<Upset>();

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

            return upsetList.ToArray();
        }

    }
}