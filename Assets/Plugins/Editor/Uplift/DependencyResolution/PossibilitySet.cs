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

	public class PossibilitySet
	{
		public string name;
		public List<Upset> packages = new List<Upset>();

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

		override public string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(name + " : ");
			foreach (Upset package in packages)
			{
				sb.Append("[" + package.PackageVersion + "] ");
			}
			return sb.ToString();
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

		public static List<PossibilitySet> GetPossibilitySetsForGivenPackage(string packageName) //TODO To Test
		{
			Debug.Log("- Getting possibility sets for : " + packageName);
			List<PossibilitySet> possibilities = null;

			foreach (Upset package in Resolver.packageRepoStub.GetPackages(packageName))
			{
				Debug.Log("- Found " + package.PackageName + " " + package.PackageVersion + " on repo.");
				List<DependencyDefinition> currentDependencies = null;
				if (package.Dependencies != null && package.Dependencies.Length > 0)
				{
					currentDependencies = new List<DependencyDefinition>(package.Dependencies);
					Debug.Log("Depends on : ");
					foreach (var dep in currentDependencies)
					{
						Debug.Log("|| " + dep.Name + " " + dep.Version);
					}
				}

				if (possibilities == null)
				{
					Debug.Log("List of possibilities doesn't exist yet, creating one");
					PossibilitySet newPossibilitySet = new PossibilitySet();
					newPossibilitySet.name = package.PackageName;
					newPossibilitySet.packages = new List<Upset>();
					newPossibilitySet.packages.Add(package);

					possibilities = new List<PossibilitySet>();

					Debug.Log("Adding possibility set in Possibilities for " + package.PackageName + " " + package.PackageVersion);
					possibilities.Add(newPossibilitySet);
				}
				else if (possibilities != null && possibilities.Count > 0)
				{
					PossibilitySet newPossibilitySet = null;

					Debug.Log("Exploring existing possibilitySets");

					bool hasMatchedPossibilitySet = false;
					foreach (PossibilitySet possibilitySet in possibilities)
					{
						if (possibilitySet == null
						|| possibilitySet.packages == null
						|| possibilitySet.packages.Count == 0)
						{
							Debug.Log("PossibilitySet is null or empty");
							break;
						}
						DependencyDefinition[] existingDependenciesArray = possibilitySet.packages.ToArray()[0].Dependencies;

						if (existingDependenciesArray == null || existingDependenciesArray.Length == 0)
						{
							Debug.Log("PossibilitySet has no dependency");
							if (package.Dependencies == null || package.Dependencies.Length < 1)
							{
								Debug.Log("Current package doesn't have dependency either, adding package to possibilitySet");
								possibilitySet.packages.Add(package);
								hasMatchedPossibilitySet = true;
							}
							break;
						}

						List<DependencyDefinition> existingDependencies = new List<DependencyDefinition>(existingDependenciesArray);

						if (existingDependencies.Count == currentDependencies.Count)
						{
							Debug.Log("PossibilitySet has same number of dependencies");
							bool hasSameDependencies = true;
							foreach (DependencyDefinition dd in currentDependencies)
							{
								if (!existingDependencies.Contains(dd))
								{
									Debug.Log("PossibilitySet does not match, skipping to next possibilitySet");
									hasSameDependencies = false;
									break;
								}
							}

							if (hasSameDependencies)
							{
								Debug.Log("PossibilitySet matches, adding package to possibilitySet");
								possibilitySet.packages.Add(package);
								hasMatchedPossibilitySet = true;
								break;
							}
						}
					}

					if (!hasMatchedPossibilitySet)
					{
						Debug.Log("No PossibilitySet matched current package dependencies, creating a new possibilitySet for package " + package.PackageName);

						newPossibilitySet = new PossibilitySet();
						newPossibilitySet.name = package.PackageName;
						newPossibilitySet.packages = new List<Upset>();
						newPossibilitySet.packages.Add(package);
						possibilities.Add(newPossibilitySet);
					}
				}
			}
			return possibilities;
		}

		public static List<PossibilitySet> GetMatchingPossibilities(List<PossibilitySet> possibilities, DependencyDefinition currentRequirement)
		{
			List<PossibilitySet> matchingPossibilities = new List<PossibilitySet>();
			foreach (PossibilitySet possibilitySet in possibilities)
			{
				if (possibilitySet.name != currentRequirement.Name)
				{
					continue;
				}
				else
				{
					PossibilitySet newMatchingPossibilitySet = null;
					foreach (Upset pkg in possibilitySet.packages)
					{
						if (currentRequirement.Requirement.IsMetBy(pkg.PackageVersion))
						{
							Debug.Log("--[/] Package " + pkg.PackageName + " " + "[" + pkg.PackageVersion + "]" + " matches requirement : " + currentRequirement.ToString());
							if (newMatchingPossibilitySet == null)
							{
								newMatchingPossibilitySet = new PossibilitySet();
								newMatchingPossibilitySet.name = possibilitySet.name;
							}
							newMatchingPossibilitySet.packages.Add(pkg);
						}
						else
						{
							Debug.Log("--[X] Package " + pkg.PackageName + " " + "[" + pkg.PackageVersion + "]" + " does not match requirement : " + currentRequirement.ToString());
						}
					}
					if (newMatchingPossibilitySet != null)
					{
						matchingPossibilities.Add(newMatchingPossibilitySet);
					}
				}
			}
			return matchingPossibilities;
		}
	}
}