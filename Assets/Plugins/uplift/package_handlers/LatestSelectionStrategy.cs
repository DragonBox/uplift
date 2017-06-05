using System;
using System.Linq;

namespace Uplift
{
    class LatestSelectionStrategy : CandidateSelectionStrategy
    {
        public override PackageRepo[] Filter(PackageRepo[] candidates)
        {
            return candidates.OrderBy(pr => pr.Package.PackageVersionAsNumber()).Take(1).ToArray();
        }
    }
}