// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using UnityEngine;
using UnityEditor;

using Uplift.Schemas;

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;



namespace Uplift.Export
{
    class Exporter
    {
        PackageExportData exportSpec;

        public void Export()
        {

            // Prepare list of entries to export
            var exportEntries = new List<string>();

            for(int i=0; i<exportSpec.paths.Length;i++)
            {

                string path = exportSpec.paths[i];

                if(System.IO.File.Exists(path))
                {
                    exportEntries.Add(path);

                }
                else if (System.IO.Directory.Exists(path))
                {
                    string[] tFiles = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    string[] tDirectories = System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                    exportEntries.AddRange(tFiles);
                    exportEntries.AddRange(tDirectories);
                }


            }

            // Calculate package file basename
            string packageBasename = string.Format("{0}-{1}", exportSpec.packageName, exportSpec.packageVersion);

            // Create Target Directory
            if(!Directory.Exists(exportSpec.targetDir))
            {
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

        public void SetExportSpec(PackageExportData exportSpec)
        {
            this.exportSpec = exportSpec;
        }

        protected void WriteUpsetFile(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Upset));

            Upset template;
            if(string.IsNullOrEmpty(exportSpec.TemplateUpsetPath))
            {
                Debug.LogWarning("No template Upset specified, dependencies and configuration will not follow through");
                template = new Upset();
            }
            else
            {
                using (FileStream fs = new FileStream(exportSpec.TemplateUpsetPath, FileMode.Open))
                {
                    template = serializer.Deserialize(fs) as Upset;
                }
            }

            var upset = new Upset() {
                UnityVersion = Application.unityVersion,
                PackageName = exportSpec.packageName,
                PackageLicense = exportSpec.license,
                PackageVersion = exportSpec.packageVersion,
                Dependencies = template.Dependencies,
                Configuration = template.Configuration
            };

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

        public static void PackageEverything()
        {
            var guids = AssetDatabase.FindAssets("t:PackageExportData");

            if(guids.Length == 0)
            {
                throw new System.Exception("PackageExportData doesn't exist. Create at least one using Uplift -> Create Export Spec.");
            }


            Debug.LogFormat("{0} Package Export Specification(s) found. Preparing for export.", guids.Length);

            for(int i=0; i<guids.Length;i++)
            {

                string packageExportPath = AssetDatabase.GUIDToAssetPath(guids[i]);
#if UNITY_5_6_OR_NEWER
                PackageExportData packageExportData = AssetDatabase.LoadAssetAtPath<PackageExportData>(packageExportPath);
#else
                PackageExportData packageExportData = (PackageExportData)AssetDatabase.LoadAssetAtPath(packageExportPath, typeof(PackageExportData));
#endif
                Debug.LogFormat("Export {0}/{1} using {2}", i+1, guids.Length, packageExportPath);

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

        public static PackageExportData GetDefaultExportData()
        {
                PackageExportData defaultExportData = ScriptableObject.CreateInstance<PackageExportData>();

                defaultExportData.packageName = PlayerSettings.productName;
                defaultExportData.packageVersion = PlayerSettings.bundleVersion;
                defaultExportData.license = "undefined";

                return defaultExportData;

        }
    }
}
