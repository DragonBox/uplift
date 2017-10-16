using Uplift.Packages;
using UnityEditor;
using System.IO;

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
            // Read file .simplebuild

            string[] paths;

            paths = File.ReadAllText(defaultPathsFile).Split('\n');

            Exporter exporter = new Exporter();

            PackageInfo pi = new PackageInfo() {
                name = PlayerSettings.productName,
                version = PlayerSettings.bundleVersion,
                license = "undefined",
                paths = paths
            };

            exporter.SetPackageInfo(pi);

            exporter.Export();

        }
    }

}
