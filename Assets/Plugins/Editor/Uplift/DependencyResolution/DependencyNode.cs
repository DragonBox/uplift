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
using Uplift.Schemas;
using UnityEngine;
using System;
using Uplift.Packages;
using System.Linq;

namespace Uplift.DependencyResolution
{
	public class DependencyNode
	{
		protected string repository;
		protected string name;
		protected int index;
		protected int lowlink;

		public Boolean isChildNode = false;
		public List<DependencyNode> dependencies;
		public PossibilitySet selectedPossibilitySet;
		public List<PossibilitySet> matchingPossibilities = new List<PossibilitySet>();
		public IVersionRequirement requirement;

		public Dictionary<string, IVersionRequirement> restrictions = new Dictionary<string, IVersionRequirement>();

		public SkipInstallSpec[] skips;
		public OverrideDestinationSpec[] overrides;

		public DependencyDefinition conflictingRequirementOnNode = null;

		public DependencyNode() { }
		public DependencyNode(DependencyDefinition definition) : this(
			definition.Name,
			definition.Version,
			definition.Repository,
			definition.SkipInstall,
			definition.OverrideDestination,
			null
		)
		{ }
		public DependencyNode(string name, string version, string repository) : this(name, version, repository, null, null, null) { }
		public DependencyNode(string name, string version, string repository, SkipInstallSpec[] skips, OverrideDestinationSpec[] overrides, List<DependencyNode> dependencies)
		{
			this.name = name;
			this.requirement = VersionParser.ParseRequirement(version);
			this.repository = repository;
			this.dependencies = dependencies;
			this.skips = skips;
			this.overrides = overrides;
			index = -1;
			lowlink = -1;
		}

		public IVersionRequirement Requirement
		{
			get
			{
				return requirement;
			}
			set
			{
				requirement = value;
			}
		}

