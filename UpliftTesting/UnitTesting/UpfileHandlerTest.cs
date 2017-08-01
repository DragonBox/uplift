using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Serialization;
using Uplift;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using UpliftTesting.Helpers;

namespace UpliftTesting.UnitTesting
{
    [TestFixture]
    class UpfileHandlerTest
    {
        private UpfileHandler ufh;
        private string pwd;
        private string temp_dir;

        [OneTimeSetUp]
        protected void FixtureInit()
        {
            Uplift.TestingProperties.SetLogging(false);
            pwd = Directory.GetCurrentDirectory();
            ufh = UpfileHandlerExposer.Instance();
        }

        [SetUp]
        protected void Init()
        {
            UpfileHandlerExposer.ResetSingleton();
            ufh = UpfileHandlerExposer.Instance();
            temp_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        [Test]
        public void UpfileHandlerUniqueInstanceTest()
        {
            UpfileHandler ufh_A = UpfileHandler.Instance();
            UpfileHandler ufh_B = UpfileHandler.Instance();
            Assert.AreEqual(ufh_A, ufh_B);
        }

        [Test]
        public void CheckForUpfileTest()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);
                UpfileHandlerExposer.ResetSingleton();
                ufh = UpfileHandler.Instance();

                File.Create(Path.Combine(temp_dir, "Upfile.xml")).Dispose();

                Assert.IsTrue(ufh.CheckForUpfile());
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void CheckForUpfileTestFail()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);
                Assert.IsFalse(ufh.CheckForUpfile());
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void LoadUpfileTest()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);

                UpfileHandlerExposer.ResetSingleton();
                ufh = UpfileHandler.Instance();

                Upfile dummy = new Upfile() {
                    Configuration = new Configuration(),
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };

                XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
                using (FileStream fs = new FileStream(Path.Combine(temp_dir, "Upfile.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, dummy);
                }
                
                Upfile result = ufh.LoadFile();
                Assert.AreEqual(result.Dependencies, dummy.Dependencies);
                Assert.AreEqual(result.Repositories, dummy.Repositories);
                Assert.AreEqual(result.UnityVersion, dummy.UnityVersion);
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void LoadPackagesListTest()
        {
            Upset dummy_package = new Upset()
            {
                Configuration = new InstallSpec[0],
                Dependencies = new DependencyDefinition[0],
                PackageLicense = "foo",
                PackageName = "bar",
                PackageVersion = "baz"
            };
            Mock<Repository> repo_mock = new Mock<Repository>();
            repo_mock.Setup(repo => repo.ListPackages()).Returns(new Upset[] { dummy_package });
            PackageRepo pr = new PackageRepo { Package = dummy_package, Repository = repo_mock.Object };
            Upfile dummy = new Upfile() { Repositories = new Repository[] { repo_mock.Object } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            ufh.LoadPackageList();
            CollectionAssert.Contains(PackageList.Instance().GetAllPackages(), pr);
        }
        
        [Test]
        public void GetPackagesRootPathTest()
        {
            Upfile dummy = new Upfile() { Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = "foo" } } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            Assert.AreEqual("foo", ufh.GetPackagesRootPath());
        }

        [Test]
        public void GetDestinationForBaseTest()
        {
            PathConfiguration dummy_path = new Mock<PathConfiguration>().Object;
            Upfile dummy = new Upfile() { Configuration = new Configuration() { BaseInstallPath = dummy_path } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec() { Type = InstallSpecType.Base };
            Assert.AreEqual(dummy_path, ufh.GetDestinationFor(spec));
        }

        [Test]
        public void GetDestinationForDocsTest()
        {
            PathConfiguration dummy_path = new Mock<PathConfiguration>().Object;
            Upfile dummy = new Upfile() { Configuration = new Configuration() { DocsPath = dummy_path } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec() { Type = InstallSpecType.Docs };
            Assert.AreEqual(dummy_path, ufh.GetDestinationFor(spec));
        }

        [Test]
        public void GetDestinationForExamplesTest()
        {
            PathConfiguration dummy_path = new Mock<PathConfiguration>().Object;
            Upfile dummy = new Upfile() { Configuration = new Configuration() { ExamplesPath = dummy_path } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec() { Type = InstallSpecType.Examples };
            Assert.AreEqual(dummy_path, ufh.GetDestinationFor(spec));
        }

        [Test]
        public void GetDestinationForMediaTest()
        {
            PathConfiguration dummy_path = new Mock<PathConfiguration>().Object;
            Upfile dummy = new Upfile() { Configuration = new Configuration() { MediaPath = dummy_path } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec() { Type = InstallSpecType.Media };
            Assert.AreEqual(dummy_path, ufh.GetDestinationFor(spec));
        }

        [Test]
        public void GetDestinationDefaultTest()
        {
            PathConfiguration dummy_path = new Mock<PathConfiguration>().Object;
            Upfile dummy = new Upfile() { Configuration = new Configuration() { BaseInstallPath = dummy_path } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec() { };
            Assert.AreEqual(dummy_path, ufh.GetDestinationFor(spec));
        }

        [Test]
        public void GetDestinationForGenericPluginTest()
        {
            Upfile dummy = new Upfile() { Configuration = new Configuration() { PluginPath = new PathConfiguration() { Location = "foo", } } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec() {
                Platform = PlatformType.All,
                Type = InstallSpecType.Plugin
            };
            Assert.AreEqual("foo", ufh.GetDestinationFor(spec).Location);
            Assert.AreEqual(true, ufh.GetDestinationFor(spec).SkipPackageStructure);
        }

        [Test]
        public void GetDestinationForIOSPluginTest()
        {
            Upfile dummy = new Upfile() { Configuration = new Configuration() { PluginPath = new PathConfiguration() { Location = "foo", } } };
            (ufh as UpfileHandlerExposer).SetUpfile(dummy);
            InstallSpec spec = new InstallSpec()
            {
                Platform = PlatformType.iOS,
                Type = InstallSpecType.Plugin
            };
            Assert.AreEqual(Path.Combine("foo", "ios"), ufh.GetDestinationFor(spec).Location);
            Assert.AreEqual(true, ufh.GetDestinationFor(spec).SkipPackageStructure);
        }
    }
}
