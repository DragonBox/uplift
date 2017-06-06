namespace Uplift.Common
{
    using Schemas;
    
    internal interface IRepositoryHandler {
        Upset[] ListPackages();
        TemporaryDirectory DownloadPackage(Upset package);
    }
}