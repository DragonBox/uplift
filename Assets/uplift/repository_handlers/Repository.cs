using System;

namespace Schemas {
    public abstract partial class Repository : IRepositoryHandler
    {
        public virtual void InstallPackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public virtual DependencyDefinition[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public virtual void NukePackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public virtual void UninstallPackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdatePackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }
    }
}