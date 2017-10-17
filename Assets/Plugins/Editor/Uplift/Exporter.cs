using UnityEngine;
using UnityEditor;

using PackageInfo = Uplift.Common.PackageInfo;

using Uplift.Schemas;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Uplift {
    class Exporter {
        public string unityVersion;

        PackageInfo packageInfo;
        Upset upset;

        public Exporter() {
            unityVersion = Application.unityVersion;
        }

        public void Export() {

            // Prepare items to export
            var exportEntries = new List<string>();

            for(int i=0; i<packageInfo.paths.Length;i++) {

                string path = packageInfo.paths[i];

                if(System.IO.File.Exists(path)) {
                    exportEntries.Add(path);

                } else if (System.IO.Directory.Exists(path)) {
                    string[] tFiles = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    string[] tDirectories = System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                    exportEntries.AddRange(tFiles);
                    exportEntries.AddRange(tDirectories);
                }


            }

            // Calculate package basename
            string packageBasename = string.Format("{0}~{1}", packageInfo.name, packageInfo.version);

            CreateTargetDir();

            // Write things to disk
            WriteUpsetFile(Path.Combine(packageInfo.targetDir, packageBasename) + ".Upset.xml");
            AssetDatabase.ExportPackage(exportEntries.ToArray(), Path.Combine(packageInfo.targetDir,packageBasename) + ".unitypackage", ExportPackageOptions.Default);

        }

        public void SetPackageInfo(PackageInfo pi) {
            packageInfo = pi;
            SetUpset();
        }

        protected void SetUpset() {
            upset = new Upset() {
                UnityVersion = Application.unityVersion,
                PackageName = packageInfo.name,
                PackageLicense = packageInfo.license,
                PackageVersion = packageInfo.version
            };
        }

        protected void WriteUpsetFile(string file) {
            XmlSerializer serializer = new XmlSerializer(typeof(Upset));
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    serializer.Serialize(sw, this.upset);
                }
            }

        }

        protected void CreateTargetDir() {
            if(!Directory.Exists(packageInfo.targetDir)) {
                Directory.CreateDirectory(packageInfo.targetDir);
            }

        }
    }

}
