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
		public string name;
		public Stack<DependencyDefinition> requirements;
		public DependencyGraph activated;
		public List<PossibilitySet> possibilities;
		public int depth;
		public List<Conflict> conflicts;

		public State(string name,
					Stack<DependencyDefinition> requirements,
					DependencyGraph activated,
					List<PossibilitySet> possibilities,
					int depth,
					List<Conflict> conflicts)
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
								List<Conflict> conflicts) :
								base(name, requirements, activated, possibilities, depth, conflicts)
		{ }


		public List<Upset> GetResolution()
		{
			Debug.Log("getting resolution");
			List<Upset> upsetList = new List<Upset>();

			Debug.Log("Dependency graph : ");
			Debug.Log(activated.ToString());
			//FIXME : Does nodeList also include children ? (answer = no)
			//TODO : fix this ! explore add child in node list
			foreach (DependencyNode node in activated.nodeList)
			{
				//FIXME : Better euristic to choose possibilities (most recent ?)
				PossibilitySet chosenPossibililtySet = node.selectedPossibilitySet;
				if (chosenPossibililtySet.packages.Count > 0)
				{
					upsetList.Add(chosenPossibililtySet.packages[0]);
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

				Debug.Log(possibilityState);
			}
			return possibilityState;
		}
	}

	class PossibilityState : State
	{
		DependencyState parent;
		public List<PossibilitySet> matchingPossibilitySet = new List<PossibilitySet>();
		DependencyDefinition currentRequirement;
		List<Conflict> unusedUnwinds = new List<Conflict>(); //FIXME Unwind = conflict + stateToUnwind

		public PossibilityState(string name,
								Stack<DependencyDefinition> requirements,
								DependencyGraph activated,
								List<PossibilitySet> possibilities,
								int depth,
								List<Conflict> conflicts,
								DependencyState parent,
								DependencyDefinition currentRequirement
								) :
								base(name, requirements, activated, possibilities, depth, conflicts)
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

			DependencyNode correspondingNode = activated.FindByName(currentRequirement.Name); //FIXME check if dependency graph look into childs to find results
			DependencyState newState = null;
			if (matchingPossibilitySet.Count == 0) // TODO IF every matchingPossibility.packages are empty
			{
				Debug.Log("- Conflict detected, need to rewind !");
				GenerateConflict(currentRequirement, activated);
			}
			else
			{
				InjectPossibilitiesInDependencyGraph(matchingPossibilitySet, correspondingNode);
				Debug.Log("Poping new dependency state");
				//TODO change Name
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
						Debug.Log("Version " + pkg.PackageVersion + " does not match requirement " + requirement.ToString());
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
			//TODO remove doubles
			correspondingNode.matchingPossibilities = correspondingNode.matchingPossibilities
													.Concat(matchingPossibilities)
													.Distinct()
													.ToList();

			correspondingNode.UpdateSelectedPossibilitySet();

			if (correspondingNode.selectedPossibilitySet == null)
			{
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
					requirements.Push(dd);
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

		private List<string> FindConflictingDependency(Conflict conflictRaised)
		{
			//FIXME conflict might be due to several requirement
			List<string> conflictingDependencies = new List<string>();
			DependencyNode correspondingNode = conflictRaised.activated.FindByName(conflictRaised.requirement.Name);

			DependencyNode copiedNode = new DependencyNode();
			copiedNode.restrictions = correspondingNode.restrictions;
			if (copiedNode.restrictions.Keys != null && copiedNode.restrictions.Keys.Count > 0)
			{
				List<String> restrictors = new List<string>(copiedNode.restrictions.Keys);

				foreach (string key in restrictors)
				{
					//Initial requirements cannot be changed
					if (key == "initial")
					{
						continue;
					}
					//TODO create a function to do this
					IVersionRequirement tmpVersion = copiedNode.restrictions[key];
					copiedNode.restrictions[key] = new NoRequirement();
					try
					{
						IVersionRequirement newRequirementForNode = copiedNode.ComputeRequirement();
						//IVersionRequirement versionRequirementTobeChecked = conflictRaised.requirement.Requirement.RestrictTo(newRequirementForNode);

						foreach (Upset package in correspondingNode.selectedPossibilitySet.packages)
						{
							if (newRequirementForNode.IsMetBy(package.PackageVersion))
							{
								Debug.Log("When removing " + key + " requirements on " + conflictRaised.requirement.Name + " no conflict remains.");
								conflictingDependencies.Add(key);
								break;
							}
						}
					}
					catch (Exception e)
					{
						Debug.LogError(e.ToString());
					}
					finally
					{
						copiedNode.restrictions[key] = tmpVersion;
					}
				}
			}

			return conflictingDependencies;
		}

		//TODO Move rewind logic in a separated class
		public Stack<State> UnwindForConflict(Stack<State> stack) //return stack ?
		{
			Conflict conflict = conflicts.ToArray()[0];
			//In dependencyState ?
			Debug.Log("unwind for conflict started");
			// Find requirements causing conflict
			Debug.Log("Get list of conflicting requirements");

			//TODO if null raise error ?
			List<string> conflictingRequirements = FindConflictingDependency(conflict);

			Stack<State> stackCopy = new Stack<State>(stack);
			List<State> possibleRewinds = new List<State>();

			Debug.Log("Go through possible rewinds");
			foreach (string nodeName in conflictingRequirements)
			{
				possibleRewinds.AddRange(GetPossibleRewinds(stackCopy, nodeName));

				//TODO look for unwinds in previous unused unwinds
				// --> Check unwind that were not executed (in list of unused unwinds)
				// * Keep only those that are smaller than those in the array[]
				// * Check if unwind has the chance of prevent encounter current conflict
				//		* the unwind must have been rejected an unwind leading to
				//		* one of the state in the current conflict reqt tree

				// --> Launch unwind
				// * If unwinds are found take the smallest one
				//		* filter destination state's possibilities to prevent conflict
				// * else
				//		* raise VersionConflict error
				// -->  Update list of unused unwinds			
			}

			PossibilityState rewindStateCandidate = FindStateWithGreaterIndex(possibleRewinds);
			Debug.Log("Rewind candidate is found.");
			Debug.Log(rewindStateCandidate.currentRequirement.Name + " has other possibility sets matching requirements");
			//FIXME Remove it from possible rewind and put the remaining one in unusedUnwinds

			int depthToRewind = rewindStateCandidate.depth;
			conflicts.Remove(conflict);
			RewindToState(stack, depthToRewind);

			return stack;
		}

		private Stack<State> RewindToState(Stack<State> stack, int depthToRewind)
		{
			Debug.Log("rewinding to state at depth " + depthToRewind);
			while (stack.Peek().depth > depthToRewind)
			{
				stack.Pop();
			}
			//Use other possibilitySet than selected possibilitySet
			PossibilityState stateToRewind = (PossibilityState)stack.Pop();
			DependencyNode correspondingNode = stateToRewind.activated.FindByName(stateToRewind.currentRequirement.Name);

			Debug.Log("removing previously selected possibility set");
			PossibilitySet selectedPossibilitySet = correspondingNode.selectedPossibilitySet;
			//FIXME : super dangerous to have duplicate matching possibilitySet...
			stateToRewind.matchingPossibilitySet.Remove(selectedPossibilitySet);

			Debug.Log("Remove it from matching possibility set");
			correspondingNode.matchingPossibilities = stateToRewind.matchingPossibilitySet;

			Debug.Log("Push edited possibility state to stack");
			stack.Push(stateToRewind);
			return stack;
		}

		private List<State> GetPossibleRewinds(Stack<State> stack, string dependencyName)
		{
			State currentState = stack.Pop();

			List<State> matchingPossibilityStates = new List<State>();
			List<State> listOfStates = new List<State>(stack.ToArray());
			List<State> listOfPossibleRewinds = new List<State>();

			//_Check parent of dependencyName for alternatives, check parent alternative, check gramps alternatives
			//Do it recursively until top
			//Add possibilityState of dependencyName
			if (stack.Count >= 1)
			{
				//TODO Use linq !!!
				listOfPossibleRewinds.Add(listOfStates.Find(state => state.GetType() == typeof(PossibilityState)
																&& ((PossibilityState)state).currentRequirement.Name == dependencyName
																&& state.activated.FindByName(dependencyName).matchingPossibilities.Count > 1));

				foreach (DependencyNode node in currentState.activated.FindNodesWithGivenDependency(dependencyName))
				{
					//TODO linq
					matchingPossibilityStates = listOfStates.FindAll(state => state.GetType() == typeof(PossibilityState)
																		&& ((PossibilityState)state).currentRequirement.Name == node.Name
																		&& state.activated.FindByName(node.Name).matchingPossibilities.Count > 1);
					listOfPossibleRewinds.AddRange(matchingPossibilityStates);
				}
			}
			return listOfPossibleRewinds;

			//Explore parent and check if they have alternative possibility sets with differents dependencies
			/*
			if (currentState.GetType() == typeof(PossibilityState)
			&& (currentState.possibilities.Count > 1))
			{
				// ;
				// Get nodes.name and look for corresponding possibility states
				List<PossibilityState> result = GetPossibleRewinds(stack, currentState.name);
				result.Add((PossibilityState)currentState);
				return result; //FIXME just add depth of state is better ?
			}
			else
			{
				return GetPossibleRewinds(stack, dependencyName);
			}
			*/
		}
		private PossibilityState FindStateWithGreaterIndex(List<State> possibleRewinds)
		{
			PossibilityState rewindCandidate = null;
			foreach (PossibilityState state in possibleRewinds)
			{
				if (rewindCandidate == null || rewindCandidate.depth < state.depth)
				{
					rewindCandidate = state;
				}
			}
			return rewindCandidate;
		}
	}
}