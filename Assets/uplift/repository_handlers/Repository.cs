using System;

namespace Schemas {
    public abstract partial class Repository : IRepositoryHandler
    {
        private static string[] installPathDefinition = {"Assets", "uplift", "packages"};
        protected static string installPath {
            get { return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), installPathDefinition); }
        }
        public virtual void InstallPackage(DependencyDefinition package)
        {
            throw new NotImplementedException();
        }

        public virtual DependencyDefinition[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public virtual void NukeAllPackages()
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