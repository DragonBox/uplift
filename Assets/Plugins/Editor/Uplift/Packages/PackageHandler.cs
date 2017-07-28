using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Uplift.Common;
using Uplift.Schemas;
using Uplift.Strategies;

namespace Uplift.Packages
{
    public class PackageHandler
    {


        public enum Comparison { NA = 0, LOWER, HIGHER, SAME };
        public struct CompareResult
        {
            public Comparison Major, Minor, Version;
        }

        public struct VersionStruct
        {
            public int Major, Minor, Version;
        }

        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
        internal PackageRepo[] FindCandidatesForDefinition(DependencyDefinition packageDefinition)
        {
            PackageList pList = PackageList.Instance();

            return (
                // From All the available packages
                from packageRepo in pList.GetAllPackages()
                // Select the ones that match the definition
                where packageRepo.Package.PackageName == packageDefinition.Name
                // And prepare compareResult for them
                let compareResult = CompareVersions(packageRepo.Package, packageDefinition)
                // Find the ones which match the logic being:
                // 1. Major version HIGHER, or
                // 2. Major version SAME AND Minor version HIGHER, or
                // 3. Major version SAME, Minor version SAME, Build version HIGHER or SAME
                where
                    (compareResult.Major == Comparison.HIGHER) ||
                    (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.HIGHER) ||
                    (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.SAME &&
                        (compareResult.Version == Comparison.HIGHER || compareResult.Version == Comparison.SAME)
                    )
                // And use found package
                select packageRepo
            
            // As an array
            ).ToArray();
        }

        public PackageRepo[] SelectCandidates(PackageRepo[] candidates, CandidateSelectionStrategy strategy)
        {
            return strategy.Filter(candidates);
        }
        public PackageRepo[] SelectCandidates(PackageRepo[] candidates, CandidateSelectionStrategy[] strategyList)
        {
            return strategyList.Aggregate(new PackageRepo[0], (selected, next) => (selected.Union(SelectCandidates(candidates, next))).ToArray());
        }


        internal PackageRepo FindPackageAndRepository(DependencyDefinition packageDefinition)
        {
            PackageRepo blankResult = new PackageRepo();

            PackageRepo[] candidates = FindCandidatesForDefinition(packageDefinition);

            candidates = SelectCandidates(candidates, new LatestSelectionStrategy());

            if (candidates.Length > 0)
            {
                return candidates[0];
            }
            else
            {
                Debug.LogWarning("Package: " + packageDefinition.Name + " not found");
                return blankResult;
            }



        }

        protected CompareResult CompareVersions(VersionStruct packageVersion, VersionStruct dependencyVersion)
        {
            CompareResult result = new CompareResult();

            // Major version comparison
            if (packageVersion.Major < dependencyVersion.Major)
            {
                result.Major = Comparison.LOWER;
                return result;
            }
            else if (packageVersion.Major > dependencyVersion.Major)
            {
                result.Major = Comparison.HIGHER;
                return result;
            }
            else if (packageVersion.Major == dependencyVersion.Major)
            {
                result.Major = Comparison.SAME;
            }
            else
            {
                result.Major = Comparison.NA;
                return result;
            }


            // Minor version comparison
            if (packageVersion.Minor < dependencyVersion.Minor)
            {
                result.Minor = Comparison.LOWER;
                return result;
            }
            else if (packageVersion.Minor > dependencyVersion.Minor)
            {
                result.Minor = Comparison.HIGHER;
                return result;
            }
            else if (packageVersion.Minor == dependencyVersion.Minor)
            {
                result.Minor = Comparison.SAME;
            }
            else
            {
                result.Minor = Comparison.NA;
                return result;
            }


            // Version version comparison
            if (packageVersion.Version < dependencyVersion.Version)
            {
                result.Version = Comparison.LOWER;
                return result;
            }
            else if (packageVersion.Version > dependencyVersion.Version)
            {
                result.Version = Comparison.HIGHER;
                return result;
            }
            else if (packageVersion.Version == dependencyVersion.Version)
            {
                result.Version = Comparison.SAME;
            }
            else
            {
                result.Version = Comparison.NA;
                return result;
            }

            return result;
        }
        protected CompareResult CompareVersions(Upset package, DependencyDefinition dependencyDefinition)
        {

            VersionStruct packageVersion = ParseVersion(package);
            VersionStruct dependencyVersion = ParseVersion(dependencyDefinition);

            return CompareVersions(packageVersion, dependencyVersion);

        }



        protected VersionStruct ParseVersion(DependencyDefinition dependencyDefinition)
        {
            return ParseVersion(dependencyDefinition.Version);
        }

        protected VersionStruct ParseVersion(Upset package)
        {
            return ParseVersion(package.PackageVersion);
        }

        public static VersionStruct ParseVersion(string versionString)
        {
            const string versionMatcherRegexp = @"(?<major>\d+)(\.(?<minor>\d+))?(\.(?<version>\d+))?";

            Match matchObject = Regex.Match(versionString, versionMatcherRegexp);

            VersionStruct result = new VersionStruct
            {
                Major = ExtractVersion(matchObject, "major"),
                Minor = ExtractVersion(matchObject, "minor"),
                Version = ExtractVersion(matchObject, "version")
            };


            return result;
        }

        protected static int ExtractVersion(Match match, string groupName)
        {
            try
            {
                return int.Parse(match.Groups[groupName].ToString());
            }
            catch (FormatException)
            {
                return 0;
            }

        }
    }
}