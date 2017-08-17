using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.Common
{
    public static class Cli
    {
        public static string LastArgument() {
            string[] args = System.Environment.GetCommandLineArgs();
            return args[args.Length - 1];
        }

        public static void InstallDependencies() {
            Upfile.Instance().InstallDependencies();
        }

        public static void UpdatePackage() {
            string packageName = LastArgument();

            PackageRepo pr = PackageList.Instance().GetLatestPackage(packageName);
            LocalHandler.UpdatePackage(pr);
        }

        public static void NukePackage() {
            string packageName = LastArgument();
            LocalHandler.NukePackage(packageName);
        }
    }

}