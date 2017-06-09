using System;
using Uplift.Common;

namespace Uplift.Schemas {
    public partial class GitRepository : Repository
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