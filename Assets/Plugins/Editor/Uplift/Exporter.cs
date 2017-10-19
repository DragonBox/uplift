using UnityEngine;
using UnityEditor;

using Uplift.Schemas;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Uplift {
    class Exporter {
        PackageExportData exportSpec;

        public void Export() {

            // Prepare list of entries to export
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

            // Create Target Directory
            if(!Directory.Exists(exportSpec.targetDir)) {
                Directory.CreateDirectory(exportSpec.targetDir);
            }

            // Write things to disk
            // Upset
            WriteUpsetFile(Path.Combine(exportSpec.targetDir, packageBasename) + ".Upset.xml");

            // .unitypackage file
            AssetDatabase.ExportPackage(
                                        exportEntries.ToArray(),
                                        Path.Combine(exportSpec.targetDir,packageBasename) + ".unitypackage",
                                        ExportPackageOptions.Default
                                        );

        }

        public void SetExportSpec(PackageExportData exportSpec) {
            this.exportSpec = exportSpec;
        }

        protected void WriteUpsetFile(string file) {
            var upset = new Upset() {
                UnityVersion = Application.unityVersion,
                PackageName = exportSpec.packageName,
                PackageLicense = exportSpec.license,
                PackageVersion = exportSpec.packageVersion
            };

            XmlSerializer serializer = new XmlSerializer(typeof(Upset));
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    serializer.Serialize(sw, upset);
                }
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

                // Preparing exporter instance
                Exporter exporter = new Exporter();

                // Checking which defaults had been overriden
                packageExportData.SetOrCheckOverridenDefaults(GetDefaultExportData());

                // Setting exporter spec
                exporter.SetExportSpec(packageExportData);

                // Export of set package
                exporter.Export();
            }

        }

        public static PackageExportData GetDefaultExportData() {
                PackageExportData defaultExportData = ScriptableObject.CreateInstance<PackageExportData>();

                defaultExportData.packageName = PlayerSettings.productName;
                defaultExportData.packageVersion = PlayerSettings.bundleVersion;
                defaultExportData.license = "undefined";

                return defaultExportData;

        }


    }

}
