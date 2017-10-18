using UnityEngine;
using UnityEditor;

using Uplift.Schemas;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Uplift {
    class Exporter {
        public string unityVersion;

        PackageExportData exportSpec;
        Upset upset;

        public Exporter() {
            unityVersion = Application.unityVersion;
        }

        public void Export() {

            // Prepare items to export
            var exportEntries = new List<string>();

            for(int i=0; i<exportSpec.paths.Length;i++) {

                string path = exportSpec.paths[i];

                if(System.IO.File.Exists(path)) {
                    exportEntries.Add(path);

                } else if (System.IO.Directory.Exists(path)) {
                    string[] tFiles = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    string[] tDirectories = System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                    exportEntries.AddRange(tFiles);
                    exportEntries.AddRange(tDirectories);
                }


            }

            // Calculate package file basename
            string packageBasename = string.Format("{0}~{1}", exportSpec.packageName, exportSpec.packageVersion);

            CreateTargetDir();

            // Write things to disk
            WriteUpsetFile(Path.Combine(exportSpec.targetDir, packageBasename) + ".Upset.xml");
            AssetDatabase.ExportPackage(exportEntries.ToArray(), Path.Combine(exportSpec.targetDir,packageBasename) + ".unitypackage", ExportPackageOptions.Default);

        }

        public void SetExportSpec(PackageExportData exportSpec) {
            this.exportSpec = exportSpec;
            SetUpset();
        }

        protected void SetUpset() {
            upset = new Upset() {
                UnityVersion = Application.unityVersion,
                PackageName = exportSpec.packageName,
                PackageLicense = exportSpec.license,
                PackageVersion = exportSpec.packageVersion
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
            if(!Directory.Exists(exportSpec.targetDir)) {
                Directory.CreateDirectory(exportSpec.targetDir);
            }

        }

        // Convenience method for packing everything according to
        // PackageExportData objects

        public static void PackageEverything() {
            var guids = AssetDatabase.FindAssets("t:PackageExportData");

            if(guids.Length == 0) {
                throw new System.Exception("PackageExportData doesn't exist. Create at least one using Uplift -> Create Export Spec");
            }


            Debug.Log(string.Format("{0} Package Export Specification(s) found. Preparing for export", guids.Length));

            for(int i=0; i<guids.Length;i++) {

                string packageExportPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                PackageExportData packageExportData = AssetDatabase.LoadAssetAtPath<PackageExportData>(packageExportPath);

                Debug.Log(string.Format("Export {0}/{1} using {2}", i+1, guids.Length, packageExportPath));

                Exporter exporter = new Exporter();

                PackageExportData exportData = new PackageExportData() {
                    packageName = PlayerSettings.productName,
                    packageVersion = PlayerSettings.bundleVersion,
                    license = "undefined",
                    paths = packageExportData.paths
                };


                CheckForOverrideData("Package Name", ref exportData.packageName, packageExportData.packageName);
                CheckForOverrideData("Package Version", ref exportData.packageVersion, packageExportData.packageVersion);
                CheckForOverrideData("License", ref exportData.license, packageExportData.license);


                exporter.SetExportSpec(exportData);

                exporter.Export();
            }

        }

        protected static void CheckForOverrideData(string what, ref string original, string overrideData) {
                if(!string.IsNullOrEmpty(overrideData)) {

                    Debug.Log(string.Format("NOTE: {0} overriden by Package Export Specification ({1} -> {2})",
                                            what, original, overrideData ));
                    original = overrideData;
                }
            }


    }

}
