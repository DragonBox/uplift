using System;
using NUnit.Framework;
using Moq;
using Uplift.Schemas;
using Uplift.Packages;
using UpliftTesting.Helpers;
using Uplift.Common;
using Uplift.Strategies;

namespace UpliftTesting.UnitTesting
{
    [TestFixture]
    public class PackageHandlerTest
    {
        private string[] string_versions;
        private PackageHandler.VersionStruct[] versions;
        private PackageHandler package_handler;
        private DependencyDefinition dependency_mock;
        private Upset package_mock;

        [SetUp]
        protected void Setup()
        {
            string_versions = new string[] {
                "0.0.0",
                "0.0.1",
                "0.1.1",
                "1.1.1"
            };
            versions = new PackageHandler.VersionStruct[] {
                new PackageHandler.VersionStruct { Major = 0, Minor = 0, Version = 0 },
                new PackageHandler.VersionStruct { Major = 0, Minor = 0, Version = 1 },
                new PackageHandler.VersionStruct { Major = 0, Minor = 1, Version = 0 },
                new PackageHandler.VersionStruct { Major = 0, Minor = 1, Version = 1 },
                new PackageHandler.VersionStruct { Major = 1, Minor = 0, Version = 0 },
                new PackageHandler.VersionStruct { Major = 1, Minor = 0, Version = 1 },
                new PackageHandler.VersionStruct { Major = 1, Minor = 1, Version = 0 },
                new PackageHandler.VersionStruct { Major = 1, Minor = 1, Version = 1 }
            };
            package_handler = new PackageHandlerTester();
            dependency_mock = new DependencyDefinition();
            dependency_mock.Version = "1.1.1";
            package_mock = new Upset();
            package_mock.PackageVersion = "1.1.1";
            Uplift.TestingProperties.SetLogging(false);
        }

        [Test]
        public void FindCandidateForDefinitionTest()
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
            PackageList.Instance().LoadPackages(repo_mock.Object);
            DependencyDefinition dependency = new DependencyDefinition();
            dependency.Name = "package 1";
            dependency.Version = "1.0.0";

            PackageRepo[] result = package_handler.FindCandidatesForDefinition(dependency);

            CollectionAssert.Contains(result, new PackageRepo { Package = package_dummy_2, Repository = repo_mock.Object });
            CollectionAssert.DoesNotContain(result, new PackageRepo { Package = package_dummy_1, Repository = repo_mock.Object });
            CollectionAssert.DoesNotContain(result, new PackageRepo { Package = package_dummy_3, Repository = repo_mock.Object });
        }

        [Test]
        public void FindPackageAndRepositoryTest()
        {
            Mock<Repository> repo_mock = new Mock<Repository>();
            Upset package_dummy_1 = new Upset();
            package_dummy_1.PackageName = "target package";
            package_dummy_1.PackageVersion = "0.0.0";
            Upset package_dummy_2 = new Upset();
            package_dummy_2.PackageName = "target package";
            package_dummy_2.PackageVersion = "1.1.1";
            Upset package_dummy_3 = new Upset();
            package_dummy_3.PackageName = "not target package";
            package_dummy_3.PackageVersion = "2.0.0";

            repo_mock.Setup(repo => repo.ListPackages()).Returns(new Upset[] { package_dummy_1, package_dummy_2, package_dummy_3 });
            PackageList.Instance().LoadPackages(repo_mock.Object);
            DependencyDefinition dependency = new DependencyDefinition();
            dependency.Name = "target package";
            dependency.Version = "1.0.0";

            PackageRepo result = package_handler.FindPackageAndRepository(dependency);
            Assert.AreEqual(result, new PackageRepo { Package = package_dummy_2, Repository = repo_mock.Object });
        }

