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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
	public class Rewinder
	{
		Stack<State> stack;

		public Rewinder()
		{
			stack = new Stack<State>();
		}

		public Rewinder(Stack<State> stack)
		{
			this.stack = stack;
		}

		public Stack<State> UnwindForConflict(Conflict conflict, PackageList pkgList) //return stack ?
		{
			Debug.Log("Unwind for conflict started");
			// Find requirements causing conflict
			Debug.Log("Get list of conflicting requirements");

			//TODO if null raise error ?
			List<string> conflictingRequirements = conflict.FindConflictingDependency();

			//Copy current stack
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

			PossibilityState rewindStateCandidate = FindStateWithGreatestIndex(possibleRewinds);
			Debug.Log("Rewind candidate is found.");
			//TODO if rewind candidate is null then unable to find viable solution
			Debug.Log(rewindStateCandidate.currentRequirement.Name + " has other possibility sets matching requirements");
			//FIXME Remove it from possible rewind and put the	 remaining one in unusedUnwinds

			int depthToRewind = rewindStateCandidate.depth;
			RewindToState(stack, depthToRewind, pkgList);

			return stack;
		}

		private Stack<State> RewindToState(Stack<State> stack, int depthToRewind, PackageList pkgList)
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
			//FIXME : dangerous to have duplicate matching possibilitySet...
			stateToRewind.matchingPossibilitySet.Remove(selectedPossibilitySet);

			Debug.Log("Remove it from matching possibility set");
			correspondingNode.matchingPossibilities = stateToRewind.matchingPossibilitySet;

			Debug.Log("Clean nodes dependencies");
			foreach (DependencyNode node in correspondingNode.dependencies)
			{
				node.RemoveRestriction(correspondingNode.Name, pkgList);
			}
			correspondingNode.dependencies = new List<DependencyNode>();

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
				List<DependencyNode> relatedNodes = currentState.activated.FindNodesRelatedToGivenDependency(dependencyName);
				foreach (DependencyNode node in relatedNodes)
				{
					matchingPossibilityStates = matchingPossibilityStates.Concat(listOfStates)
																		.Where(state => state.GetType() == typeof(PossibilityState))
																		.Where(state => ((PossibilityState)state).currentRequirement.Name == node.Name)
																		.Where(state => state.activated.FindByName(node.Name).matchingPossibilities.Count > 1)
																		.Distinct()
																		.ToList();

					listOfPossibleRewinds.AddRange(matchingPossibilityStates);
				}
			}
			return listOfPossibleRewinds;
		}
		private PossibilityState FindStateWithGreatestIndex(List<State> possibleRewinds)
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