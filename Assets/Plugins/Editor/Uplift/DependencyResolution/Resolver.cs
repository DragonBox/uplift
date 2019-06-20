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
		public PackageRepoStub packageRepoStub;
		// --> parameters
		// SpecificationProvider

		// -- Base of dependency graph (aka a node ?)
		DependencyGraph baseGraph;
		Stack<DependencyDefinition> originalDependencies;
		Stack<State> stateStack = new Stack<State>();

		// --> methods

		// Initialize(specification provider, originalDependencies, graphRoots)
		// |__ Affect class parameters

		public Resolver(Stack<DependencyDefinition> originalDependencies, DependencyGraph baseGraph)
		{
			this.originalDependencies = originalDependencies;
			this.baseGraph = baseGraph;
		}

		// Start resolution process
		void StartResolution()
		{
			Debug.Log("Start Resolution");
			//startedAt = Time.now;
			pushInitialState();
		}

		// Ends the resolution process
		void EndResolution()
		{
			Debug.Log("Ending resolution");
		}

		DependencyDefinition[] SolveDependencies(DependencyDefinition[] dependencies)
		{
			StartResolution();
			/*
			While states is not empty{
   				//requirements = https://github.com/CocoaPods/Molinillo/blob/master/lib/molinillo/dependency_graph/vertex.rb
				if state has requirements
				{
					if (state is DependencyState)
					{
						pop off a PossibilityState
						if (PossibilityState != null)
						{
							push PossibilityState to states
							activated.tag(PossibilityState)
						}
					}
					processTopMostState();
				}else{
					continue
				}
				resolveActivatedSpecs()
			}
			EndResolution();
			*/
		}

		void processTopMostState()
		{
			/*
				try
				{
					if (PossibilitySet.possibilities.last != null)
					{
						attemptToActivate();
					}
					else
					{
						Conflict conflict = new Conflict([...]);
						unwindForConflict(conflict);
					}
				}
				catch (CircularDependencyError underlyingError)
				{
					create_conflict(underlyingError);
					unwindForConflict;
				}
			*/
		}

		void resolveActivatedSpecs()
		{
			for (each vertex in activated.nodes)
			{
				if (vertex.payload == null)
				{
					continue;
				}
				else
				{
					latestVersion =
				}

			}

			/*
				activated.vertices.each do |_, vertex|
					next unless vertex.payload

					latest_version = vertex.payload.possibilities.reverse_each.find do |possibility|
					vertex.requirements.all? { |req| requirement_satisfied_by?(req, activated, possibility) }
					end

					activated.set_payload(vertex.name, latest_version)
				end
				activated.freeze
			 */
		}
	}


}