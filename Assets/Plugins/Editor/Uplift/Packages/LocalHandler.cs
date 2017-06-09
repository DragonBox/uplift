using System.IO;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.Packages
{
    internal class LocalHandler
    {
        private static string[] _installPathDefinition = { "Assets", "Plugins", "Upackages" };
        protected static string InstallPath
        {
            get
            {
                UpfileHandler upfile = UpfileHandler.Instance();
                if (upfile.GetPackagesRootPath() != null)
                {
                    return upfile.GetPackagesRootPath();
                }
                return string.Join(Path.DirectorySeparatorChar.ToString(), _installPathDefinition);
            }
        }

        public static string GetLocalDirectory(string name, string version)
        {
            return InstallPath + Path.DirectorySeparatorChar + name + "~" + version;
        }

        public static void NukeAllPackages()
        {
            Upbring upbring = Upbring.FromXml();

            foreach (InstalledPackage package in upbring.InstalledPackage)
            {
                package.Nuke();
            }

            Upbring.RemoveFile();
        }

        //FIXME: This is super unsafe right now, as we can copy down into the FS.
        // This should be contained using kinds of destinations.
        public static void InstallPackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbringFile = Upbring.FromXml();
            // Note: Full package is ALWAYS copied to the upackages directory right now
            string localPackagePath = GetLocalDirectory(package.PackageName, package.PackageVersion);
            upbringFile.AddPackage(package);
            FileSystemUtil.CopyDirectory(td.Path, localPackagePath);
            upbringFile.AddLocation(package, KindSpec.Base, localPackagePath);

            if (package.InstallSpecifications != null)
            {
                foreach (InstallSpec spec in package.InstallSpecifications)
                {
                    string sourcePath = Path.Combine(td.Path, spec.Source);

                    if (File.Exists(sourcePath))
                    {
                        // Working with singular file
                        string directoryPath = Path.GetDirectoryName(spec.Destination);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        File.Copy(sourcePath, spec.Destination);

                    }

                    if (Directory.Exists(sourcePath))
                    {
                        // Working with directory
                        FileSystemUtil.CopyDirectory(sourcePath, spec.Destination);
                    }

                    upbringFile.AddLocation(package, KindSpec.Other, spec.Destination);
                }
            }
            // Mark file in Upbring


            upbringFile.SaveFile();

            td.Dispose();
        }

        public static void UpdatePackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbring = Upbring.FromXml();

            // Nuking previous version
            InstalledPackage installedPackage = upbring.GetInstalledPackage(package.PackageName);
            installedPackage.Nuke();

            InstallPackage(package, td);
        }

        public static void UpdatePackage(PackageRepo packageRepo)
        {
            using(TemporaryDirectory td = packageRepo.Repository.DownloadPackage(packageRepo.Package))
            {
                UpdatePackage(packageRepo.Package, td);
            }
        }

        // What's the difference between Nuke and Uninstall?
        // Nuke doesn't care for dependencies (if present)
        public static void NukePackage(string packageName)
        {
            Upbring upbring = Upbring.FromXml();
            InstalledPackage package = upbring.GetInstalledPackage(packageName);
            package.Nuke();
        }
    }
}