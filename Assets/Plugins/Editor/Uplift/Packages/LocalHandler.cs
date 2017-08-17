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
            Upbring upbring = Upbring.Instance();
           
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
            return Path.Combine(Upfile.Instance().GetPackagesRootPath(), GetPackageDirectory(package));
        }

        //FIXME: This is super unsafe right now, as we can copy down into the FS.
        // This should be contained using kinds of destinations.
        public static void InstallPackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbringFile = Upbring.Instance();
            // Note: Full package is ALWAYS copied to the upackages directory right now
            string localPackagePath = GetRepositoryInstallPath(package);
            upbringFile.AddPackage(package);
            Uplift.Common.FileSystemUtil.CopyDirectory(td.Path, localPackagePath);
            upbringFile.AddLocation(package, InstallSpecType.Root, localPackagePath);


            InstallSpecPath[] specArray;
            if (package.Configuration == null)
            {
                // If there is no Configuration present we assume
                // that the whole package is wrapped in "InstallSpecType.Base"
                InstallSpecPath wrapSpec = new InstallSpecPath
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

            foreach (InstallSpecPath spec in specArray)
            {
                var sourcePath = Uplift.Common.FileSystemUtil.JoinPaths(td.Path, spec.Path);
                PathConfiguration PH = Upfile.Instance().GetDestinationFor(spec);

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
                    FileSystemUtil.TryCopyMeta(sourcePath, destination);

                    if (destination.StartsWith("Assets"))
                    {
                        TryUpringAddGUID(upbringFile, sourcePath, package, spec.Type, destination);
                    }
                    else
                    {
                        upbringFile.AddLocation(package, spec.Type, destination);
                    }

                }

                // Working with directory
                if (Directory.Exists(sourcePath))
                {
                    // Working with directory
                    Uplift.Common.FileSystemUtil.CopyDirectoryWithMeta(sourcePath, destination);

                    if (destination.StartsWith("Assets"))
                    {
                        foreach (var file in Uplift.Common.FileSystemUtil.RecursivelyListFiles(sourcePath, true))
                        {
                            TryUpringAddGUID(upbringFile, file, package, spec.Type, destination);
                        }
                    }
                    else
                    {
                        foreach (var file in Uplift.Common.FileSystemUtil.RecursivelyListFiles(sourcePath, true))
                        {
                            upbringFile.AddLocation(package, spec.Type, Path.Combine(destination, file));
                        }
                    }
                }            
            }

            upbringFile.SaveFile();

            td.Dispose();
        }

        private static void TryUpringAddGUID(Upbring upbring, string file, Upset package, InstallSpecType type, string destination)
        {
            if (file.EndsWith(".meta")) return;
            string metaPath = Path.Combine(destination, file + ".meta");
            if (!File.Exists(metaPath))
            {
                upbring.AddLocation(package, type, Path.Combine(destination, file));
                return;
            }
            MetaFile meta = MetaFile.FromFile(metaPath);
            upbring.AddGUID(package, type, meta.Guid);
        }

        public static void UpdatePackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbring = Upbring.Instance();

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
            Upbring upbring = Upbring.Instance();
            InstalledPackage package = upbring.GetInstalledPackage(packageName);
            package.Nuke();
        }
    }
}