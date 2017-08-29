using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    class NoTransitiveDependenciesSolver : IDependencySolver
    {
        public DependencyDefinition[] SolveDependencies(DependencyDefinition[] dependencies)
        {
            return dependencies;
        }
    }
}