        [Test]
        public void FindPackageAndRepositoryTestFail()
        {
            DependencyDefinition dependency = new DependencyDefinition();
            dependency.Name = "package missing";
            dependency.Version = "1.0.0";

            PackageRepo result = package_handler.FindPackageAndRepository(dependency);
            // An empty PackageRepo is expected for now, but maybe we should have a failure here
            Assert.AreEqual(result, new PackageRepo());
        }

        [Test]
        public void SelectCandidatesSingleStrategy()
        {
            Mock<Repository> repo_dummy = new Mock<Repository>();
            Mock<Upset> package_dummy = new Mock<Upset>();
            PackageRepo pr_dummy_1 = new PackageRepo
            {
                Package = new Upset()
                {
                    PackageName = "package 1",
                    PackageVersion = "0.0.0"
                },
                Repository = repo_dummy.Object
            };
            PackageRepo pr_dummy_2 = new PackageRepo
            {
                Package = new Upset()
                {
                    PackageName = "package 1",
                    PackageVersion = "1.1.1"
                },
                Repository = repo_dummy.Object
            };
            PackageRepo pr_dummy_3 = new PackageRepo
            {
                Package = new Upset()
                {
                    PackageName = "not package 1",
                    PackageVersion = "2.2.2"
                },
                Repository = repo_dummy.Object
            };
            PackageRepo[] pr_list = new PackageRepo[] { pr_dummy_1, pr_dummy_2, pr_dummy_3 };
            Mock<CandidateSelectionStrategy> css_mock = new Mock<CandidateSelectionStrategy>();
            css_mock.Setup(css => css.Filter(pr_list)).Returns(new PackageRepo[] { pr_dummy_2 });
            PackageRepo[] result = package_handler.SelectCandidates(pr_list, css_mock.Object);
            css_mock.Verify(css => css.Filter(pr_list));
            CollectionAssert.DoesNotContain(result, pr_dummy_1);
            CollectionAssert.Contains(result, pr_dummy_2);
            CollectionAssert.DoesNotContain(result, pr_dummy_3);
        }

        [Test]
        public void SelectCandidatesMultipleStrategies()
        {
            Mock<Repository> repo_dummy = new Mock<Repository>();
            Mock<Upset> package_dummy = new Mock<Upset>();
            PackageRepo pr_dummy_1 = new PackageRepo
            {
                Package = new Upset()
                {
                    PackageName = "package 1",
                    PackageVersion = "0.0.0"
                },
                Repository = repo_dummy.Object
            };
            PackageRepo pr_dummy_2 = new PackageRepo
            {
                Package = new Upset()
                {
                    PackageName = "package 1",
                    PackageVersion = "1.1.1"
                },
                Repository = repo_dummy.Object
            };
            PackageRepo pr_dummy_3 = new PackageRepo
            {
                Package = new Upset()
                {
                    PackageName = "not package 1",
                    PackageVersion = "2.2.2"
                },
                Repository = repo_dummy.Object
            };
            PackageRepo[] pr_list = new PackageRepo[] { pr_dummy_1, pr_dummy_2, pr_dummy_3 };
            Mock<CandidateSelectionStrategy> css_mock_A = new Mock<CandidateSelectionStrategy>();
            Mock<CandidateSelectionStrategy> css_mock_B = new Mock<CandidateSelectionStrategy>();
            css_mock_A.Setup(css => css.Filter(pr_list)).Returns(new PackageRepo[] { pr_dummy_1 });
            css_mock_B.Setup(css => css.Filter(pr_list)).Returns(new PackageRepo[] { pr_dummy_2 });
            PackageRepo[] result = package_handler.SelectCandidates(pr_list, new CandidateSelectionStrategy[] { css_mock_A.Object, css_mock_B.Object });
            css_mock_A.Verify(css => css.Filter(pr_list));
            css_mock_B.Verify(css => css.Filter(pr_list));
            CollectionAssert.Contains(result, pr_dummy_1);
            CollectionAssert.Contains(result, pr_dummy_2);
            CollectionAssert.DoesNotContain(result, pr_dummy_3);
        }

