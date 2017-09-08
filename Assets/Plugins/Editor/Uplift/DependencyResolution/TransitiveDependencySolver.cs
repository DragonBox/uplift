using System.Collections.Generic;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    class TransitiveDependencySolver : IDependencySolver
    {
        public event DependencyHelper.ConflictChecker CheckConflict;

        public DependencyDefinition[] SolveDependencies(DependencyDefinition[] dependencies)
        {
            DependencyGraph dependencyGraph = GenerateGraph(dependencies);
            TarjanCycleDetector cycleDetector = new TarjanCycleDetector();

            List<List<DependencyNode>> cycles = cycleDetector.DetectCycles(dependencyGraph);

            // TODO: Save the current dependency tree so the whole tree doesn't have to be solved entirely later on

            return GetDependencyDefinitions(dependencyGraph);
        }

        private DependencyGraph GenerateGraph(DependencyDefinition[] dependencies)
        {
            DependencyGraph graph = new DependencyGraph();
            PackageList packageList = PackageList.Instance();

            foreach (DependencyDefinition dependency in dependencies)
            {
                DependencyNode current;
                graph.LoadDependencies(dependency, packageList, CheckConflict, out current);
            }

            return graph;
        }

        private DependencyDefinition[] GetDependencyDefinitions(DependencyGraph graph)
        {
            DependencyDefinition[] result = new DependencyDefinition[graph.nodeList.Count];
            DependencyNode current;
            for(int i = 0; i < graph.nodeList.Count; i++)
            {
                current = graph.nodeList[i];
                result[i] = new DependencyDefinition()
                {
                    Name = current.Name,
                    Version = current.Requirement.ToString(),
                    Repository = current.Repository,
                    Requirement = current.Requirement
                };
            }

            return result;
        }
    }
}
