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
    namespace VersionRequirementTest
    {
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

                // When less detailed (1)
                loseRequirement = new LoseVersionRequirement("1");
                IVersionRequirement targetRequirement = new RangeVersionRequirement("1.0", "2");
                Assert.AreEqual(requirement.RestrictTo(loseRequirement), targetRequirement);

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
                Assert.IsFalse(requirement.IsMetBy("1.1.0"));
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

                // When more specific 1.0.4+
                minimalRequirement = new MinimalVersionRequirement("1.0.4");
                // Restricts to a new, more restrictive range
                IVersionRequirement targetRequirement = new RangeVersionRequirement("1.0.4", "1.1");
                Assert.AreEqual(requirement.RestrictTo(minimalRequirement), targetRequirement);

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

                // When losely comparable (1.0)
                loseRequirement = new LoseVersionRequirement("1.0");
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
#endif
