using System;

namespace Schemas {
    public abstract partial class RepositoryType : IRepositoryHandler {
        public virtual void InstallPackage(PackageType package) {
            throw new NotImplementedException();
        }
    }
}