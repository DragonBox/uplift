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
            PackageRepo[] packageRepos = packageList.GetAllPackages();

            foreach (DependencyDefinition dependency in dependencies)
            {
                LoadDependencies(dependency, graph, packageList);
            }

            return graph;
        }

        private DependencyNode LoadDependencies(DependencyDefinition dependency, DependencyGraph graph, PackageList packageList)
        {
            DependencyNode current = new DependencyNode(dependency.Name, dependency.Version, dependency.Repository);
            if(graph.Contains(dependency.Name))
            {
                DependencyNode existing = graph.FindByName(dependency.Name) as DependencyNode;
                CheckConflict(ref existing, current);
            }
            else
            {
                graph.nodes.Add(current);

                Upset package = packageList.GetLatestPackage(dependency.Name).Package;
                if(package == null)
                {
                    throw new MissingDependencyException(string.Format(" depends on {0} but it is not present in any of your specified repository", dependency.Name));
                }

                DependencyNode child;
                if(package.Dependencies != null)
                {
                    foreach (DependencyDefinition packageDependency in package.Dependencies)
                    {
                        try
                        {
                            child = LoadDependencies(packageDependency, graph, packageList);
                            graph.AddDependency(current, child);
                        }
                        catch (MissingDependencyException e)
                        {
                            throw new MissingDependencyException(dependency.Name + e.Message);
                        }
                    }
                }                
            }
            return current;
        }

        private DependencyDefinition[] GetDependencyDefinitions(DependencyGraph graph)
        {
            DependencyDefinition[] result = new DependencyDefinition[graph.nodes.Count];
            DependencyNode current;
            for(int i = 0; i < graph.nodes.Count; i++)
            {
                current = graph.nodes[i];
                result[i] = new DependencyDefinition()
                {
                    Name = current.Name,
                    Version = current.Version,
                    Repository = current.Repository
                };
            }

            return result;
        }
    }
}
