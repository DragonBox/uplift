using System.IO;
using System.Linq;
using UnityEngine;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.Packages
{
    internal class LocalHandler
    {
        public static void NukeAllPackages()
        {
            Upbring upbring = Upbring.FromXml();
           
            

            foreach (InstalledPackage package in upbring.InstalledPackage)
            {
                package.Nuke();
            }

            //TODO: Remove file when Upbring properly removes everything
            Upbring.RemoveFile();
        }

        public static string GetPackageDirectory(Upset package)
        {
            return package.PackageName + "~" + package.PackageVersion;
        }

        public static string GetRepositoryInstallPath(Upset package)
        {
            var UH = UpfileHandler.Instance();

            return Path.Combine(UH.GetPackagesRootPath(), GetPackageDirectory(package));
        }

        //FIXME: This is super unsafe right now, as we can copy down into the FS.
        // This should be contained using kinds of destinations.
        public static void InstallPackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbringFile = Upbring.FromXml();
            // Note: Full package is ALWAYS copied to the upackages directory right now
            string localPackagePath = GetRepositoryInstallPath(package);
            upbringFile.AddPackage(package);
            FileSystemUtil.CopyDirectory(td.Path, localPackagePath);
            upbringFile.AddLocation(package, InstallSpecType.Root, localPackagePath);


            InstallSpec[] specArray;
            if (package.Configuration == null)
            {
                // If there is no Configuration present we assume
                // that the whole package is wrapped in "InstallSpecType.Base"
                InstallSpec wrapSpec = new InstallSpec
                {
                    Path = "",
                    Type = InstallSpecType.Base
                };

                specArray = new[] {wrapSpec};
            }
            else
            {
                specArray = package.Configuration;
            }

            foreach (InstallSpec spec in specArray)
            {
                var sourcePath = FileSystemUtil.JoinPaths(td.Path, spec.Path);
                PathConfiguration PH = UpfileHandler.Instance().GetDestinationFor(spec.Type);

                var packageStructurePrefix =
                    PH.SkipPackageStructure ? "" : GetPackageDirectory(package);
                
                var destination = Path.Combine(PH.Location, packageStructurePrefix);
                
                

                // Working with single file
                if (File.Exists(sourcePath))
                {
                    // Working with singular file
                    if (!Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                    }
                    File.Copy(sourcePath, destination);
                    
                    upbringFile.AddLocation(package, spec.Type, destination);

                }

                // Working with directory

                if (Directory.Exists(sourcePath))
                {
                    // Working with directory
                    FileSystemUtil.CopyDirectory(sourcePath, destination);

                    foreach (var file in FileSystemUtil.RecursivelyListFiles(sourcePath, true))
                    {
                        upbringFile.AddLocation(package, spec.Type, Path.Combine(packageStructurePrefix, file));
                    }
                    
                    
                }

                
            }



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