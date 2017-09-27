using Uplift.Packages;
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
            AssetDatabase.Refresh();
        }

        public static void UpdatePackage() {
            string packageName = LastArgument();

            PackageRepo pr = PackageList.Instance().GetLatestPackage(packageName);
            UpliftManager.Instance().UpdatePackage(pr);
            AssetDatabase.Refresh();
        }

        public static void NukePackage() {
            string packageName = LastArgument();
            UpliftManager.Instance().NukePackage(packageName);
            AssetDatabase.Refresh();
        }

        public static void NukeAllPackages() {
            UpliftManager.Instance().NukeAllPackages();
            AssetDatabase.Refresh();
        }
    }

}