using System;
using Schemas;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class PackageHandler
{
    public struct PackageRepo
    {
        public Schemas.Upset package;
        public Schemas.Repository repository;
    }

    public enum Comparison { NA = 0, LOWER, HIGHER, SAME };
    public struct CompareResult {
        public Comparison Major, Minor, Version;
    }

    public struct VersionStruct {
        public int Major, Minor, Version;
    }

    internal PackageRepo[] FindCandidatesForDefinition(DependencyDefinition packageDefinition, Repository[] repositories) {
        List<PackageRepo> result = new List<PackageRepo>();

        foreach (var repo in repositories)
        {
            Upset[] repoPackages = repo.ListPackages();

            foreach (var package in repoPackages)
            {
                
                if (package.PackageName != packageDefinition.Name)
                {
                    continue;
                }

                CompareResult compareResult = CompareVersions(package, packageDefinition);

                if (
                    (compareResult.Major == Comparison.HIGHER) ||
                    (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.HIGHER ) ||
                    (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.SAME && (compareResult.Version == Comparison.HIGHER || compareResult.Version == Comparison.SAME))
                )
                {
                    PackageRepo definition = new PackageRepo();
                    definition.repository = repo;
                    definition.package = package;
                    result.Add(definition);
                }
                else {
                    continue;
                }


                

            }
        }

        return result.ToArray();
    }

    public PackageRepo[] SelectCandidates(PackageRepo[] candidates, CandidateSelectionStrategy strategy) {
        return strategy.Filter(candidates);
    }
    public PackageRepo[] SelectCandidates(PackageRepo[] candidates, CandidateSelectionStrategy[] strategyList) {

        foreach(var strategy in strategyList) {
            candidates = SelectCandidates(candidates, strategy);

        }

        return candidates;
    }


    internal PackageRepo FindPackageAndRepository(DependencyDefinition packageDefinition, Repository[] repositories)
    {
        PackageRepo blankResult = new PackageRepo();

        PackageRepo[] candidates = FindCandidatesForDefinition(packageDefinition, repositories);

        candidates =  SelectCandidates(candidates, new LatestSelectionStrategy());

        if(candidates.Length > 0) {
            return candidates[0];
        } else {
            Debug.LogWarning("Package: " + packageDefinition.Name + " not found");
            return blankResult;
        }      
        


    }

    protected CompareResult CompareVersions(VersionStruct packageVersion, VersionStruct dependencyVersion) {
        var result = new CompareResult();

        // Major version comparison
        if(packageVersion.Major < dependencyVersion.Major) {
            result.Major = Comparison.LOWER;
            return result;
        } else if(packageVersion.Major > dependencyVersion.Major) {
            result.Major = Comparison.HIGHER;
            return result;
        } else if(packageVersion.Major == dependencyVersion.Major) {
            result.Major = Comparison.SAME;
        } else {
            result.Major = Comparison.NA;
            return result;
        }


        // Minor version comparison
        if(packageVersion.Minor < dependencyVersion.Minor) {
            result.Minor = Comparison.LOWER;
            return result;
        } else if(packageVersion.Minor > dependencyVersion.Minor) {
            result.Minor = Comparison.HIGHER;
            return result;
        } else if(packageVersion.Minor == dependencyVersion.Minor) {
            result.Minor = Comparison.SAME;
        } else {
            result.Minor = Comparison.NA;
            return result;
        }


        // Version version comparison
        if(packageVersion.Version < dependencyVersion.Version) {
            result.Version = Comparison.LOWER;
            return result;
        } else if(packageVersion.Version > dependencyVersion.Version) {
            result.Version = Comparison.HIGHER;
            return result;
        } else if(packageVersion.Version == dependencyVersion.Version) {
            result.Version = Comparison.SAME;
        } else {
            result.Version = Comparison.NA;
            return result;
        }

        return result;
    }
    protected CompareResult CompareVersions(Upset package, DependencyDefinition dependencyDefinition) {      
        
        VersionStruct packageVersion = ParseVersion(package);
        VersionStruct dependencyVersion = ParseVersion(dependencyDefinition);

        return CompareVersions(packageVersion, dependencyVersion);

    }



    protected VersionStruct ParseVersion(DependencyDefinition dependencyDefinition) {
         return ParseVersion(dependencyDefinition.Version);
    }

    protected VersionStruct ParseVersion(Upset package) {
        return ParseVersion(package.PackageVersion);
    }

    public static VersionStruct ParseVersion(string versionString) {
        string versionMatcherRegexp = @"(?<major>\d+)(\.(?<minor>\d+))?(\.(?<version>\d+))?";

        Match matchObject = Regex.Match(versionString, versionMatcherRegexp);

        var result = new VersionStruct();

        result.Major = ExtractVersion(matchObject, "major");
        result.Minor = ExtractVersion(matchObject, "minor");
        result.Version = ExtractVersion(matchObject, "version");

        return result;
    }

    protected static int ExtractVersion(Match match, String groupName) {
        return Int32.Parse(match.Groups[groupName].ToString());
    }
}