using System;
using System.Text.RegularExpressions;
using Uplift.Schemas;

namespace Uplift.Common
{
    public class VersionParser
    {
        public enum Comparison { NA = 0, LOWER = 1, SAME = 2, HIGHER = 3 };
        public enum UnityRelease { ALPHA = 0, BETA = 1, RELEASE = 2, PATCH = 3 };
        readonly static string[] releases = { "a", "b", "f", "p", "xa", "xb", "xf", "xp" };

        public struct CompareResult
        {
            public Comparison Major, Minor, Build, Release, Revision;
        }

        public struct VersionStruct
        {
            public int Major, Minor, Build, Revision;
            public UnityRelease Release;
        }

        public bool GreaterThan(string compared, string existing)
        {
            return IsComparisonGreater(CompareVersions(compared, existing));
        }

        public bool GreaterThan(Upset package, DependencyDefinition definition)
        {
            return IsComparisonGreater(CompareVersions(package, definition));
        }

        internal bool IsComparisonGreater(CompareResult compareResult)
        {
            // Package matches version definition if
            // 1. Major version HIGHER, or
            // 2. Major version SAME AND Minor version HIGHER, or
            // 3. Major version SAME, Minor version SAME, Build version HIGHER or SAME
            return (compareResult.Major == Comparison.HIGHER) ||
                (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.HIGHER) ||
                (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.SAME && compareResult.Build == Comparison.HIGHER) ||
                (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.SAME && compareResult.Build == Comparison.SAME &&
                    compareResult.Release == Comparison.HIGHER) ||
                (compareResult.Major == Comparison.SAME && compareResult.Minor == Comparison.SAME && compareResult.Build == Comparison.SAME &&
                    compareResult.Release == Comparison.SAME && compareResult.Revision >= Comparison.SAME);
        }

        public CompareResult CompareVersions(VersionStruct packageVersion, VersionStruct dependencyVersion)
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

            // Build version comparison
            if (packageVersion.Build < dependencyVersion.Build)
            {
                result.Build = Comparison.LOWER;
                return result;
            }
            else if (packageVersion.Build > dependencyVersion.Build)
            {
                result.Build = Comparison.HIGHER;
                return result;
            }
            else if (packageVersion.Build == dependencyVersion.Build)
            {
                result.Build = Comparison.SAME;
            }
            else
            {
                result.Build = Comparison.NA;
                return result;
            }

            // Release strength comparison
            if (packageVersion.Release < dependencyVersion.Release)
            {
                result.Release = Comparison.LOWER;
                return result;
            }
            else if (packageVersion.Release > dependencyVersion.Release)
            {
                result.Release = Comparison.HIGHER;
                return result;
            }
            else if (packageVersion.Release == dependencyVersion.Release)
            {
                result.Release = Comparison.SAME;
            }
            else
            {
                result.Release = Comparison.NA;
                return result;
            }

            // Revision version comparison
            if (packageVersion.Revision < dependencyVersion.Revision)
            {
                result.Revision = Comparison.LOWER;
                return result;
            }
            else if (packageVersion.Revision > dependencyVersion.Revision)
            {
                result.Revision = Comparison.HIGHER;
                return result;
            }
            else if (packageVersion.Revision == dependencyVersion.Revision)
            {
                result.Revision = Comparison.SAME;
            }
            else
            {
                result.Revision = Comparison.NA;
                return result;
            }

            return result;
        }

        public CompareResult CompareVersions(Upset package, DependencyDefinition dependencyDefinition)
        {
            VersionStruct packageVersion = ParseVersion(package);
            VersionStruct dependencyVersion = ParseVersion(dependencyDefinition);

            return CompareVersions(packageVersion, dependencyVersion);
        }

        public CompareResult CompareVersions(string a, string b)
        {
            VersionStruct versionA = ParseVersion(a);
            VersionStruct versionB = ParseVersion(b);

            return CompareVersions(versionA, versionB);
        }

        public VersionStruct ParseVersion(DependencyDefinition dependencyDefinition)
        {
            return ParseVersion(dependencyDefinition.Version);
        }

        public VersionStruct ParseVersion(Upset package)
        {
            return ParseVersion(package.PackageVersion);
        }

        public static VersionStruct ParseVersion(string versionString)
        {
            const string versionMatcherRegexp = @"(?<major>\d+)(\.(?<minor>\d+))?(\.(?<build>\d+))?(((?<release>\w)|\.)(?<revision>\d+))?";

            Match matchObject = Regex.Match(versionString, versionMatcherRegexp);

            VersionStruct result = new VersionStruct
            {
                Major = ExtractVersion(matchObject, "major"),
                Minor = ExtractVersion(matchObject, "minor"),
                Build = ExtractVersion(matchObject, "build"),
                Release = ExtractRelease(matchObject),
                Revision = ExtractVersion(matchObject, "revision")
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

        protected static UnityRelease ExtractRelease(Match match)
        {
            try
            {
                string release = match.Groups["release"].ToString();
                for(int i = 0; i < releases.Length; i++)
                {
                    if(releases[i] == release)
                    {
                        return (UnityRelease)(i % 4);
                    }
                }

                return UnityRelease.ALPHA;
            }
            catch (FormatException)
            {
                return UnityRelease.ALPHA;
            }
        }
    }
}
