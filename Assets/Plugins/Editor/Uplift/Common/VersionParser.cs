using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Uplift.Schemas;

namespace Uplift.Common
{
    public class VersionParser
    {
        public static IVersionRequirement ParseRequirement(string requirement)
        {
            if (string.IsNullOrEmpty(requirement))
            {
                return new NoRequirement();
            }
            else if (requirement.EndsWith("!"))
            {
                return new ExactVersionRequirement(requirement.TrimEnd('!'));
            }
            else if (requirement.EndsWith("+"))
            {
                return new MinimalVersionRequirement(requirement.TrimEnd('+'));
            }
            else if (requirement.EndsWith(".*"))
            {
                return new BoundedVersionRequirement(requirement.TrimEnd('*').TrimEnd('.'));
            }
            else
            {
                return new LoseVersionRequirement(requirement);
            }
        }

        public static VersionStruct ParseVersion(string version)
        {
            const string matcher = @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)";
            Match matchObject = Regex.Match(version, matcher);
            VersionStruct result = new VersionStruct
            {
                Major = 0,
                Minor = 0,
                Patch = 0,
                Optional = null
            };
            try
            {
                result.Major = int.Parse(matchObject.Groups["major"].ToString());
                result.Minor = int.Parse(matchObject.Groups["minor"].ToString());
                result.Patch = int.Parse(matchObject.Groups["patch"].ToString());
            }
            catch (FormatException e)
            {
                Debug.LogWarning(string.Format("Version {0} does not respect the MAJOR.MINOR.PATCH structure ({1}).", version, e));
                return ParseIncompleteVersion(version);
            }
            return result;
        }

        public static VersionStruct ParseIncompleteVersion(string version)
        {
            VersionStruct result = new VersionStruct
            {
                Major = 0,
                Minor = null,
                Patch = null,
                Optional = null
            };

            string rest = "";
            result.Major = ParseBeginning(version, ref rest);
            if(rest != "")
            {
                string temp = rest;
                result.Minor = ParseBeginning(temp, ref rest);
                if (rest != "")
                {
                    temp = rest;
                    result.Patch = ParseBeginning(temp, ref rest);
                    if (rest != "")
                    {
                        temp = rest;
                        result.Optional = ParseBeginning(temp, ref rest);
                    }
                }
            }

            return result;
        }

        private static int ParseBeginning(string input, ref string rest)
        {
            const string matcher = @"(?<item>\d+)\.?(?<identifier>[a-zA-Z]+)?\.?(?<rest>[\w\.]+)?";
            Match matchObject = Regex.Match(input, matcher);
            int item = 0;
            try { item = int.Parse(matchObject.Groups["item"].ToString()); }
            catch (FormatException) { return 0; }
            try
            {
                string identifier = matchObject.Groups["identifier"].ToString();
                if (!string.IsNullOrEmpty(identifier)) Debug.LogWarning(string.Format("Uplift does not support non-numeric identifiers such as {0}", identifier));
            }
            catch (FormatException) { }
            try
            { rest = matchObject.Groups["rest"].ToString(); }
            catch (FormatException) { }
            return item;
        }

        public static VersionStruct ParseUnityVersion(string version)
        {
            const string matcher = @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)\w+(?<build>\d+)\w*";
            Match matchObject = Regex.Match(version, matcher);
            VersionStruct result = new VersionStruct
            {
                Major = int.Parse(matchObject.Groups["major"].ToString()),
                Minor = int.Parse(matchObject.Groups["minor"].ToString()),
                Patch = int.Parse(matchObject.Groups["patch"].ToString()),
                Optional = int.Parse(matchObject.Groups["build"].ToString())
            };

            return result;
        }
    }
}
