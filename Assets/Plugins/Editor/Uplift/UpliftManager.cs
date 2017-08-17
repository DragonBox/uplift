using System.IO;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift
{
    class UpliftManager
    {
        // --- SINGLETON DECLARATION ---
        protected static UpliftManager instance;

        internal UpliftManager() {
            upfile = Upfile.Instance();
        }

        public static UpliftManager Instance()
        {
            if (instance == null)
            {
                InitializeInstance();
            }

            return instance;
        }

        internal static void InitializeInstance()
        {
            instance = new UpliftManager();
        }

        // --- CLASS DECLARATION ---
        protected Upfile upfile;

        public void InstallDependencies()
        {
            //FIXME: We should check for all repositories, not the first one
            //FileRepository rt = (FileRepository) Upfile.Repositories[0];

            PackageHandler pHandler = new PackageHandler();

            foreach (DependencyDefinition packageDefinition in upfile.Dependencies)
            {
                PackageRepo result = pHandler.FindPackageAndRepository(packageDefinition);
                if (result.Repository != null)
                {
                    using (TemporaryDirectory td = result.Repository.DownloadPackage(result.Package))
                    {
                        InstallPackage(result.Package, td);
                    }
                }
            }
        }

        public void NukeAllPackages()
        {
            Upbring upbring = Upbring.Instance();

            foreach (InstalledPackage package in upbring.InstalledPackage)
            {
                package.Nuke();
            }

            //TODO: Remove file when Upbring properly removes everything
            Upbring.RemoveFile();
        }

        public string GetPackageDirectory(Upset package)
        {
            return package.PackageName + "~" + package.PackageVersion;
        }

        public string GetRepositoryInstallPath(Upset package)
        {
            return Path.Combine(upfile.GetPackagesRootPath(), GetPackageDirectory(package));
        }

        //FIXME: This is super unsafe right now, as we can copy down into the FS.
        // This should be contained using kinds of destinations.
        public void InstallPackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbring = Upbring.Instance();
            // Note: Full package is ALWAYS copied to the upackages directory right now
            string localPackagePath = GetRepositoryInstallPath(package);
            upbring.AddPackage(package);
            FileSystemUtil.CopyDirectory(td.Path, localPackagePath);
            upbring.AddLocation(package, InstallSpecType.Root, localPackagePath);


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

                specArray = new[] { wrapSpec };
            }
            else
            {
                specArray = package.Configuration;
            }

            foreach (InstallSpecPath spec in specArray)
            {
                var sourcePath = Uplift.Common.FileSystemUtil.JoinPaths(td.Path, spec.Path);
                PathConfiguration PH = upfile.GetDestinationFor(spec);

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
                        TryUpringAddGUID(upbring, sourcePath, package, spec.Type, destination);
                    }
                    else
                    {
                        upbring.AddLocation(package, spec.Type, destination);
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
                            TryUpringAddGUID(upbring, file, package, spec.Type, destination);
                        }
                    }
                    else
                    {
                        foreach (var file in Uplift.Common.FileSystemUtil.RecursivelyListFiles(sourcePath, true))
                        {
                            upbring.AddLocation(package, spec.Type, Path.Combine(destination, file));
                        }
                    }
                }
            }

            upbring.SaveFile();

            td.Dispose();
        }

        private void TryUpringAddGUID(Upbring upbring, string file, Upset package, InstallSpecType type, string destination)
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

        public void UpdatePackage(Upset package, TemporaryDirectory td)
        {
            Upbring upbring = Upbring.Instance();

            // Nuking previous version
            InstalledPackage installedPackage = upbring.GetInstalledPackage(package.PackageName);
            installedPackage.Nuke();

            InstallPackage(package, td);
        }

        public void UpdatePackage(PackageRepo packageRepo)
        {
            using (TemporaryDirectory td = packageRepo.Repository.DownloadPackage(packageRepo.Package))
            {
                UpdatePackage(packageRepo.Package, td);
            }
        }

        // What's the difference between Nuke and Uninstall?
        // Nuke doesn't care for dependencies (if present)
        public void NukePackage(string packageName)
        {
            Upbring upbring = Upbring.Instance();
            InstalledPackage package = upbring.GetInstalledPackage(packageName);
            package.Nuke();
        }
    }
}
