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


namespace Uplift.DependencyResolution
{
	public struct PossibilitySet
	{
		public string name;
		public List<Upset> packages;

		public Upset GetMostRecentPackage()
		{
			//Could also use VersionParser.GreaterThan(string a, string b)

			Version currentVersion = null;
			Version mostRecentVersionSofar = null;
			Upset mostRecentPackageSoFar = null;

			if (packages != null && packages.Count > 0)
			{

				foreach (Upset package in packages)
				{
					currentVersion = VersionParser.ParseVersion(package.PackageVersion, false);

					if (mostRecentVersionSofar == null || currentVersion > mostRecentVersionSofar)
					{
						mostRecentVersionSofar = VersionParser.ParseVersion(package.PackageVersion);
						mostRecentPackageSoFar = package;
					}
				}

			}
			return mostRecentPackageSoFar;
		}

		public static PossibilitySet GetMostRecentPossibilitySetFromList(List<PossibilitySet> possibilitySetList)
		{
			PossibilitySet mostRecentPossibility = new PossibilitySet();
			string currentVersion = null;
			string mostRecentVersion = null;
			if (possibilitySetList != null && possibilitySetList.Count > 0)
			{
				foreach (PossibilitySet pos in possibilitySetList)
				{
					if (pos.GetMostRecentPackage() == null)
					{
						continue;
					}

					currentVersion = pos.GetMostRecentPackage().PackageVersion;
					if (mostRecentVersion == null || VersionParser.GreaterThan(currentVersion, mostRecentVersion))
					{
						mostRecentVersion = currentVersion;
						mostRecentPossibility = pos;
					}
				}
			}

			return mostRecentPossibility;
		}

		public DependencyDefinition[] GetDependencies()
		{
			if (packages != null)
			{
				return packages.ToArray()[0].Dependencies;
			}
			else
			{
				return null;
			}
		}
	}

	public class State
	{
		public string name;
		public Stack<DependencyDefinition> requirements;
		public DependencyGraph activated;
		public List<PossibilitySet> possibilities;
		public int depth;
		public Dictionary<string, Conflict> conflicts;

