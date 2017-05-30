using System;

namespace Schemas {
    public partial class GitRepositoryType : Repository
    {
        public override void InstallPackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public override Upset[] ListPackages()
        {
            throw new NotImplementedException();
        }

        public override void UpdatePackage(Upset package)
        {
            throw new NotImplementedException();
        }
    }
}