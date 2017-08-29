using System.Collections.Generic;

namespace Uplift.DependencyResolution
{
    class TarjanCycleDetector
    {
        private List<List<DependencyNode>> stronglyConnectedComponents;
        private Stack<DependencyNode> stack;
        private int index;

        public TarjanCycleDetector()
        {
            stronglyConnectedComponents = new List<List<DependencyNode>>();
        }

        public List<List<DependencyNode>> DetectCycles(DependencyGraph graph)
        {
            stack = new Stack<DependencyNode>();
            index = 0;

            foreach(DependencyNode node in graph.nodeList)
            {
                if (node.Index < 0)
                {
                    StrongConnect(node);
                }
            }

            return stronglyConnectedComponents;
        }

        internal void StrongConnect(DependencyNode node)
        {
            node.Index = index;
            node.Lowlink = index;
            index++;

            stack.Push(node);

            foreach(DependencyNode children in node.Dependencies)
            {
                if (children.Index < 0)
                {
                    // Not visited yet
                    StrongConnect(children);
                    node.Lowlink = System.Math.Min(node.Lowlink, children.Lowlink);
                }
                else if(stack.Contains(children)) {
                    // Already in stack
                    // Belongs to the same strongly connected component
                    node.Lowlink = System.Math.Min(node.Lowlink, children.Index);
                }
            }

            if(node.Index == node.Lowlink)
            {
                List<DependencyNode> stronglyConnectedComponent = new List<DependencyNode>();

                DependencyNode poped;
                do
                {
                    poped = stack.Pop();
                    stronglyConnectedComponent.Add(poped);
                } while (poped != node);

                stronglyConnectedComponents.Add(stronglyConnectedComponent);
            }
        }
    }
}
