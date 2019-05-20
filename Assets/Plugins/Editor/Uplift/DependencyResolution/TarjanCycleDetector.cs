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

			foreach (DependencyNode node in graph.nodeList)
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

			foreach (DependencyNode children in node.Dependencies)
			{
				if (children.Index < 0)
				{
					// Not visited yet
					StrongConnect(children);
					node.Lowlink = System.Math.Min(node.Lowlink, children.Lowlink);
				}
				else if (stack.Contains(children))
				{
					// Already in stack
					// Belongs to the same strongly connected component
					node.Lowlink = System.Math.Min(node.Lowlink, children.Index);
				}
			}

			if (node.Index == node.Lowlink)
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