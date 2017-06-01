using System;
using System.IO;
using Schemas;

class LocalHandler
{
    private static string[] installPathDefinition = { "Assets", "upackages" };
    protected static string installPath
    {
        get
        {
            UpfileHandler upfile = UpfileHandler.Instance();
            if (upfile.GetPackagesRootPath() != null)
            {
                return upfile.GetPackagesRootPath();
            }
            return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), installPathDefinition);
        }
    }

    public static string GetLocalDirectory(string name, string version)
    {
        return installPath + System.IO.Path.DirectorySeparatorChar + name + "~" + version;
    }

    public static void NukeAllPackages()
    {
        Upbring upbring = Upbring.FromXml();

        foreach(var package in upbring.InstalledPackage) {
            package.Nuke();
        }

        Schemas.Upbring.RemoveFile();
    }

    public static void InstallPackage(Upset package, TemporaryDirectory td)
    {
        FileSystemUtil.copyDirectory(td.Path, GetLocalDirectory(package.PackageName, package.PackageVersion));
        
        // Mark file in Upbring
        Upbring upbringFile = Upbring.FromXml();
        upbringFile.AddPackage(package);
        upbringFile.SaveFile();

        td.Destroy();
    }

    public static void UpdatePackage(Upset package, TemporaryDirectory td) {
        Upbring upbring = Upbring.FromXml();

        // Nuking previous version
        Schemas.InstalledPackage installedPackage = upbring.GetInstalledPackage(package.PackageName);
        installedPackage.Nuke();

        InstallPackage(package, td);
    }
    
    public static void UpdatePackage(PackageRepo packageRepo) {
        TemporaryDirectory td = packageRepo.Repository.DownloadPackage(packageRepo.Package);
        UpdatePackage(packageRepo.Package, td);
    }

    // What's the difference between Nuke and Uninstall?
    // Nuke doesn't care for dependencies (if present)
    public static void NukePackage(String packageName) {
        Upbring upbring = Upbring.FromXml();
        Schemas.InstalledPackage package = upbring.GetInstalledPackage(packageName);
        package.Nuke();
    }
}