		public State(string name,
					Stack<DependencyDefinition> requirements,
					DependencyGraph activated,
					List<PossibilitySet> possibilities,
					int depth,
					Dictionary<string, Conflict> conflicts)
		{
			this.name = name;
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
				foreach (var item in conflicts.Keys)
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
				foreach (Upset pkg in possibilitySet.packages)
				{
					stringBuffer.Append("[" + pkg.PackageVersion + "]" + " ");
				}
			}
			stringBuffer.Append("\n");
			return stringBuffer.ToString();
		}
	}

	class DependencyState : State
	{
		public DependencyState(string name,
								Stack<DependencyDefinition> requirements,
								DependencyGraph activated,
								List<PossibilitySet> possibilities,
								int depth,
								Dictionary<string, Conflict> conflicts) :
								base(name, requirements, activated, possibilities, depth, conflicts)
		{ }


		public List<Upset> GetResolution()
		{
			Debug.Log("getting resolution");
			List<Upset> upsetList = new List<Upset>();

			Debug.Log("Dependency graph : ");
			Debug.Log(activated.ToString());
			//FIXME : Does nodeList also include children ? (answer = no)
			foreach (DependencyNode node in activated.nodeList)
			{
				Debug.Log("a node is explored");
				if (node.matchingPossibilities.Count > 0)
				{
					Debug.Log("Has a matching possibility");
					//FIXME : Better euristic to choose possibilities (most recent ?)
					PossibilitySet chosenPossibililtySet = node.matchingPossibilities.ToArray()[0];
					if (chosenPossibililtySet.packages.Count > 0)
					{
						Debug.Log("displaying it");
						upsetList.Add(chosenPossibililtySet.packages[0]);
					}
				}
			}
			return upsetList;
		}

		/*
			method for when no requirements
		 */
		public PossibilityState PopPossibilityState()
		{
			Debug.Log("Poping possibility State");
			PossibilityState possibilityState = null;
			if (requirements.Count > 0)
			{
				DependencyDefinition currentRequirement = requirements.Pop();

				//TODO check shallow copy 
				possibilityState = new PossibilityState(name,
														requirements,   //Should be shallow copy
														activated,
														possibilities,
														depth + 1,
														conflicts,       //Should be shallow copy
														this,
														currentRequirement);
				//tag possibilityState.activated as state ?
				Debug.Log(possibilityState);
			}
			return possibilityState;
		}
	}

	class PossibilityState : State
	{
		DependencyState parent;
		DependencyDefinition currentRequirement;
		public PossibilityState(string name,
								Stack<DependencyDefinition> requirements,
								DependencyGraph activated,
								List<PossibilitySet> possibilities,
								int depth,
								Dictionary<string, Conflict> conflicts,
								DependencyState parent,
								DependencyDefinition currentRequirement
								) :
								base(name, requirements, activated, possibilities, depth, conflicts)
		{
			this.parent = parent;
			this.currentRequirement = currentRequirement;
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
			foreach (PossibilitySet possibilitySet in possibilities)
			{
				Debug.Log(possibilitySet.name + " | nb of possibilities : " + possibilitySet.packages.Count);
			}

			List<PossibilitySet> availablePossibilities = possibilities.FindAll(pos => pos.name == currentRequirement.Name);

			if (availablePossibilities.Count == 0)
			{
				Debug.LogError("Conflict detected, need to rewind !");
				GenerateConflict();
			}

			DependencyState newState = null;
			if (!activated.Contains(currentRequirement.Name))
			{
				Debug.Log("Creates new node and pushes it in activated graph");
				activated.AddNode(new DependencyNode(currentRequirement));
			}
			DependencyNode correspondingNode = activated.FindByName(currentRequirement.Name);

			//FIXME LoadDependency
			Debug.Log("- Goes through possibilities to update graph for " + currentRequirement.Name);
			List<PossibilitySet> matchingPossibilities = FindMatchingPossibilities(availablePossibilities, correspondingNode.Requirement);

			if (matchingPossibilities.Count == 0) // TODO IF every matchingPossibility.packages are empty
			{
				Debug.LogError("- Conflict detected, need to rewind !");
				GenerateConflict();
			}
			else
			{
				InjectPossibilitiesInDependencyGraph(matchingPossibilities, correspondingNode);
				Debug.Log("Poping new dependency state");
				newState = new DependencyState(name, requirements, activated, possibilities, depth + 1, conflicts);
			}
			return newState;
		}

		private List<PossibilitySet> FindMatchingPossibilities(List<PossibilitySet> availablePossibilities, IVersionRequirement requirement)
		{
			foreach (PossibilitySet possibilitySet in availablePossibilities)
			{
				foreach (Upset pkg in possibilitySet.packages)
				{
					if (!requirement.IsMetBy(pkg.PackageVersion))
					{
						Debug.Log("Version " + pkg.PackageVersion + " does not match requirement " + requirement);
						possibilitySet.packages.Remove(pkg);
						Debug.Log("Version deleted from matching version");
					}
					else
					{
						Debug.Log("Version " + pkg.PackageVersion + " matches requirement " + requirement);
					}
				}
			}
			return availablePossibilities;
		}

		private void InjectPossibilitiesInDependencyGraph(List<PossibilitySet> matchingPossibilities, DependencyNode correspondingNode)
		{
			PossibilitySet mostRecentPossibility = PossibilitySet.GetMostRecentPossibilitySetFromList(matchingPossibilities);
			DependencyDefinition[] dependenciesToAdd = mostRecentPossibility.GetDependencies();

			//TODO remove doubles || if !contains
			correspondingNode.matchingPossibilities.AddRange(matchingPossibilities);
			correspondingNode.AddDependencies(dependenciesToAdd, activated);

			if (dependenciesToAdd != null)
			{
				foreach (DependencyDefinition dd in dependenciesToAdd)
				{
					Debug.Log("Adding " + dd.Name + " to requirements");
					requirements.Push(dd);
				}
			}
		}

		public void GenerateConflict()
		{
			Conflict newConflict = new Conflict(currentRequirement, possibilities, activated);
			conflicts[this.name] = newConflict;
			Debug.Log(newConflict);
		}

		public void UnwindForConflict()
		{
			/*
			* Rename Possibility in PossibilitySet (packages into possibilities ?)
			
			* Check Conflicts on state
			* Look for a state to rewind
				* Check alternative possibilities among state/parent/parent.parent/...
					* Store possibilities in an array[]
					* Take smallest unwind
				* Check unwind that were not executed (in list of unused unwinds)
					* Keep only those that are smaller than those in the array[]
					* Check if unwind has the chance of prevent encounter current conflict
						* the unwind must have been rejected an unwind leading to
						* one of the state in the current conflict reqt tree
					* If unwinds are found take the smallest one
						* filter destination state's possibilities to prevent conflict
					* else
						* raise VersionConflict error
				* Update list of unused unwinds
			*/
		}
	}
}