using System.Collections.Generic;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.Testing.Helpers
{
    public class PackageListTester : PackageList
    {
        private static PackageListTester _testing_instance;

        public static PackageListTester TestingInstance()
        {
            return _testing_instance ?? (_testing_instance = new PackageListTester());
        }

        public void Clear()
        {
            this.Repositories = new Repository[0];
            this.Packages.Clear();
        }

        public void SetRepositories(Repository[] _repositories)
        {
            this.Repositories = _repositories;
        }

        public Repository[] GetRepositories()
        {
            return Repositories;
        }

        public void SetPackages(List<PackageRepo> _packages)
        {
            this.Packages = _packages;
        }

        public List<PackageRepo> GetPackages()
        {
            return Packages;
        }
    }
}
