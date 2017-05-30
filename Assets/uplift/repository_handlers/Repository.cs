using System;

namespace Schemas {
    public abstract partial class Repository : IRepositoryHandler
    {
        public const string UpsetFile = "Upset.xml";
        public virtual Upset[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public virtual void InstallPackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdatePackage(Upset package)
        {
            throw new NotImplementedException();
        }
    }
}