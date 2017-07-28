using Uplift.Strategies;
using Uplift.Schemas;
using NUnit.Framework;
using Moq;
using Uplift.Common;
using System;

namespace UpliftTesting.UnitTesting
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