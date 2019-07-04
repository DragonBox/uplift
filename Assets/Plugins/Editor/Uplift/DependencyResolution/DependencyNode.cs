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

namespace Uplift.DependencyResolution
{
	public class DependencyNode
	{
		protected IVersionRequirement requirement;
		protected string repository;
		protected string name;
		protected int index;
		protected int lowlink;

		public List<DependencyNode> dependencies;
		public PossibilitySet selectedPossibilitySet;
		public List<PossibilitySet> matchingPossibilities = new List<PossibilitySet>();
		//public List<PossibilitySet> possibilities = new List<PossibilitySet>();
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
			//TODO write a test for this
			Debug.Log("Updating " + this.name + " requirement " + requirement);

			Debug.Log(restrictor + " adds new restriction on " + name);

			restrictions[restrictor] = newRequirements.Requirement;

			try
			{
				this.requirement = requirement.RestrictTo(restrictions[restrictor]);
			}
			catch (IncompatibleRequirementException e)
			{
				conflictingRequirementOnNode = newRequirements;
				return;
			}

			Debug.Log("Requirement updated");
			Debug.Log("New requirement is : " + requirement);
			//FIXME Maybe add test if null value in foreach loops...

			Debug.Log("Updating node possibilities according to new requirements");
			foreach (PossibilitySet pos in matchingPossibilities)
			{
				foreach (Upset package in pos.packages)
				{
					if (!requirement.IsMetBy(package.PackageVersion))
					{
						Debug.Log("Package " + package.PackageName + " " + package.PackageVersion + " is no longer met by new requirement.");
						pos.packages.Remove(package);
					}
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
			if (matchingPossibilities.Count > 0)
			{
				//FIXME change the selecting method to take the most recent possibilitySet
				//FIXME Ensure that possibilitySet is complete regardless of the requirement
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
						if (!activated.Contains(dd.Name))//FIXME does contains method explore childnodes ?
						{
							Debug.Log("Node added for " + dd.Name);
							DependencyNode childNode = new DependencyNode(dd);
							Debug.Log(restrictor + " adds first restriction on new dep node " + dd.Name);
							childNode.restrictions[restrictor] = dd.Requirement;
							activated.AddDependency(this, childNode); //activated.AddNode(childNode);
						}
						else
						{
							Debug.Log("Node for " + dd.Name + " already in tree.");
							activated.FindByName(dd.Name).UpdateNodeRequirements(dd, restrictor);
						}
					}
				}
			}
		}
	}
}