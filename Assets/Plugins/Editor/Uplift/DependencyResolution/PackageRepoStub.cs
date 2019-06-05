using System.Collections.Generic;


namespace Uplift.DependencyResolution
{
	class PackageRepoStub
	{
		Dictionary<string, string[]> packageRepo;

		public PackageRepoStub(Dictionary<string, string[]> packageRepo)
		{
			this.packageRepo = packageRepo;
		}

		public string[] GetVersions(string requirementName)
		{
			return packageRepo[requirementName];
		}

	}
}