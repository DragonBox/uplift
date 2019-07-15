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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Uplift.Common;
using Uplift.Schemas;
using Uplift.Strategies;

namespace Uplift.Packages
{
	public class PackageList
	{
		// Singleton implementation
		protected PackageList()
		{
			Packages = new List<PackageRepo>();
			versionParser = new VersionParser();
		}

		private static PackageList _instance;

		public static PackageList Instance()
		{
			return _instance ?? (_instance = new PackageList());
		}

		// End of Singleton implementation
		protected List<PackageRepo> Packages;
		protected Repository[] Repositories;
		protected VersionParser versionParser;

		public void LoadPackages(Repository[] repositories, bool refresh = false)
		{
			if (refresh)
			{
				Packages.Clear();
			}

			if (Repositories == null)
			{
				Repositories = repositories;
			}

			foreach (Repository repo in repositories)
			{
				LoadPackages(repo);
			}
		}

		public void RefreshPackages()
		{
			Packages.Clear();
			LoadPackages(Repositories);
		}

		public void LoadPackages(Repository repository)
		{
			using (LogAggregator la = LogAggregator.InUnity(
				"Packages successfully loaded from {0}",
				"Packages successfully loaded from {0}, but warning were raised",
				"Error(s) occured while loading packages from {0}",
				repository.ToString()
			))
			{
				Upset[] packages;
				try
				{
					packages = repository.ListPackages();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					return;
				}

				PackageRepo pr;
				foreach (Upset package in packages)
				{
					pr = new PackageRepo
					{
						Package = package,
						Repository = repository
					};
					Packages.Add(pr);
				}
			}
		}

		public PackageRepo[] GetAllPackages()
		{
			return Packages.ToArray();
		}

		public PackageRepo GetLatestPackage(string packageName)
		{
			CandidateSelectionStrategy css = new LatestSelectionStrategy();
			PackageRepo[] packages = Packages.Where(pr => pr.Package.PackageName == packageName).ToArray();

			return css.Filter(packages)[0];
		}

		public PackageRepo[] SelectCandidates(PackageRepo[] candidates, CandidateSelectionStrategy strategy)
		{
			return strategy.Filter(candidates);
		}

		public PackageRepo[] SelectCandidates(PackageRepo[] candidates, CandidateSelectionStrategy[] strategyList)
		{
			PackageRepo[] tempCandidates;

			foreach (var strategy in strategyList)
			{
				tempCandidates = strategy.Filter(candidates);
				candidates = tempCandidates;

			}

			return candidates;
		}

		internal PackageRepo FindPackageAndRepository(DependencyDefinition packageDefinition)
		{
			PackageRepo blankResult = new PackageRepo();
			PackageRepo[] candidates = FindCandidatesForDefinition(packageDefinition);

			var strategies = new CandidateSelectionStrategy[] {
				new OnlyMatchingUnityVersionStrategy (Application.unityVersion),
					new FindBestPackageForUnityVersion (Application.unityVersion),
					new LatestSelectionStrategy ()
			};

			candidates = SelectCandidates(candidates, strategies);

			if (candidates.Length > 0)
			{
				return candidates[0];
			}
			else
			{
				Debug.LogWarning("No package " + packageDefinition.Name + " matching requirements was found");
				return blankResult;
			}
		}

		public DependencyDefinition[] ListDependenciesRecursively(DependencyDefinition packageDefinition)
		{
			DependencyDefinition[] dependencies = new DependencyDefinition[0];

			PackageRepo pr = FindPackageAndRepository(packageDefinition);
			if (pr.Package != null && pr.Package.Dependencies != null)
			{
				dependencies = pr.Package.Dependencies;
				foreach (DependencyDefinition def in pr.Package.Dependencies)
				{
					// Aggregate results
					DependencyDefinition[] packageDependencies = ListDependenciesRecursively(def);
					int newLength = packageDependencies.Length + dependencies.Length;
					DependencyDefinition[] newDeps = new DependencyDefinition[newLength];
					Array.Copy(dependencies, newDeps, dependencies.Length);
					Array.Copy(packageDependencies, 0, newDeps, dependencies.Length, packageDependencies.Length);

					dependencies = newDeps;
				}
			}
			return dependencies;
		}

		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		internal PackageRepo[] FindCandidatesForDefinition(DependencyDefinition packageDefinition)
		{
			return (
				// From All the available packages
				from packageRepo in GetAllPackages()
					// Select the ones that match the definition name
				where packageRepo.Package.PackageName == packageDefinition.Name
				// And the version definition
				where packageDefinition.Requirement.IsMetBy(packageRepo.Package.PackageVersion)
				// And use found package
				select packageRepo
			// As an array
			).ToArray();
		}

		public List<PackageRepo> GetPackageRepo(string requirementName)
		{
			return (
				// From All the available packages
				from packageRepo in GetAllPackages()
					// Select the ones that match the definition name
				where packageRepo.Package.PackageName == requirementName
				// And use found package
				select packageRepo
			// As an List<PackageRepo>
			).ToList();
		}

	}
}