// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2019-present WeWantToKnow AS
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


using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Linq;


namespace Uplift.DependencyResolution
{

	public class State
	{
		public LinkedList<DependencyDefinition> requirements;
		public DependencyGraph activated;
		public List<PossibilitySet> possibilities;
		public int depth;
		public List<Conflict> conflicts;

		public State(LinkedList<DependencyDefinition> requirements,
					DependencyGraph activated,
					List<PossibilitySet> possibilities,
					int depth,
					List<Conflict> conflicts)
		{
			this.requirements = requirements;
			this.activated = activated;
			this.possibilities = possibilities;
			this.depth = depth;
			this.conflicts = conflicts;
		}

		override public string ToString()
		{
			StringBuilder stringBuffer = new StringBuilder();

			//Requirements : (name version) (name version) (name version)
			stringBuffer.Append("Requirements : ");
			foreach (DependencyDefinition dep in requirements)
			{
				stringBuffer.Append("(" + dep.Name + " " + dep.Version + ") ");
			}

			stringBuffer.AppendLine();
			//Conflicts : [name]
			stringBuffer.Append("Conflicts : ");
			if (conflicts.Count == 0)
			{
				stringBuffer.Append("None");
			}
			else
			{
				foreach (var item in conflicts)
				{
					stringBuffer.Append("[" + item.ToString() + "]");
				}
			}

			stringBuffer.AppendLine();
			/*
				Possibilities : 
					- name : [] [] []
					...
			 */
			stringBuffer.Append("Possibilities : ");
			foreach (PossibilitySet possibilitySet in possibilities)
			{
				stringBuffer.Append("\n    - " + possibilitySet.name + " : ");
				foreach (PackageRepo pkg in possibilitySet.packages)
				{
					stringBuffer.Append("[" + pkg.Package.PackageVersion + "]" + " ");
				}
			}
			stringBuffer.Append("\n");
			return stringBuffer.ToString();
		}
	}

	class DependencyState : State
	{
		public DependencyState(LinkedList<DependencyDefinition> requirements,
								DependencyGraph activated,
								List<PossibilitySet> possibilities,
								int depth,
								List<Conflict> conflicts) :
								base(requirements, activated, possibilities, depth, conflicts)
		{ }

		public List<PackageRepo> GetResolution()
		{
			Debug.Log("getting resolution");
			List<PackageRepo> repoList = new List<PackageRepo>();

			List<DependencyNode> nodesToExplore = activated.nodeList.FindAll(node => !node.isChildNode);
			nodesToExplore = nodesToExplore.Concat(nodesToExplore.SelectMany(node => node.GetChildNodesList()))
											.Distinct()
											.ToList();

			foreach (DependencyNode node in nodesToExplore)
			{
				repoList.Add(node.GetResolutionPackage());
			}
			return repoList;
		}

		public PossibilityState PopPossibilityState()
		{
			Debug.Log("Poping possibility State");
			PossibilityState possibilityState = null;
			if (requirements.Count > 0)
			{
				DependencyDefinition currentRequirement = requirements.First();
				requirements.RemoveFirst();
				//TODO check shallow copy 
				possibilityState = new PossibilityState(requirements,   //Should be shallow copy
														activated,
														possibilities,
														depth + 1,
														conflicts,       //Should be shallow copy
														this,
														currentRequirement);

				Debug.Log(possibilityState);
			}
			return possibilityState;
		}
	}

	class PossibilityState : State
	{
		DependencyState parent;
		public List<PossibilitySet> matchingPossibilitySet = new List<PossibilitySet>();
		public DependencyDefinition currentRequirement;
		List<Conflict> unusedUnwinds = new List<Conflict>();

		public PossibilityState(LinkedList<DependencyDefinition> requirements,
								DependencyGraph activated,
								List<PossibilitySet> possibilities,
								int depth,
								List<Conflict> conflicts,
								DependencyState parent,
								DependencyDefinition currentRequirement
								) :
								base(requirements, activated, possibilities, depth, conflicts)
		{
			this.parent = parent;
			this.currentRequirement = currentRequirement;
			this.matchingPossibilitySet = PossibilitySet.GetMatchingPossibilities(possibilities, currentRequirement);
		}

		override public string ToString()
		{
			StringBuilder stringBuffer = new StringBuilder();
			//Current Requirement : name version
			stringBuffer.AppendLine("Current Requirement : " + "(" + currentRequirement.Name + " " + currentRequirement.Version + ")");
			stringBuffer.AppendLine(base.ToString());

			return stringBuffer.ToString();
		}

