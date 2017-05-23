using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Schemas {

    
    public partial class FileRepository : IRepositoryHandler {
        
        static string packageMatchPattern = @"([^/\\]*)~(.*)";
        static string formatPattern = "{0}{1}{2}~{3}";

        public override void InstallPackage(DependencyDefinition package) {
            string sourcePath = String.Format(formatPattern, this.Path, System.IO.Path.DirectorySeparatorChar, package.Name, package.Version);
            
            string destination = String.Format(formatPattern, installPath, System.IO.Path.DirectorySeparatorChar, package.Name, package.Version);
                
            try {
                FileSystemUtil.copyDirectory(sourcePath, destination);
            } catch (DirectoryNotFoundException) {
                Debug.LogError(String.Format("Package {0} not found in specified version {1}", package.Name, package.Version));
                Directory.Delete(destination);
            }

        }

        public override DependencyDefinition[] ListPackages() {
            string[] packages =  Directory.GetDirectories(this.Path);
            List<DependencyDefinition> ddList = new List<DependencyDefinition>();

            for(int i=0; i<packages.Length; i++) {
                string path = packages[i];
                Match m = Regex.Match(path, packageMatchPattern);
                DependencyDefinition packageDefinition = new DependencyDefinition();
                packageDefinition.Version = m.Groups[2].ToString();
                packageDefinition.Name = m.Groups[1].ToString();

                ddList.Add(packageDefinition);
                
            }
            return ddList.ToArray();
        }

        public override void NukePackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public override void NukeAllPackages() {
            string[] directories = Directory.GetDirectories(installPath);

            foreach(string dir in directories) {
                Directory.Delete(dir, true);
            }
        }

        public override void UninstallPackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }
    }
}