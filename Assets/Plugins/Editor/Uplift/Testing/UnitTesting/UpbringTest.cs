#if UNITY_5_3_OR_NEWER
using Uplift.Schemas;
using NUnit.Framework;
using System.IO;
using System.Xml.Serialization;
using Uplift;
using System.Linq;
using Uplift.Testing.Helpers;

namespace Uplift.Testing.Unit
{
    [TestFixture]
    class UpbringTest
    {
        private Upbring upbring;
        private string pwd;
        private string temp_dir;

        [OneTimeSetUp]
        protected void FixtureInit()
        {
            TestingProperties.SetLogging(false);
            pwd = Directory.GetCurrentDirectory();
        }

        [SetUp]
        protected void Init()
        {
            upbring = new Upbring();
            temp_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            UpfileExposer.ClearInstance();
        }

        [Test]
        public void GetInstalledPackageTest()
        {
            InstalledPackage package_A = new InstalledPackage() { Name = "packageA" };
            InstalledPackage package_B = new InstalledPackage() { Name = "packageB" };
            upbring.InstalledPackage = new InstalledPackage[] { package_A, package_B };
            Assert.AreEqual(upbring.GetInstalledPackage("packageA"), package_A);
        }

        [Test]
        public void AddPackageTest()
        {
            InstalledPackage package_A = new InstalledPackage() { Name = "packageA" };
            upbring.InstalledPackage = new InstalledPackage[] { package_A };
            upbring.AddPackage(new Upset() {
                PackageName = "packageB",
                PackageVersion = "foo"
            });

            Assert.IsTrue(upbring.InstalledPackage.Any(pack => pack.Name == "packageA"), "Adding a package overwrote the existing");
            Assert.IsTrue(upbring.InstalledPackage.Any(pack => pack.Name == "packageB"), "The package has not been added");
        }

        [Test]
        public void AddExistingPackageTest()
        {
            InstalledPackage package_A = new InstalledPackage() { Name = "packageA" };
            upbring.InstalledPackage = new InstalledPackage[] { package_A };
            upbring.AddPackage(new Upset()
            {
                PackageName = "packageA",
                PackageVersion = "foo"
            });

            Assert.AreEqual(1, upbring.InstalledPackage.Where(pack => pack.Name == "packageA").ToArray().Length, "The package was duplicated");
        }

        [Test]
        public void AddLocationWhenNoneTest()
        {
            InstalledPackage package_A = new InstalledPackage()
            {
                Name = "packageA"
            };
            upbring.InstalledPackage = new InstalledPackage[] { package_A };
            upbring.AddLocation(new Upset()
            {
                PackageName = "packageA",
                PackageVersion = "0.0.0"
            }, InstallSpecType.Base, "foo");

            Assert.AreEqual(1, upbring.InstalledPackage[0].Install.Length, "The new installation hasn't been registered");
        }

        [Test]
        public void AddLocationWhenAnotherTest()
        {
            InstalledPackage package_A = new InstalledPackage()
            {
                Name = "packageA",
                Install = new InstallSpec[] { new InstallSpecPath() { Path = "foo", Type = InstallSpecType.Base } }
            };
            upbring.InstalledPackage = new InstalledPackage[] { package_A };
            upbring.AddLocation(new Upset()
            {
                PackageName = "packageA",
                PackageVersion = "0.0.0"
            }, InstallSpecType.Docs, "bar");

            Assert.AreEqual(2, upbring.InstalledPackage[0].Install.Length, "The new installation hasn't been registered");
        }

        [Test]
        public void AddLocationWhenExistingTest()
        {
            InstalledPackage package_A = new InstalledPackage()
            {
                Name = "packageA",
                Install = new InstallSpec[] { new InstallSpecPath() { Path = "foo", Type = InstallSpecType.Base } }
            };
            upbring.InstalledPackage = new InstalledPackage[] { package_A };
            upbring.AddLocation(new Upset()
            {
                PackageName = "packageA",
                PackageVersion = "0.0.0"
            }, InstallSpecType.Base, "foo");

            Assert.AreEqual(1, upbring.InstalledPackage[0].Install.Length, "The location has been duplicated");
        }
        
        [Test]
        public void AddLocationNoMatchingPackage()
        {
            InstalledPackage package_A = new InstalledPackage()
            {
                Name = "packageA",
                Install = new InstallSpec[0]
            };
            upbring.InstalledPackage = new InstalledPackage[] { package_A };
            upbring.AddLocation(new Upset()
            {
                PackageName = "packageB",
                PackageVersion = "0.0.0"
            }, InstallSpecType.Docs, "bar");

            CollectionAssert.IsEmpty(upbring.InstalledPackage[0].Install, "The location has been added, but it shouldn't have as it doesn't match");
        }

        [Test]
        public void FromXmlWhenPresentTest()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);
                string repo_path = Path.Combine(temp_dir, Path.GetRandomFileName());
                Directory.CreateDirectory(repo_path);

                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                UpfileExposer.SetInstance(dummy);

                InstalledPackage package_A = new InstalledPackage() { Name = "packageA", Install = new InstallSpec[0], Version = "0.0.0" };
                Upbring test = new Upbring() { InstalledPackage = new InstalledPackage[] { package_A } };
                XmlSerializer serializer = new XmlSerializer(typeof(Upbring));
                using (FileStream fs = new FileStream(Path.Combine(repo_path, "Upbring.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, test);
                }
                Upbring.InitializeInstance();

                Assert.That(Upbring.Instance().InstalledPackage.Any(p =>
                p.Name == package_A.Name &&
                p.Version == package_A.Version
                ));
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void FromXmlWhenAbsentTest()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);
                string repo_path = Path.Combine(temp_dir, Path.GetRandomFileName());
                Directory.CreateDirectory(repo_path);

                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                UpfileExposer.SetInstance(dummy);

                Upbring.InitializeInstance();

                CollectionAssert.IsEmpty(Upbring.Instance().InstalledPackage);
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void SaveFileTest()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);

                string repo_path = Path.Combine(temp_dir, Path.GetRandomFileName());
                Directory.CreateDirectory(repo_path);

                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                UpfileExposer.SetInstance(dummy);

                InstalledPackage package_A = new InstalledPackage() { Name = "packageA", Install = new InstallSpec[0], Version = "0.0.0" };
                Upbring test = new Upbring() { InstalledPackage = new InstalledPackage[] { package_A } };

                test.SaveFile();

                XmlSerializer serializer = new XmlSerializer(typeof(Upbring));
                Upbring parsed;
                using (FileStream fs = new FileStream(Path.Combine(repo_path, "Upbring.xml"), FileMode.Open))
                {
                    parsed = serializer.Deserialize(fs) as Upbring;
                }

                Assert.That(parsed.InstalledPackage.Any(p =>
                p.Name == test.InstalledPackage[0].Name &&
                p.Version == test.InstalledPackage[0].Version
                ));
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void RemoveFileTest()
        {
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);

                string repo_path = Path.Combine(temp_dir, Path.GetRandomFileName());
                Directory.CreateDirectory(repo_path);

                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                UpfileExposer.SetInstance(dummy);

                string upbring_path = Path.Combine(repo_path, "Upbring.xml");
                File.Create(upbring_path).Dispose();                

                Upbring.RemoveFile();
                Assert.IsFalse(File.Exists(upbring_path));
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }
    }
}
#endif