        [Test]
		public void VersionStringParsingTest()
		{
			PackageHandler.VersionStruct[] expected_versions = new PackageHandler.VersionStruct[] {
				new PackageHandler.VersionStruct { Major = 0, Minor = 0, Version = 0 },
				new PackageHandler.VersionStruct { Major = 0, Minor = 0, Version = 1 },
				new PackageHandler.VersionStruct { Major = 0, Minor = 1, Version = 1 },
				new PackageHandler.VersionStruct { Major = 1, Minor = 1, Version = 1 }
			};
			for(int i = 0; i < 4; i++) {
				Assert.AreEqual(expected_versions[i], PackageHandler.ParseVersion(string_versions[i]));
			}
        }

        [Test]
        public void VersionParsingFailureTest()
        {
            Assert.AreEqual(
                new PackageHandler.VersionStruct { Major = 0, Minor = 0, Version = 0 },
                PackageHandler.ParseVersion("not a version string")
            );
        }

        [Test]
		public void VersionComparisonTest()
		{
			PackageHandlerTester tester = package_handler as PackageHandlerTester;
			Assert.AreEqual(
				new PackageHandler.CompareResult {
					Major = PackageHandler.Comparison.LOWER,
					Minor = PackageHandler.Comparison.NA,
					Version = PackageHandler.Comparison.NA
				},
				tester.CompareVersionsWrapper(versions[0],versions[4])
			);
			Assert.AreEqual(
				new PackageHandler.CompareResult {
					Major = PackageHandler.Comparison.SAME,
					Minor = PackageHandler.Comparison.SAME,
					Version = PackageHandler.Comparison.LOWER
				},
				tester.CompareVersionsWrapper(versions[0],versions[1])
			);
			Assert.AreEqual(
				new PackageHandler.CompareResult {
					Major = PackageHandler.Comparison.SAME,
					Minor = PackageHandler.Comparison.SAME,
					Version = PackageHandler.Comparison.SAME
				},
				tester.CompareVersionsWrapper(versions[7],versions[7])
			);
			Assert.AreEqual(
				new PackageHandler.CompareResult {
					Major = PackageHandler.Comparison.SAME,
					Minor = PackageHandler.Comparison.HIGHER,
					Version = PackageHandler.Comparison.NA
				},
				tester.CompareVersionsWrapper(versions[2],versions[0])
			);
			Assert.AreEqual(
				new PackageHandler.CompareResult {
					Major = PackageHandler.Comparison.HIGHER,
					Minor = PackageHandler.Comparison.NA,
					Version = PackageHandler.Comparison.NA
				},
				tester.CompareVersionsWrapper(versions[4],versions[3])
			);
            Assert.AreEqual(
                new PackageHandler.CompareResult
                {
                    Major = PackageHandler.Comparison.SAME,
                    Minor = PackageHandler.Comparison.LOWER,
                    Version = PackageHandler.Comparison.NA
                },
                tester.CompareVersionsWrapper(versions[1], versions[2])
            );
            Assert.AreEqual(
                new PackageHandler.CompareResult
                {
                    Major = PackageHandler.Comparison.SAME,
                    Minor = PackageHandler.Comparison.SAME,
                    Version = PackageHandler.Comparison.HIGHER
                },
                tester.CompareVersionsWrapper(versions[7], versions[6])
            );
        }

        [Test]
        public void PackageDependencyVersionComparison()
        {
            PackageHandlerTester tester = package_handler as PackageHandlerTester;

            Assert.AreEqual(
                new PackageHandler.CompareResult
                {
                    Major = PackageHandler.Comparison.SAME,
                    Minor = PackageHandler.Comparison.SAME,
                    Version = PackageHandler.Comparison.SAME
                },
                tester.CompareVersionsWrapper(package_mock, dependency_mock)
            );
        }
	}
}

