using Uplift.Common;

namespace Uplift.Strategies
{
    public abstract class CandidateSelectionStrategy
    {
        public abstract PackageRepo[] Filter(PackageRepo[] candidates);
    }
}
