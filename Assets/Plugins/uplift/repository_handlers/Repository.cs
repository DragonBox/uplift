using System;

namespace Uplift.Schemas {
    public abstract partial class Repository : IRepositoryHandler
    {
        public const string UpsetFile = "Upset.xml";
        public virtual Upset[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public virtual TemporaryDirectory DownloadPackage(Upset package)
        {
            throw new NotImplementedException();
        }
    }
}