		public string Repository
		{
			get
			{
				return repository;
			}
			set
			{
				repository = value;
			}
		}
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public List<DependencyNode> Dependencies
		{
			get
			{
				if (dependencies == null)
					dependencies = new List<DependencyNode>();

				return dependencies;
			}
			set
			{
				dependencies = value;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		public int Lowlink
		{
			get
			{
				return lowlink;
			}
			set
			{
				lowlink = value;
			}
		}

		private void UpdateNodeRequirements(DependencyDefinition newRequirements, string restrictor)
		{
			Debug.Log("Updating " + this.name + " requirement " + requirement);
			Debug.Log(restrictor + " adds new restriction on " + name);

			restrictions[restrictor] = newRequirements.Requirement;

			try
			{
				this.requirement = requirement.RestrictTo(restrictions[restrictor]);
			}
			catch (IncompatibleRequirementException e)
			{
				Debug.Log("Incompatible requirement on node");
				conflictingRequirementOnNode = newRequirements;
				return;
			}

			Debug.Log("Requirement updated");
			Debug.Log("New requirement is : " + requirement);

			Debug.Log("Updating node possibilities according to new requirements");
			Dictionary<PossibilitySet, List<PackageRepo>> packagesToRemove = new Dictionary<PossibilitySet, List<PackageRepo>>();
			foreach (PossibilitySet pos in matchingPossibilities)
			{
				foreach (PackageRepo package in pos.packages)
				{
					Debug.Log("Checking if " + package.Package.PackageName + " version " + package.Package.PackageVersion + " matches " + requirement.ToString());
					if (!requirement.IsMetBy(package.Package.PackageVersion))
					{
						Debug.Log("Package " + package.Package.PackageName + " " + package.Package.PackageVersion + " is no longer met by new requirement.");
						Debug.Log("Removing a package !");
						if (!packagesToRemove.ContainsKey(pos)
						  || packagesToRemove[pos] == null
						  || packagesToRemove[pos].Count < 1)
						{
							packagesToRemove[pos] = new List<PackageRepo>();
							packagesToRemove[pos].Add(package);
						}
					}
				}
			}

			foreach (PossibilitySet pos in packagesToRemove.Keys)
			{
				foreach (PackageRepo pkg in packagesToRemove[pos])
				{
					matchingPossibilities.Find(posSet => pos == posSet).packages.Remove(pkg);
				}
			}
		}

		public IVersionRequirement ComputeRequirement()
		{
			requirement = new NoRequirement();
			foreach (IVersionRequirement versionRequirement in restrictions.Values)
			{
				requirement = requirement.RestrictTo(versionRequirement);
			}
			return requirement;
		}

		public void UpdateSelectedPossibilitySet()
		{
			if (restrictions.ContainsKey("legacy"))
			{
				String legacyVersion = ((MinimalVersionRequirement)restrictions["legacy"]).minimal.ToString();
				foreach (PossibilitySet posSet in matchingPossibilities)
				{
					if (posSet.packages.Exists(repo => repo.Package.PackageVersion == legacyVersion))
					{
						selectedPossibilitySet = posSet;
						return;
					}
				}
			}

			if (matchingPossibilities.Count > 0)
			{
				selectedPossibilitySet = PossibilitySet.GetMostRecentPossibilitySetFromList(matchingPossibilities);
				Debug.Log("SelectedPossibility is set to " + selectedPossibilitySet);
			}
			else
			{
				Debug.Log("selectedPossibility is set to null because matchingPossibility is empty");
				selectedPossibilitySet = null;
			}
		}

		public void AddDependencies(DependencyDefinition[] requirements, DependencyGraph activated, string restrictor)
		{
			if (requirements != null && requirements.Length > 0)
			{
				foreach (DependencyDefinition dd in requirements)
				{
					Debug.Log("Depends on " + dd.Name);
					if (dd != null)
					{
						if (!activated.Contains(dd.Name))
						{
							Debug.Log("Node added for " + dd.Name);
							DependencyNode childNode = new DependencyNode(dd);
							Debug.Log(restrictor + " adds first restriction on new dep node " + dd.Name);
							childNode.restrictions[restrictor] = dd.Requirement;
							activated.AddDependency(this, childNode);
						}
						else
						{
							DependencyNode parentNode = activated.FindByName(restrictor);
							DependencyNode childNode = activated.FindByName(dd.Name);

							Debug.Log("Node for " + dd.Name + " already in tree.");
							if (!parentNode.Dependencies.Contains(childNode))
							{
								Debug.Log("Adding " + restrictor + "as parent for " + dd.Name + ".");
								parentNode.dependencies.Add(childNode);
							}
							activated.FindByName(dd.Name).UpdateNodeRequirements(dd, restrictor);
						}
					}
				}
			}
		}

		public List<DependencyNode> GetChildNodesList()
		{
			List<DependencyNode> childNodesList = new List<DependencyNode>();
			List<DependencyNode> visitedNodes = new List<DependencyNode>();
			if (dependencies != null)
			{
				childNodesList.AddRange(dependencies);
				visitedNodes.AddRange(dependencies);
				foreach (DependencyNode childNode in dependencies)
				{
					childNodesList.AddRange(childNode.GetChildNodesListReq(visitedNodes));
				}
			}
			return childNodesList;
		}

		private List<DependencyNode> GetChildNodesListReq(List<DependencyNode> visitedNodes)
		{
			List<DependencyNode> childNodesList = visitedNodes;
			if (dependencies != null)
			{
				foreach (DependencyNode childNode in dependencies)
				{
					if (!visitedNodes.Contains(childNode))
					{
						childNodesList.Add(childNode);
						childNodesList.AddRange(childNode.GetChildNodesListReq(childNodesList));
					}
				}
			}
			return childNodesList.Distinct().ToList();
		}

		public void RemoveRestriction(string restrictor, PackageList pkgList)
		{
			restrictions.Remove(restrictor);
			List<PossibilitySet> possibilitySet = PossibilitySet.GetPossibilitySetsForGivenPackage(this.Name, pkgList);
			try
			{
				matchingPossibilities = PossibilitySet.GetMatchingPossibilities(possibilitySet, name, ComputeRequirement());
			}
			catch (IncompatibleRequirementException e)
			{
				Debug.Log("Matching possibility is set to null due to conflict on node");
				matchingPossibilities = new List<PossibilitySet>();
			}
		}

		public PackageRepo GetResolutionPackage()
		{
			if (selectedPossibilitySet == null)
			{
				UpdateSelectedPossibilitySet();
			}

			if (restrictions.ContainsKey("legacy"))
			{
				String legacyVersion = ((MinimalVersionRequirement)restrictions["legacy"]).minimal.ToString();

				foreach (PackageRepo pr in selectedPossibilitySet.packages)
				{
					if (pr.Package.PackageVersion == legacyVersion)
					{
						return pr;
					}
				}
			}
			return selectedPossibilitySet.GetMostRecentPackage();
		}
	}
}