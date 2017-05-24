using System;

namespace Schemas {
    public abstract partial class Repository : IRepositoryHandler
    {

        public const string UpsetFile = "Upset.xml";
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
        public virtual void InstallPackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public virtual Upset[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public virtual void NukeAllPackages()
        {
            throw new NotImplementedException();
        }

        public virtual void NukePackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public virtual void UninstallPackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdatePackage(Upset package)
        {
            throw new NotImplementedException();
        }
    }
}