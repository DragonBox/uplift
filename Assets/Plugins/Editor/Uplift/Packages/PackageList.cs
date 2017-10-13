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

        public void LoadPackages(Repository[] repositories)
        {
            if (Repositories == null)
            {
                Repositories = repositories;
            }

            foreach (Repository repo in repositories)
            {
                LoadPackages(repo);
            }
        }

        public void LoadPackages(Repository[] repositories, bool refresh = false)
        {
            if (refresh)
            {
                Packages.Clear();
            }
            LoadPackages(repositories);
        }

        public void RefreshPackages()
        {
            Packages.Clear();
            LoadPackages(Repositories);
        }

        public void LoadPackages(Repository repository)
        {
            PackageRepo pr;
            foreach (Upset package in repository.ListPackages())
            {
                pr = new PackageRepo
                {
                    Package = package,
                    Repository = repository
                };
                Packages.Add(pr);

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

            foreach(var strategy in strategyList) {
                tempCandidates = strategy.Filter(candidates);
                candidates = tempCandidates;

            }

            return candidates;
        }


        internal PackageRepo FindPackageAndRepository(DependencyDefinition packageDefinition)
        {
            PackageRepo blankResult = new PackageRepo();
            PackageRepo[] candidates = FindCandidatesForDefinition(packageDefinition);

            var strategies = new CandidateSelectionStrategy[]{
                new OnlyMatchingUnityVersionStrategy(Application.unityVersion),
                new FindBestPackageForUnityVersion(Application.unityVersion),
                new LatestSelectionStrategy()
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
    }
}
