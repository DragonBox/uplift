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

#if UNITY_5_3_OR_NEWER
using NUnit.Framework;
using Uplift.Common;

namespace Uplift.Testing.Unit
{
    namespace VersionTest
    {
        [TestFixture]
        class VersionParserTest
        {
            [Test]
            public void ParseEmptyRequirement()
            {
                Assert.IsTrue(VersionParser.ParseRequirement("") is NoRequirement);
            }

            [Test]
            public void ParseMinimalRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2+");
                Assert.IsTrue(parsed is MinimalVersionRequirement);
                Assert.AreEqual((parsed as MinimalVersionRequirement).minimal, new Version { Major = 1, Minor = 2 });
            }

            [Test]
            public void ParseLoseRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2");
                Assert.IsTrue(parsed is LoseVersionRequirement);
                Assert.AreEqual((parsed as LoseVersionRequirement).lower, new Version { Major = 1, Minor = 2 });
            }

            [Test]
            public void ParseBoundedRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2.*");
                Assert.IsTrue(parsed is BoundedVersionRequirement);
                Assert.AreEqual((parsed as BoundedVersionRequirement).lower, new Version { Major = 1, Minor = 2 });
            }

            [Test]
            public void ParseExactRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2.3!");
                Assert.IsTrue(parsed is ExactVersionRequirement);
                Assert.AreEqual((parsed as ExactVersionRequirement).expected, new Version { Major = 1, Minor = 2, Patch = 3 });
            }

            [Test]
            public void ParseRangeRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2,3.4");
                Assert.IsTrue(parsed is RangeVersionRequirement);
                Assert.AreEqual((parsed as RangeVersionRequirement).lower, new Version { Major = 1, Minor = 2 });
                Assert.AreEqual((parsed as RangeVersionRequirement).upper, new Version { Major = 3, Minor = 4 });
            }

            [Test]
            public void DoNotParseInvertedRangeRequirement()
            {
                Assert.Throws<System.ArgumentException>(
                    delegate
                    {
                       VersionParser.ParseRequirement("3.4,1.2");
                    }
                );
            }

            [Test]
            public void DoNotParseWrongRequirement()
            {
                // QUESTION: Should we not fail there?
                Assert.Throws<System.ArgumentException>(
                    delegate
                    {
                        VersionParser.ParseRequirement("not.a.requirement");
                    }
                );
            }

            [Test]
            public void ParseCorrectUnityVersions()
            {
                Assert.AreEqual(VersionParser.ParseUnityVersion("1.2.3f4"),
                    new Version
                    {
                        Major = 1,
                        Minor = 2,
                        Patch = 3,
                        Optional = 4
                    }
                );
                Assert.AreEqual(VersionParser.ParseUnityVersion("5.6.1b1"),
                    new Version
                    {
                        Major = 5,
                        Minor = 6,
                        Patch = 1,
                        Optional = 1
                    }
                );
                Assert.AreEqual(VersionParser.ParseUnityVersion("2017.1.0a3"),
                    new Version
                    {
                        Major = 2017,
                        Minor = 1,
                        Patch = 0,
                        Optional = 3
                    }
                );
            }

            [Test]
            public void DoNotParseUncorrectUnityVersions()
            {
                Assert.Throws<System.FormatException>(
                    delegate
                    {
                        VersionParser.ParseUnityVersion("This should never be a Unity version");
                    }
                );
                Assert.Throws<System.FormatException>(
                    delegate
                    {
                        VersionParser.ParseUnityVersion("5.6f1");
                    }
                );
            }

            [Test]
            public void ParseCorrectUpliftVersions()
            {
                Assert.AreEqual(VersionParser.ParseIncompleteVersion("0.9.1"),
                    new Version
                    {
                        Major = 0,
                        Minor = 9,
                        Patch = 1
                    }
                );
                Assert.AreEqual(VersionParser.ParseIncompleteVersion("1.0.0beta3"),
                    new Version
                    {
                        Major = 1,
                        Minor = 0,
                        Patch = 0,
                        Optional = 3
                    }
                );
            }

            [Test]
            public void CompareUpliftReleases()
            {
                Assert.IsTrue(VersionParser.GreaterThan("3.0.0", "2.0.0"));
            }

            [Test]
            public void CompareUpliftBetaReleases()
            {
                Assert.IsTrue(VersionParser.GreaterThan("1.0.0beta5", "1.0.0beta4", incomplete: true));
                Assert.IsFalse(VersionParser.GreaterThan("1.0.0beta5", "1.0.0beta4", incomplete: false));
            }
        }
    }
}
#endif