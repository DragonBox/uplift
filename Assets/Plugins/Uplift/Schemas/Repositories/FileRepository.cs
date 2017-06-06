using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;

namespace Uplift.Schemas {
    
    public partial class FileRepository : IRepositoryHandler {
        
        private const string formatPattern = "{0}{1}{2}";

        public override TemporaryDirectory DownloadPackage(Upset package) {
            var sourcePath = string.Format(formatPattern, Path, System.IO.Path.DirectorySeparatorChar, package.MetaInformation.dirName);
            var td = new TemporaryDirectory();

            //string destination = LocalHandler.GetLocalDirectory(package.PackageName, package.PackageVersion);
                
            try {
                FileSystemUtil.CopyDirectory(sourcePath, td.Path);
            } catch (DirectoryNotFoundException) {
                Debug.LogError(string.Format("Package {0} not found in specified version {1}", package.PackageName, package.PackageVersion));
                td.Destroy();
            }

            return td;
        }

        public override Upset[] ListPackages() {
            string[] directories =  Directory.GetDirectories(Path);
            var upsetList = new List<Upset>();

            foreach(string directoryName in directories) {
                string upsetPath = directoryName + System.IO.Path.DirectorySeparatorChar + UpsetFile;
                
                if (!File.Exists(upsetPath)) continue;
                
                var serializer = new XmlSerializer(typeof(Upset));
                var file = new FileStream(upsetPath, FileMode.Open);
                var upset = serializer.Deserialize(file) as Upset;
                
                upset.MetaInformation.dirName = directoryName;
                file.Close();
                upsetList.Add(upset);
            }

            return upsetList.ToArray();
        }

    }
}