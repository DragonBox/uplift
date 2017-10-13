using NUnit.Framework;
using Uplift.Common;

namespace Uplift.Testing.Unit
{
    namespace VersionRequirementTest
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
                Assert.AreEqual((parsed as LoseVersionRequirement).stub, new Version { Major = 1, Minor = 2 });
            }

            [Test]
            public void ParseBoundedRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2.*");
                Assert.IsTrue(parsed is BoundedVersionRequirement);
                Assert.AreEqual((parsed as BoundedVersionRequirement).lowerBound, new Version { Major = 1, Minor = 2 });
            }

            [Test]
            public void ParseExactRequirement()
            {
                IVersionRequirement parsed = VersionParser.ParseRequirement("1.2.3!");
                Assert.IsTrue(parsed is ExactVersionRequirement);
                Assert.AreEqual((parsed as ExactVersionRequirement).expected, new Version { Major = 1, Minor = 2, Patch = 3 });
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
        }

        [TestFixture]
        class NoRequirementTest
        {
            IVersionRequirement requirement;

            [OneTimeSetUp]
            protected void Given()
            {
                requirement = new NoRequirement();
            }

            [Test]
            public void IsMetBy()
            {
                Assert.IsTrue(requirement.IsMetBy("0.0.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.0"));
                Assert.IsTrue(requirement.IsMetBy("2.0.0"));
            }

            [Test]
            public void ResrictToNoRequirement()
            {
                NoRequirement noRequirement = new NoRequirement();
                Assert.AreSame(requirement.RestrictTo(noRequirement), noRequirement);
            }

            [Test]
            public void ResrictToMinimalRequirement()
            {
                MinimalVersionRequirement minimalRequirement = new MinimalVersionRequirement("1.0");
                Assert.AreSame(requirement.RestrictTo(minimalRequirement), minimalRequirement);
            }

            [Test]
            public void RestrictToLoseRequirement()
            {
                LoseVersionRequirement loseRequirement = new LoseVersionRequirement("1.0");
                Assert.AreSame(requirement.RestrictTo(loseRequirement), loseRequirement);
            }

            [Test]
            public void RestrictToBoundedRequirement()
            {
                BoundedVersionRequirement boundedRequirement = new BoundedVersionRequirement("1.0");
                Assert.AreSame(requirement.RestrictTo(boundedRequirement), boundedRequirement);
            }

            [Test]
            public void RestrictToExactRequirement()
            {
                ExactVersionRequirement exactRequirement = new ExactVersionRequirement("1.0");
                Assert.AreSame(requirement.RestrictTo(exactRequirement), exactRequirement);
            }
        }

        [TestFixture]
        class MinimalVersionRequirementTest
        {
            IVersionRequirement requirement;

            [OneTimeSetUp]
            protected void Given()
            {
                // 1.0+
                requirement = new MinimalVersionRequirement("1.0");
            }

            [Test]
            public void IsMetBy()
            {
                Assert.IsFalse(requirement.IsMetBy("0.0.0"));
                Assert.IsFalse(requirement.IsMetBy("0.9.9"));
                Assert.IsTrue(requirement.IsMetBy("1.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.1"));
                Assert.IsTrue(requirement.IsMetBy("2.0.0"));
                Assert.IsTrue(requirement.IsMetBy("2"));
            }

            [Test]
            public void ResrictToNoRequirement()
            {
                NoRequirement noRequirement = new NoRequirement();
                Assert.AreSame(requirement.RestrictTo(noRequirement), requirement);
            }

            [Test]
            public void ResrictToMinimalRequirement()
            {
                MinimalVersionRequirement minimalRequirement;
                // When greater (1.1+)
                minimalRequirement = new MinimalVersionRequirement("1.1");
                Assert.AreSame(requirement.RestrictTo(minimalRequirement), minimalRequirement);

                // When lesser (0.9+)
                minimalRequirement = new MinimalVersionRequirement("0.9");
                Assert.AreSame(requirement.RestrictTo(minimalRequirement), requirement);
            }

            [Test]
            public void RestrictToLoseRequirement()
            {
                LoseVersionRequirement loseRequirement;
                // When greater (1.1)
                loseRequirement = new LoseVersionRequirement("1.1");
                Assert.AreSame(requirement.RestrictTo(loseRequirement), loseRequirement);

                // When lesser (0.9)
                loseRequirement = new LoseVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToBoundedRequirement()
            {
                BoundedVersionRequirement boundedRequirement;
                // When greater (1.1.*)
                boundedRequirement = new BoundedVersionRequirement("1.1");
                Assert.AreSame(requirement.RestrictTo(boundedRequirement), boundedRequirement);

                // When lesser (0.9.*)
                boundedRequirement = new BoundedVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToExactRequirement()
            {
                ExactVersionRequirement exactRequirement;
                // When greater (1.1.0!)
                exactRequirement = new ExactVersionRequirement("1.1.0");
                Assert.AreSame(requirement.RestrictTo(exactRequirement), exactRequirement);

                // When lesser (0.9.9!)
                exactRequirement = new ExactVersionRequirement("0.9.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );
            }
        }

        [TestFixture]
        class LoseVersionRequirementTest
        {
            IVersionRequirement requirement;

            [OneTimeSetUp]
            protected void Given()
            {
                // 1.0
                requirement = new LoseVersionRequirement("1.0");
            }

            [Test]
            public void IsMetBy()
            {
                Assert.IsFalse(requirement.IsMetBy("0.0.0"));
                Assert.IsFalse(requirement.IsMetBy("0.9.9"));
                Assert.IsTrue(requirement.IsMetBy("1.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.1"));
                Assert.IsFalse(requirement.IsMetBy("2.0.0"));
            }

            [Test]
            public void ResrictToNoRequirement()
            {
                NoRequirement noRequirement = new NoRequirement();
                Assert.AreSame(requirement.RestrictTo(noRequirement), requirement);
            }

            [Test]
            public void ResrictToMinimalRequirement()
            {
                MinimalVersionRequirement minimalRequirement;
                // When greater (1.1+)
                minimalRequirement = new MinimalVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(minimalRequirement);
                    }
                );

                // When lesser (0.9+)
                minimalRequirement = new MinimalVersionRequirement("0.9");
                Assert.AreSame(requirement.RestrictTo(minimalRequirement), requirement);
            }

            [Test]
            public void RestrictToLoseRequirement()
            {
                LoseVersionRequirement loseRequirement;
                // When strictly greater (1.1)
                loseRequirement = new LoseVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );
                // When losely greater (1.0.1)
                loseRequirement = new LoseVersionRequirement("1.0.1");
                Assert.AreSame(requirement.RestrictTo(loseRequirement), loseRequirement);


                // When lesser (0.9)
                loseRequirement = new LoseVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToBoundedRequirement()
            {
                BoundedVersionRequirement boundedRequirement;
                // When greater (1.1.*)
                boundedRequirement = new BoundedVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );

                // When comparable (1.0.*)
                boundedRequirement = new BoundedVersionRequirement("1.0");
                Assert.AreSame(requirement.RestrictTo(boundedRequirement), boundedRequirement);

                // When lesser (0.9.*)
                boundedRequirement = new BoundedVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToExactRequirement()
            {
                ExactVersionRequirement exactRequirement;
                // When greater (1.1.0!)
                exactRequirement = new ExactVersionRequirement("1.1.0");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );

                // When comparable (1.0.7!)
                exactRequirement = new ExactVersionRequirement("1.0.7");
                Assert.AreSame(requirement.RestrictTo(exactRequirement), exactRequirement);

                // When lesser (0.9.9!)
                exactRequirement = new ExactVersionRequirement("0.9.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );
            }
        }

        [TestFixture]
        class BoundedVersionRequirementTest
        {
            IVersionRequirement requirement;

            [OneTimeSetUp]
            protected void Given()
            {
                // 1.0.*
                requirement = new BoundedVersionRequirement("1.0");
            }

            [Test]
            public void IsMetBy()
            {
                Assert.IsFalse(requirement.IsMetBy("0.0.0"));
                Assert.IsFalse(requirement.IsMetBy("0.9.9"));
                Assert.IsFalse(requirement.IsMetBy("1.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.0"));
                Assert.IsTrue(requirement.IsMetBy("1.0.1"));
                Assert.IsFalse(requirement.IsMetBy("2.0.0"));
            }

            [Test]
            public void ResrictToNoRequirement()
            {
                NoRequirement noRequirement = new NoRequirement();
                Assert.AreSame(requirement.RestrictTo(noRequirement), requirement);
            }

            [Test]
            public void ResrictToMinimalRequirement()
            {
                MinimalVersionRequirement minimalRequirement;
                // When greater (1.1+)
                minimalRequirement = new MinimalVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(minimalRequirement);
                    }
                );

                // When lesser (0.9+)
                minimalRequirement = new MinimalVersionRequirement("0.9");
                Assert.AreSame(requirement.RestrictTo(minimalRequirement), requirement);
            }

            [Test]
            public void RestrictToLoseRequirement()
            {
                LoseVersionRequirement loseRequirement;
                // When strictly greater (1.1)
                loseRequirement = new LoseVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );

                // When losely greater (1.0.1)
                loseRequirement = new LoseVersionRequirement("1.0.1");
                Assert.AreSame(requirement.RestrictTo(loseRequirement), loseRequirement);


                // When lesser (0.9)
                loseRequirement = new LoseVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToBoundedRequirement()
            {
                BoundedVersionRequirement boundedRequirement;
                // When greater (1.1.*)
                boundedRequirement = new BoundedVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );

                // When included (1.0.1.*)
                boundedRequirement = new BoundedVersionRequirement("1.0.1");
                Assert.AreSame(requirement.RestrictTo(boundedRequirement), boundedRequirement);

                // When lesser (0.9.*)
                boundedRequirement = new BoundedVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToExactRequirement()
            {
                ExactVersionRequirement exactRequirement;
                // When greater (1.1.0!)
                exactRequirement = new ExactVersionRequirement("1.1.0");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );

                // When comparable (1.0.7!)
                exactRequirement = new ExactVersionRequirement("1.0.7");
                Assert.AreSame(requirement.RestrictTo(exactRequirement), exactRequirement);

                // When lesser (0.9.9!)
                exactRequirement = new ExactVersionRequirement("0.9.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );
            }
        }

        [TestFixture]
        class ExactVersionRequirementTest
        {
            IVersionRequirement requirement;

            [OneTimeSetUp]
            protected void Given()
            {
                // 1.0!
                requirement = new ExactVersionRequirement("1.0");
            }

            [Test]
            public void IsMetBy()
            {
                Assert.IsFalse(requirement.IsMetBy("0.0.0"));
                Assert.IsFalse(requirement.IsMetBy("0.9.9"));
                Assert.IsFalse(requirement.IsMetBy("1.0.6.9"));
                Assert.IsTrue(requirement.IsMetBy("1.0"));
                Assert.IsFalse(requirement.IsMetBy("1.0.1"));
                Assert.IsFalse(requirement.IsMetBy("2.0.0"));
            }

            [Test]
            public void ResrictToNoRequirement()
            {
                NoRequirement noRequirement = new NoRequirement();
                Assert.AreSame(requirement.RestrictTo(noRequirement), requirement);
            }

            [Test]
            public void ResrictToMinimalRequirement()
            {
                MinimalVersionRequirement minimalRequirement;
                // When greater (1.1+)
                minimalRequirement = new MinimalVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(minimalRequirement);
                    }
                );

                // When lesser (0.9+)
                minimalRequirement = new MinimalVersionRequirement("0.9");
                Assert.AreSame(requirement.RestrictTo(minimalRequirement), requirement);
            }

            [Test]
            public void RestrictToLoseRequirement()
            {
                LoseVersionRequirement loseRequirement;
                // When strictly greater (1.1)
                loseRequirement = new LoseVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );

                // When wider (1.0)
                loseRequirement = new LoseVersionRequirement("1");
                Assert.AreSame(requirement.RestrictTo(loseRequirement), requirement);


                // When lesser (0.9)
                loseRequirement = new LoseVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(loseRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToBoundedRequirement()
            {
                BoundedVersionRequirement boundedRequirement;
                // When greater (1.1.*)
                boundedRequirement = new BoundedVersionRequirement("1.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );

                // When wider (1.*)
                boundedRequirement = new BoundedVersionRequirement("1");
                Assert.AreSame(requirement.RestrictTo(boundedRequirement), requirement);

                // When lesser (0.9.*)
                boundedRequirement = new BoundedVersionRequirement("0.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(boundedRequirement);
                    }
                );
            }

            [Test]
            public void RestrictToExactRequirement()
            {
                ExactVersionRequirement exactRequirement;
                // When greater (1.0.1!)
                exactRequirement = new ExactVersionRequirement("1.0.1");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );

                // When same (1.0!)
                exactRequirement = new ExactVersionRequirement("1.0");
                Assert.AreSame(requirement.RestrictTo(exactRequirement), requirement);

                // When lesser (0.9.9!)
                exactRequirement = new ExactVersionRequirement("0.9.9");
                Assert.Throws<IncompatibleRequirementException>(
                    delegate
                    {
                        requirement.RestrictTo(exactRequirement);
                    }
                );
            }
        }
    }
}
