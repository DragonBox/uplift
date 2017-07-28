using System;
using Uplift.Schemas;
using Uplift.Packages;

namespace UpliftTesting.Helpers
{
	// Some protected methods of the Package Handler need to be tested
	// This class offers public wrappers for the methods that need testing
	public class PackageHandlerTester : PackageHandler
	{
		public CompareResult CompareVersionsWrapper(VersionStruct packageVersion, VersionStruct dependencyVersion)
		{
			return CompareVersions(packageVersion, dependencyVersion);
		}

        public CompareResult CompareVersionsWrapper(Upset package, DependencyDefinition dependencyDefinition)
        {
            return CompareVersions(package, dependencyDefinition);
        }
    }
}

