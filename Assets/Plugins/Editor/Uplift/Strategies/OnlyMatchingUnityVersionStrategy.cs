using Uplift.Common;
using System.Collections.Generic;

namespace Uplift.Strategies {
    internal class OnlyMatchingUnityVersionStrategy : CandidateSelectionStrategy {

        VersionStruct unityVersion;

        public OnlyMatchingUnityVersionStrategy(string unityVersion) {
            this.unityVersion = VersionParser.ParseVersion(unityVersion, false);
        }

        public override PackageRepo[] Filter(PackageRepo[] candidates) {
            var result = new List<PackageRepo>();

            foreach(PackageRepo item in candidates) {
                VersionStruct packageUnityRequirement = VersionParser.ParseVersion(item.Package.UnityVersion, false);

                if(unityVersion >= packageUnityRequirement) {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }
    }
}