		public DependencyState SolveState()
		{
			Debug.Log("Starts to solve possibility state");
			Debug.Log("Possibilities in current state : ");
			foreach (PossibilitySet possibilitySet in matchingPossibilitySet)
			{
				Debug.Log(possibilitySet.name + " | nb of possibilities : " + possibilitySet.packages.Count);
			}

			if (!activated.Contains(currentRequirement.Name))
			{
				Debug.Log("Creates new node and pushes it in activated graph");
				activated.AddNode(new DependencyNode(currentRequirement));
			}

			DependencyNode correspondingNode = activated.FindByName(currentRequirement.Name);
			DependencyState newState = null;
			if (matchingPossibilitySet.Count == 0)
			{
				Debug.Log("- Conflict detected, need to rewind : No matching possibility Set available");
				GenerateConflict(currentRequirement, activated);
			}
			else
			{
				InjectPossibilitiesInDependencyGraph(matchingPossibilitySet, correspondingNode);
				Debug.Log("Poping new dependency state");
				newState = new DependencyState(requirements, activated, possibilities, depth + 1, conflicts);
			}
			return newState;
		}

		//TODO Remove ?
		private List<PossibilitySet> FindMatchingPossibilities(List<PossibilitySet> availablePossibilities, IVersionRequirement requirement)
		{
			foreach (PossibilitySet possibilitySet in availablePossibilities)
			{
				foreach (PackageRepo pkg in possibilitySet.packages)
				{
					if (!requirement.IsMetBy(pkg.Package.PackageVersion))
					{
						Debug.Log("Version " + pkg.Package.PackageVersion + " does not match requirement " + requirement.ToString());
						possibilitySet.packages.Remove(pkg);
						Debug.Log("Version deleted from matching version");
					}
					else
					{
						Debug.Log("Version " + pkg.Package.PackageVersion + " matches requirement " + requirement);
					}
				}
			}
			return availablePossibilities;
		}

		private void InjectPossibilitiesInDependencyGraph(List<PossibilitySet> matchingPossibilities, DependencyNode correspondingNode)
		{
			correspondingNode.matchingPossibilities = correspondingNode.matchingPossibilities
													.Concat(matchingPossibilities)
													.Distinct()
													.ToList();

			correspondingNode.UpdateSelectedPossibilitySet();

			if (correspondingNode.selectedPossibilitySet == null)
			{
				Debug.Log("Selected possibility set is null ! Conflict raised");
				GenerateConflict(currentRequirement, activated);
			}

			DependencyDefinition[] dependenciesToAdd = correspondingNode.selectedPossibilitySet.GetDependencies();
			correspondingNode.AddDependencies(dependenciesToAdd, activated, correspondingNode.selectedPossibilitySet.name);

			if (correspondingNode.conflictingRequirementOnNode != null)
			{
				Debug.Log("Conflict detected : incompatible requirements");
				GenerateConflict(correspondingNode.conflictingRequirementOnNode, activated);
			}

			if (dependenciesToAdd != null)
			{
				foreach (DependencyDefinition dd in dependenciesToAdd)
				{
					Debug.Log("Adding " + dd.Name + " to requirements");
					DependencyDefinition existingDependency = requirements.FirstOrDefault(tmp => tmp.Name == dd.Name);

					if (existingDependency != null)
					{
						IVersionRequirement existingRequirement = existingDependency.Requirement;
						try
						{
							existingRequirement = existingRequirement.RestrictTo(dd.Requirement);
							DependencyDefinition updatedDependency = new DependencyDefinition();
							updatedDependency.Name = dd.Name;
							updatedDependency.Version = existingRequirement.ToString();
							updatedDependency.Repository = dd.Repository;
							updatedDependency.SkipInstall = dd.SkipInstall;
							updatedDependency.OverrideDestination = dd.OverrideDestination;
							requirements.AddLast(updatedDependency);
							requirements.Remove(existingDependency);
						}
						catch (IncompatibleRequirementException e)
						{
							Debug.Log("Incompatible requirements : Cannot merge them as one");
							requirements.AddLast(dd);
						}
					}
					else
					{
						requirements.AddLast(dd);
					}
				}
			}
		}

		public void GenerateConflict(DependencyDefinition requirement, DependencyGraph activated)
		{
			Debug.Log("Generating conflict for " + requirement.Name);
			Conflict newConflict = new Conflict(currentRequirement, activated);
			conflicts.Add(newConflict);
			Debug.Log(newConflict);
		}
	}
}