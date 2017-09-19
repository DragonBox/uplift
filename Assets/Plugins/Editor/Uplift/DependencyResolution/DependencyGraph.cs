using System.Collections.Generic;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    class DependencyGraph
    {
        public List<DependencyNode> nodeList;

        public DependencyGraph() {
            nodeList = new List<DependencyNode>();
        }
        public DependencyGraph(List<DependencyNode> nodes)
        {
            this.nodeList = nodes;
        }

        public DependencyNode FindByName(string name)
        {
            foreach (DependencyNode node in nodeList)
                if (node.Name == name)
                    return node;

            return null;
        }

        public bool Contains(string name)
        {
            return FindByName(name) != null;
        }

        public void AddNode(DependencyNode node)
        {
            nodeList.Add(node);
        }

        public void AddDependency(DependencyNode parent, DependencyNode child)
        {
            parent.Dependencies.Add(child);
        }

        public void LoadDependencies(DependencyDefinition dependency, PackageList packageList, DependencyHelper.ConflictChecker checkConflict, out DependencyNode node)
        {
            node = new DependencyNode(dependency);

            if (Contains(dependency.Name))
            {
                DependencyNode existing = FindByName(dependency.Name);
                checkConflict(ref existing, node);
            }
            else
            {
                nodeList.Add(node);

                Upset package = packageList.GetLatestPackage(dependency.Name).Package;
                if (package == null)
                {
                    throw new MissingDependencyException(string.Format(" depends on {0} but it is not present in any of your specified repository", dependency.Name));
                }

                if (package.Dependencies != null)
                {
                    DependencyNode child;
                    foreach (DependencyDefinition packageDependency in package.Dependencies)
                    {
                        child = null;
                        try
                        {
                            LoadDependencies(packageDependency, packageList, checkConflict, out child);
                            AddDependency(node, child);
                        }
                        catch (MissingDependencyException e)
                        {
                            throw new MissingDependencyException(dependency.Name + e.Message);
                        }
                    }
                }
            }
        }
    }
}
