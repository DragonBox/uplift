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
using UnityEngine;
using System.Text;

namespace Uplift.DependencyResolution
{
	class Resolver //: IDependencySolver
	{
		public static PackageRepoStub packageRepoStub;

		DependencyGraph baseGraph;
		Stack<DependencyDefinition> originalDependencies;
		Stack<State> stateStack = new Stack<State>();

		public Resolver(Stack<DependencyDefinition> originalDependencies, DependencyGraph baseGraph)
		{
			this.originalDependencies = originalDependencies;
			this.baseGraph = baseGraph;
		}

		// Start resolution process
		void StartResolution()
		{
			Debug.Log("Start Resolution");
			pushInitialState();
		}

		public void pushInitialState()
		{
			Debug.Log("Pushing initial state");
			DependencyGraph dg = new DependencyGraph();

			foreach (DependencyDefinition requested in originalDependencies)
			{
				DependencyNode node = new DependencyNode(requested);
				node.restrictions["initial"] = requested.Requirement;
				dg.AddNode(node);
			}

			Stack<DependencyDefinition> currentDependencies = originalDependencies;
			List<Conflict> conflicts = new List<Conflict>();
			DependencyState initialState = new DependencyState(currentDependencies,
																dg,
																new List<PossibilitySet>(), //possibilities
																0,
																conflicts //conflicts
																);
			stateStack.Push(initialState);
			Debug.Log("===> Initial state : ");
			Debug.Log(initialState);
		}

		List<PossibilitySet> GeneratePossibilitySets(State state)
		{
			List<PossibilitySet> possibilities = state.possibilities;
			foreach (DependencyDefinition dependency in state.requirements)
			{
				if (!possibilities.Exists(possibilitySet => possibilitySet.name == dependency.Name))
				{
					Debug.Log(dependency.Name + " is not listed in possibility sets");
					possibilities.AddRange(PossibilitySet.GetPossibilitySetsForGivenPackage(dependency.Name));
				}
			}
			return possibilities;
		}

		void EndResolution()
		{
			Debug.Log("Ending resolution");
		}

		void ShowStateStack()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("==== State Stack ====");
			foreach (State state in stateStack)
			{
				sb.AppendLine("[ (" + state.depth.ToString() + ") " + state.GetType().ToString() + " ]");
			}
			sb.AppendLine("================");
			Debug.Log(sb.ToString());
		}
		public List<Upset> SolveDependencies()
		{
			//FIXME Clean code here and split in sub methods
			Debug.Log("Solve dependencies");
			StartResolution();

			List<Upset> resolution = new List<Upset>();

			int i = 100;
			while (i > 0)//stateStack.Count > 0)
			{
				if (stateStack.Count == 0)
				{
					break;
				}

				i--;
				ShowStateStack();
				State currentState = stateStack.Peek();

				if (currentState.GetType() == typeof(DependencyState))
				{
					Debug.Log("Current state is dependency state !");
					List<PossibilitySet> possibilitySets = GeneratePossibilitySets(currentState);
					currentState.possibilities = possibilitySets;

					//TODO For debug
					foreach (PossibilitySet possibilitySet in possibilitySets)
					{
						Debug.Log(possibilitySet);
					}

					if (currentState.requirements.Count == 0)
					{
						Debug.Log("No more requirements to match, getting solutions : ");
						resolution = ((DependencyState)currentState).GetResolution();
						break;
					}
					else
					{
						PossibilityState newPossibilityState = ((DependencyState)currentState).PopPossibilityState();
						if (newPossibilityState != null)
						{
							Debug.Log("Add new possibility state in stack");
							stateStack.Push(newPossibilityState);
						}
					}
				}
				else if (currentState.GetType() == typeof(PossibilityState))
				{
					Debug.Log("Current state is possibility state !");
					DependencyState newState = ((PossibilityState)currentState).SolveState();

					if (currentState.conflicts != null && currentState.conflicts.Count > 0)
					{
						Conflict conflict = currentState.conflicts.ToArray()[0];
						Rewinder rewinder = new Rewinder(stateStack);
						stateStack = rewinder.UnwindForConflict(conflict);
						currentState.conflicts.Remove(conflict);

						//TODO Remove
						//stateStack = ((PossibilityState)currentState).UnwindForConflict(stateStack);
					}
					else
					{
						Debug.Log("Push new dependency state in stack");
						if (newState == null)
						{
							Debug.LogError("error, no viable solution found");
							break;
						}
						stateStack.Push(newState);
					}
				}
				else
				{
					Debug.LogError("Error : Current state is neither possibility or dependency state");
				}
			}

			EndResolution();
			Debug.Log("===== Final resolution : =====");
			foreach (Upset pkg in resolution)
			{
				Debug.Log(pkg.PackageName + " : " + pkg.PackageVersion);
			}
			return resolution;
		}
	}
}