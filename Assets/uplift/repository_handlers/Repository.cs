using System;

namespace Schemas {
    public abstract partial class Repository : IRepositoryHandler
    {

        protected Upfile upfile;

        public virtual void SetContext(Upfile upfile) {
            this.upfile = upfile;
        }


        private static string[] installPathDefinition = {"Assets", "upackages"};
        protected string installPath {
            get { 
                if(this.upfile.PackagesRootPath != null) {
                    return this.upfile.PackagesRootPath;
                }
                return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), installPathDefinition);
            }
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