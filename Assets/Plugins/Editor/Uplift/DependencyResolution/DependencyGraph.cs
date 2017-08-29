using System.Collections.Generic;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    class DependencyGraph
    {
        public List<DependencyNode> nodes;

        public DependencyGraph() {
            nodes = new List<DependencyNode>();
        }
        public DependencyGraph(List<DependencyNode> nodes)
        {
            this.nodes = nodes;
        }

        public DependencyNode FindByName(string name)
        {
            foreach (DependencyNode node in nodes)
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
            nodes.Add(node);
        }

        public void AddDependency(DependencyNode parent, DependencyNode child)
        {
            parent.Dependencies.Add(child);
        }
    }
}
