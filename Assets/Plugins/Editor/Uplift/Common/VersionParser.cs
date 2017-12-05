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
using System.Text.RegularExpressions;
using UnityEngine;

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
                if(ParseVersion(requirement) != new Version { Major = 0 })
                    return new LoseVersionRequirement(requirement);
            }
            throw new ArgumentException("Cannot parse requirement from " + requirement);
        }

        public static Version ParseVersion(string version, bool verbose = true)
        {
            const string matcher = @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)";
            Match matchObject = Regex.Match(version, matcher);
            Version result = new Version
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
                if(verbose) {
                    Debug.LogWarning(string.Format("Version {0} does not respect the MAJOR.MINOR.PATCH structure ({1}).", version, e));
                }
                return ParseIncompleteVersion(version);
            }
            return result;
        }

        public static Version ParseIncompleteVersion(string version)
        {
            Version result = new Version
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

        public static Version ParseUnityVersion(string version)
        {
            const string matcher = @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)\w+(?<build>\d+)\w*";
            Match matchObject = Regex.Match(version, matcher);
            Version result = new Version
            {
                Major = int.Parse(matchObject.Groups["major"].ToString()),
                Minor = int.Parse(matchObject.Groups["minor"].ToString()),
                Patch = int.Parse(matchObject.Groups["patch"].ToString()),
                Optional = int.Parse(matchObject.Groups["build"].ToString())
            };

            return result;
        }

        public static bool GreaterThan(string a, string b)
        {
            return ParseVersion(a) > ParseVersion(b);
        }
    }
}
