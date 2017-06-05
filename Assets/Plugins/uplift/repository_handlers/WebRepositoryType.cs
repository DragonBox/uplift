using System;

namespace Schemas {
    public partial class WebRepositoryType : Repository
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