using Uplift.Common;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;



namespace Uplift.Strategies
{
    internal class FindBestPackageForUnityVersion : CandidateSelectionStrategy
    {
        protected VersionStruct unityVersion;

        public FindBestPackageForUnityVersion(string unityVersion) {
            this.unityVersion = VersionParser.ParseVersion(unityVersion, false);
        }

        public override PackageRepo[] Filter(PackageRepo[] candidates) {

            var result = new List<PackageRepo>();

            var grouped = candidates.GroupBy(pr => new {
                    pr.Package.PackageName,
                    pr.Package.PackageVersion
                });

            foreach(var group in grouped) {
                var closestMatch = group.OrderByDescending(i => VersionParser.ParseVersion(i.Package.UnityVersion, false).NumeralForm()).First();
                result.Add(closestMatch);
            }

            return result.ToArray();


        }
    }
}
