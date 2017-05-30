using System;
using System.IO;

class LocalHandler {
        private static string[] installPathDefinition = {"Assets", "upackages"};
        protected static string installPath {
            get { 
                UpfileHandler upfile = UpfileHandler.Instance();
                if(upfile.GetPackagesRootPath() != null) {
                    return upfile.GetPackagesRootPath();
                }
                return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), installPathDefinition);
            }
        }

        public static string GetLocalDirectory(string name, string version) {
            return installPath + System.IO.Path.DirectorySeparatorChar + name + "~" + version;
        }

        public static void NukeAllPackages() {
            string[] directories = Directory.GetDirectories(installPath);

            foreach(string dir in directories) {
                Directory.Delete(dir, true);
            }

            Schemas.Upbring.RemoveFile();
        }
}