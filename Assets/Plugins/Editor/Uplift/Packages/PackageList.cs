using System.Collections.Generic;
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
        private PackageList()
        {
            Packages = new List<PackageRepo>();
        }

        private static PackageList _instance;

        public static PackageList Instance()
        {
            return _instance ?? (_instance = new PackageList());
        }

        // End of Singleton implementation
        protected List<PackageRepo> Packages;
        protected Repository[] Repositories;

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
            Debug.Log("Loaded packages, count: " + Packages.Count.ToString());
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


    }
}