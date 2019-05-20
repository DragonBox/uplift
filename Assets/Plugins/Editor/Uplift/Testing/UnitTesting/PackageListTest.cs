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
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using Uplift.Testing.Helpers;

namespace Uplift.Testing.Unit
{
	[TestFixture]
	public class PackageListTest
	{
		private PackageList pl;

		[SetUp]
		protected void Setup()
		{
			pl = PackageListTester.TestingInstance();
			(pl as PackageListTester).Clear();
			Uplift.TestingProperties.SetLogging(false);
		}

		[TearDown]
		protected void ClearInstance()
		{
			(pl as PackageListTester).Clear();
		}

		[Test]
		public void PackageListUniqueInstanceTest()
		{
			PackageList plA = PackageList.Instance();
			PackageList plB = PackageList.Instance();
			Assert.AreSame(plA, plB);
		}

		[Test]
		public void GetEmptyPackagesTest()
		{
			CollectionAssert.IsEmpty(pl.GetAllPackages());
		}

		[Test]
		public void LoadPackagesSingleRepoTest()
		{
			Mock<Repository> repo_mock = new Mock<Repository>();
			Mock<Upset> package_dummy = new Mock<Upset>();
			repo_mock.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy.Object });
			pl.LoadPackages(repo_mock.Object);
			Assert.AreEqual(new PackageRepo
			{
				Package = package_dummy.Object,
				Repository = repo_mock.Object
			},
				(pl as PackageListTester).GetPackages()[0]);
		}

		[Test]
		public void RefreshPackagesTest()
		{
			Mock<Repository> repo_mock = new Mock<Repository>();
			Mock<Upset> package_dummy_old = new Mock<Upset>();
			Mock<Upset> package_dummy_recent = new Mock<Upset>();
			PackageRepo old_pr = new PackageRepo
			{
				Package = package_dummy_old.Object,
				Repository = repo_mock.Object
			};
			PackageRepo recent_pr = new PackageRepo
			{
				Package = package_dummy_recent.Object,
				Repository = repo_mock.Object
			};

			repo_mock.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_recent.Object });

			(pl as PackageListTester).SetPackages(new List<PackageRepo> { old_pr });
			(pl as PackageListTester).SetRepositories(new Repository[] { repo_mock.Object });
			pl.RefreshPackages();
			List<PackageRepo> result = (pl as PackageListTester).GetPackages();
			CollectionAssert.DoesNotContain(result, old_pr);
			CollectionAssert.Contains(result, recent_pr);
		}

		[Test]
		public void LoadPackagesMultipleRepoTest()
		{
			Mock<Repository> repo_mock_A = new Mock<Repository>();
			Mock<Repository> repo_mock_B = new Mock<Repository>();
			Mock<Upset> package_dummy_A1 = new Mock<Upset>();
			Mock<Upset> package_dummy_A2 = new Mock<Upset>();
			Mock<Upset> package_dummy_B = new Mock<Upset>();
			PackageRepo pr_A1 = new PackageRepo
			{
				Package = package_dummy_A1.Object,
				Repository = repo_mock_A.Object
			};
			PackageRepo pr_A2 = new PackageRepo
			{
				Package = package_dummy_A2.Object,
				Repository = repo_mock_A.Object
			};
			PackageRepo pr_B = new PackageRepo
			{
				Package = package_dummy_B.Object,
				Repository = repo_mock_B.Object
			};
			repo_mock_A.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_A1.Object, package_dummy_A2.Object });
			repo_mock_B.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_B.Object });

			(pl as PackageListTester).SetRepositories(null);

			pl.LoadPackages(new Repository[] { repo_mock_A.Object, repo_mock_B.Object }, false);

			List<PackageRepo> result = (pl as PackageListTester).GetPackages();

			Assert.AreEqual(3, result.Count);
			CollectionAssert.Contains(result, pr_A1);
			CollectionAssert.Contains(result, pr_A2);
			CollectionAssert.Contains(result, pr_B);
		}

		[Test]
		public void LoadPackagesForceRefreshTest()
		{
			Mock<Repository> repo_mock_A = new Mock<Repository>();
			Mock<Repository> repo_mock_B = new Mock<Repository>();
			Mock<Upset> package_dummy_A1 = new Mock<Upset>();
			Mock<Upset> package_dummy_A2 = new Mock<Upset>();
			Mock<Upset> package_dummy_B = new Mock<Upset>();
			PackageRepo pr_A1 = new PackageRepo
			{
				Package = package_dummy_A1.Object,
				Repository = repo_mock_A.Object
			};
			PackageRepo pr_A2 = new PackageRepo
			{
				Package = package_dummy_A2.Object,
				Repository = repo_mock_A.Object
			};
			PackageRepo pr_B = new PackageRepo
			{
				Package = package_dummy_B.Object,
				Repository = repo_mock_B.Object
			};

			repo_mock_A.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_A1.Object, package_dummy_A2.Object });
			repo_mock_B.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_B.Object });

			(pl as PackageListTester).SetPackages(new List<PackageRepo> { pr_B });
			(pl as PackageListTester).SetRepositories(new Repository[] { repo_mock_B.Object });

			pl.LoadPackages(new Repository[] { repo_mock_A.Object }, true);
			PackageRepo[] result = pl.GetAllPackages();

			Assert.AreEqual(2, result.Length);
			CollectionAssert.Contains(result, pr_A1);
			CollectionAssert.Contains(result, pr_A2);
			CollectionAssert.DoesNotContain(result, pr_B);
		}

		[Test]
		public void GetLatestPackageTest()
		{
			Mock<Repository> repo_mock = new Mock<Repository>();
			Upset package_dummy_1 = new Upset();
			package_dummy_1.PackageName = "package 1";
			package_dummy_1.PackageVersion = "0.0.0";
			Upset package_dummy_2 = new Upset();
			package_dummy_2.PackageName = "package 1";
			package_dummy_2.PackageVersion = "1.1.1";
			Upset package_dummy_3 = new Upset();
			package_dummy_3.PackageName = "not package 1";
			package_dummy_3.PackageVersion = "2.0.0";

			repo_mock.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_1, package_dummy_2, package_dummy_3 });
			(pl as PackageListTester).SetPackages(new List<PackageRepo> {
				new PackageRepo {
					Package = package_dummy_1,
						Repository = repo_mock.Object
				},
				new PackageRepo {
					Package = package_dummy_2,
						Repository = repo_mock.Object
				},
				new PackageRepo {
					Package = package_dummy_3,
						Repository = repo_mock.Object
				}
			});
			PackageRepo result = pl.GetLatestPackage("package 1");

			Assert.AreEqual(new PackageRepo { Package = package_dummy_2, Repository = repo_mock.Object }, result);
		}
	}
}
#endif