using System;

namespace Schemas {
    public partial class GitRepositoryType : Repository
    {
        public override void InstallPackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public override DependencyDefinition[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public override void NukePackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public override void UninstallPackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }
    }
}