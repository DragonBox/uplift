using System;

namespace Schemas {
    public partial class GitRepositoryType : Repository
    {
        public override TemporaryDirectory DownloadPackage(Upset package)
        {
            throw new NotImplementedException();
        }

        public override Upset[] ListPackages()
        {
            throw new NotImplementedException();
        }

    }
}