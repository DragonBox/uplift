using Uplift.Packages;
using UnityEngine;
using UnityEditor;

namespace Uplift.Common
{
    public static class Cli
    {
        public static string LastArgument() {
            string[] args = System.Environment.GetCommandLineArgs();
            return args[args.Length - 1];
        }

        public static void InstallDependencies() {
            UpliftManager.Instance().InstallDependencies();
        }

        public static void UpdatePackage() {
            string packageName = LastArgument();

            PackageRepo pr = PackageList.Instance().GetLatestPackage(packageName);
            UpliftManager.Instance().UpdatePackage(pr);
        }

        public static void NukePackage() {
            string packageName = LastArgument();
            UpliftManager.Instance().NukePackage(packageName);
        }

        public static void NukeAllPackages() {
            UpliftManager.Instance().NukeAllPackages();
        }

        public static string defaultPathsFile = ".simplebuild";

        // Note - in order for PackageModule to work, you need to have '.simplebuild' file with
        // newline-separated list of directories to package.

        public static void PackageModule() {
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

                PackageInfo pi = new PackageInfo() {
                    name = PlayerSettings.productName,
                    version = PlayerSettings.bundleVersion,
                    license = "undefined",
                    paths = packageExportData.pathsToStringArray()
                };


                CheckForOverrideData("Package Name", ref pi.name, packageExportData.packageName);
                CheckForOverrideData("Package Version", ref pi.version, packageExportData.packageVersion);
                CheckForOverrideData("License", ref pi.license, packageExportData.license);


                exporter.SetPackageInfo(pi);

                exporter.Export();
            }


        }

        private static void CheckForOverrideData(string what, ref string original, string overrideData) {
            if(!string.IsNullOrEmpty(overrideData)) {

                Debug.Log(string.Format("NOTE: {0} overriden by Package Export Specification ({1} -> {2})",
                                        what, original, overrideData ));
                original = overrideData;
            }

        }
    }

}
