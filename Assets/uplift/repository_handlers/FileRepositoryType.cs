using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Schemas {

    
    
    public partial class FileRepository : IRepositoryHandler {
        
        static string formatPattern = "{0}{1}{2}";

        public override void InstallPackage(Upset package) {
            string sourcePath = String.Format(formatPattern, this.Path, System.IO.Path.DirectorySeparatorChar, package.MetaInformation.dirName);
            
            string destination = LocalHandler.GetLocalDirectory(package.PackageName, package.PackageVersion);
                
            try {
                FileSystemUtil.copyDirectory(sourcePath, destination);
            } catch (DirectoryNotFoundException) {
                Debug.LogError(String.Format("Package {0} not found in specified version {1}", package.PackageName, package.PackageVersion));
                Directory.Delete(destination);
            }

        }

        public override Upset[] ListPackages() {
            string[] directories =  Directory.GetDirectories(this.Path);
            List<Upset> upsetList = new List<Upset>();

            foreach(string directoryName in directories) {
                string upsetPath = directoryName + System.IO.Path.DirectorySeparatorChar + Repository.UpsetFile;
                if(File.Exists(upsetPath)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(Schemas.Upset));
                    FileStream file = new FileStream(upsetPath, FileMode.Open);
                    Upset upset = serializer.Deserialize(file) as Upset;
                    upset.MetaInformation.dirName = directoryName;
                    file.Close();
                    upsetList.Add(upset);
                }
                
            }

            return upsetList.ToArray();
        }

        public override void NukePackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public override void NukeAllPackages() {
            LocalHandler.NukeAllPackages();

        }

        public override void UninstallPackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePackage(Upset package)
        {
            throw new NotImplementedException();
        }
    }
}