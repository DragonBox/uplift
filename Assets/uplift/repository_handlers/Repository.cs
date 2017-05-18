using System;

namespace Schemas {
    public abstract partial class Repository : IRepositoryHandler {
        public virtual void InstallPackage(DependencyDefinition package) {
            throw new NotImplementedException();
        }
    }
}