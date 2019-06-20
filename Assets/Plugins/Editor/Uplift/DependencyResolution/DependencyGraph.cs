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

		//TODO add a tag method

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("=== Dependency graph : ===");
			foreach (DependencyNode node in nodeList)
			{
				sb.AppendLine(" * " + node.Name + " : " + node.Requirement.ToString());
				foreach (DependencyNode depNode in node.Dependencies)
				{
					sb.Append(depNode.Name + " | ");
				}
			}
			return sb.ToString();
		}
	}
}