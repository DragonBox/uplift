// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

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

            cycleDetector.DetectCycles(dependencyGraph);

            // TODO: Save the current dependency tree so the whole tree doesn't have to be solved entirely later on
            DependencyDefinition[] solvedDependencies = GetDependencyDefinitions(dependencyGraph);

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
