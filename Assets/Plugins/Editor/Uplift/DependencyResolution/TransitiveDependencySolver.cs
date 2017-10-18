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
            VisualizeDependencies(dependencies);

            DependencyGraph dependencyGraph = GenerateGraph(dependencies);
            TarjanCycleDetector cycleDetector = new TarjanCycleDetector();

            cycleDetector.DetectCycles(dependencyGraph);

            // TODO: Save the current dependency tree so the whole tree doesn't have to be solved entirely later on
            DependencyDefinition[] solvedDependencies = GetDependencyDefinitions(dependencyGraph);
            string result = "# DEPENDENCIES\n";
            foreach(DependencyDefinition def in solvedDependencies)
            {
                PackageRepo pr = PackageList.Instance().FindPackageAndRepository(def);
                result += pr.Package.PackageName + " , " + pr.Package.PackageVersion + "\n";
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("Upfile.lock", false))
            {
                file.WriteLine(result);
            }

            return solvedDependencies;
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

        private void VisualizeDependencies(DependencyDefinition[] dependencies)
        {
            string result = "";
            foreach(DependencyDefinition def in dependencies)
            {
                result += RecursivelyListDependencies(def, "");
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("Uptree.txt", false))
            {
                file.WriteLine(result);
            }
        }

        private string RecursivelyListDependencies(DependencyDefinition def, string indent = "")
        {
            string result = indent + def.Name + " " + def.Requirement + " " + def.Version + "\n";
            PackageRepo pr = PackageList.Instance().FindPackageAndRepository(def);
            if(pr.Package != null && pr.Package.Dependencies != null)
            {
                foreach (DependencyDefinition packageDefinition in pr.Package.Dependencies)
                    result += RecursivelyListDependencies(packageDefinition, indent + "   ");
            }
            return result;
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
                    SkipInstall = current.skips,
                    OverrideDestination = current.overrides
                };
            }

            return result;
        }
    }
}
