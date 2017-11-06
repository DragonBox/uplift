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
using Uplift.Strategies;
using Uplift.Schemas;
using NUnit.Framework;
using Moq;
using Uplift.Common;
using System;

namespace Uplift.Testing.Unit
{
    [TestFixture]
    public class LatestSelectionStrategyTest
    {
        [Test]
        public void FilterTest()
        {
            CandidateSelectionStrategy css = new LatestSelectionStrategy();
            Mock<Repository> repo_mock = new Mock<Repository>();
            Upset package_A = new Upset();
            package_A.PackageVersion = "0.0.0";
            Upset package_B = new Upset();
            package_B.PackageVersion = "1.1.1";
            PackageRepo pr_A = new PackageRepo { Package = package_A, Repository = repo_mock.Object };
            PackageRepo pr_B = new PackageRepo { Package = package_B, Repository = repo_mock.Object };

            PackageRepo[] result = css.Filter(new PackageRepo[] { pr_A, pr_B });

            Assert.AreEqual(result[0], pr_B);
        }
    }
}
#endif
