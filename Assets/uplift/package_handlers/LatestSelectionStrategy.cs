using System;
using System.Linq;

class LatestSelectionStrategy : CandidateSelectionStrategy
{
    public override PackageHandler.PackageRepo[] Filter(PackageHandler.PackageRepo[] candidates)
    {
        return candidates.OrderBy(pr => pr.package.PackageVersionAsNumber()).Take(1).ToArray();
    }
}