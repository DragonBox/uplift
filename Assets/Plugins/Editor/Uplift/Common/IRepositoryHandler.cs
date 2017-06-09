namespace Uplift.Common
{
    using Schemas;
    
    internal interface IRepositoryHandler {
        Upset[] ListPackages();
        // IDEA: we could also use, and let the caller manager / dispose the target
        // void DownloadPackageTo(Upset package, TemporaryDirectory target);
        TemporaryDirectory DownloadPackage(Upset package);
    }
}