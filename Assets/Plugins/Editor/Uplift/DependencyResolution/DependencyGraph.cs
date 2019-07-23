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
using System.Text;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
	public class DependencyGraph
	{
		public List<DependencyNode> nodeList;

		public DependencyGraph()
		{
			nodeList = new List<DependencyNode>();
		}
		public DependencyGraph(List<DependencyNode> nodes)
		{
			this.nodeList = nodes;
		}

		public DependencyGraph(PackageRepo[] packages)
		{
			//TODO test
			nodeList = new List<DependencyNode>();
			if (packages != null && packages.Length > 0)
			{
				foreach (PackageRepo pr in packages)
				{
					DependencyNode node = new DependencyNode(pr.Package.PackageName, pr.Package.PackageVersion, pr.Repository.ToString());
					node.requirement = new MinimalVersionRequirement(pr.Package.PackageVersion);
					node.restrictions["legacy"] = new MinimalVersionRequirement(pr.Package.PackageVersion);
					//Dependencies to add ?
					this.AddNode(node);
				}
			}
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
			if (!nodeList.Contains(node))
			{
				nodeList.Add(node);
			}
		}

		public void AddDependency(DependencyNode parent, DependencyNode child)
		{
			parent.Dependencies.Add(child);
			AddNode(child);
			child.isChildNode = true;
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

				Upset package = packageList.FindPackageAndRepository(dependency).Package;
				if (package == null)
				{
					throw new MissingDependencyException(string.Format(" depends on {0} ({1}) but it is not present in any of your specified repository", dependency.Name, dependency.Requirement));
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


		//Check if node has a childnode with the given name in parameter and return his parent
		public List<DependencyNode> FindNodesRelatedToGivenDependency(string name)
		{
			List<DependencyNode> matchingNodes = new List<DependencyNode>();
			matchingNodes.Add(this.FindByName(name));

			foreach (DependencyNode node in nodeList)
			{
				//Check if depends on given dependency name
				if (node.selectedPossibilitySet == null) continue;

				DependencyDefinition[] dependencies = node.selectedPossibilitySet.GetDependencies();
				if (dependencies != null && dependencies.Length > 0)
				{
					foreach (DependencyDefinition dep in dependencies)
					{
						if (dep.Name == name)
						{
							matchingNodes.Add(node);
							matchingNodes.AddRange(FindNodesRelatedToGivenDependency(node.Name));
						}
					}
				}
			}
			return matchingNodes;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("=== Dependency graph : ===");

			List<DependencyNode> visitedNodes = new List<DependencyNode>();
			foreach (DependencyNode node in nodeList.FindAll(node => !node.isChildNode))
			{
				sb.AppendLine("* " + node.Name + " : " + node.Requirement.ToString());
				if (node.dependencies != null && node.dependencies.Count > 0)
				{
					sb.AppendLine(PrintChildNodes(node, 1, visitedNodes));
				}
			}
			return sb.ToString();
		}

		private string PrintChildNodes(DependencyNode parent, int depth, List<DependencyNode> visitedNodes)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DependencyNode childNode in parent.dependencies)
			{
				for (int i = 0; i < depth; i++) sb.Append(" ");

				sb.AppendLine("* " + childNode.Name + " : " + childNode.Requirement.ToString());

				if (childNode.dependencies != null
				&& !visitedNodes.Contains(childNode)
				&& childNode.dependencies.Count > 0)
				{
					visitedNodes.Add(parent);
					sb.AppendLine(PrintChildNodes(childNode, depth + 1, visitedNodes));
				}
			}
			return sb.ToString();
		}
	}
}