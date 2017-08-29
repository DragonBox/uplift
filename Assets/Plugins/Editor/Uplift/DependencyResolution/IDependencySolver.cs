using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    interface IDependencySolver
    {
        DependencyDefinition[] SolveDependencies(DependencyDefinition[] dependencies);
    }
}
