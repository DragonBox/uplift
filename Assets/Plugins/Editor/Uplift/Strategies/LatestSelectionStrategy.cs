using System.Linq;
using Uplift.Common;

namespace Uplift.Strategies
{
    internal class LatestSelectionStrategy : CandidateSelectionStrategy
    {
        public override PackageRepo[] Filter(PackageRepo[] candidates)
        {
            return candidates.OrderByDescending(pr => pr.Package.PackageVersionAsNumber()).Take(1).ToArray();
        }
    }
}