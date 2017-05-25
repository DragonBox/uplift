using System;
using Schemas;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

class PackageHandler
{
    public struct PackageRepo
    {
        public Schemas.Upset package;
        public Schemas.Repository repository;
    }

    internal enum Comparison { NA = 0, LOWER, HIGHER, SAME };
    public struct CompareResult {
        public Comparison Major, Minor, Version;
    }

    public struct VersionStruct {
        public int Major, Minor, Version;
    }

    internal PackageRepo FindPackageAndRepository(DependencyDefinition packageDefinition, Repository[] repositories)
    {
        PackageRepo result = new PackageRepo();


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
                    result.repository = repo;
                    result.package = package;
                    return result;
                }
                else {
                    continue;
                }


                

            }
        }

        Debug.LogWarning("Package: " + packageDefinition.Name + " not found");
        return result;


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

    protected VersionStruct ParseVersion(string versionString) {
        string versionMatcherRegexp = @"(?<major>\d+)(\.(?<minor>\d+))?(\.(?<version>\d+))?";

        Match matchObject = Regex.Match(versionString, versionMatcherRegexp);

        var result = new VersionStruct();

        result.Major = ExtractVersion(matchObject, "major");
        result.Minor = ExtractVersion(matchObject, "minor");
        result.Version = ExtractVersion(matchObject, "version");

        return result;
    }

    protected int ExtractVersion(Match match, String groupName) {
        return Int32.Parse(match.Groups[groupName].ToString());
    }
}