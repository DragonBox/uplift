using Uplift.Common;
using Uplift.Schemas;
using NUnit.Framework;
using Moq;
using System.IO;
using System.Xml.Serialization;
using Uplift;
using System;
using System.Linq;

namespace UpliftTesting.UnitTesting
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
                Install = new InstallSpec[] { new InstallSpec() { Path = "foo", Type = InstallSpecType.Base } }
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
                Install = new InstallSpec[] { new InstallSpec() { Path = "foo", Type = InstallSpecType.Base } }
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
                string repo_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                UpfileHandler ufh = UpfileHandler.Instance();
                Directory.CreateDirectory(repo_path);
                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
                using (FileStream fs = new FileStream(Path.Combine(temp_dir, "Upfile.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, dummy);
                }
                ufh.InternalLoadFile();
                InstalledPackage package_A = new InstalledPackage() { Name = "packageA", Install = new InstallSpec[0], Version = "0.0.0" };
                Upbring test = new Upbring() { InstalledPackage = new InstalledPackage[] { package_A } };
                serializer = new XmlSerializer(typeof(Upbring));
                using (FileStream fs = new FileStream(Path.Combine(repo_path, "Upbring.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, test);
                }
                Assert.AreEqual(package_A.Name, Upbring.FromXml().InstalledPackage[0].Name);
                //Assert.AreEqual(package_A.Install, Upbring.FromXml().InstalledPackage[0].Install);
                Assert.AreEqual(package_A.Version, Upbring.FromXml().InstalledPackage[0].Version);
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
                string repo_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                UpfileHandler ufh = UpfileHandler.Instance();
                Directory.CreateDirectory(repo_path);
                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
                using (FileStream fs = new FileStream(Path.Combine(temp_dir, "Upfile.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, dummy);
                }
                ufh.InternalLoadFile();

                CollectionAssert.IsEmpty(Upbring.FromXml().InstalledPackage);
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

                // UpfileHandler Setup
                string repo_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                UpfileHandler ufh = UpfileHandler.Instance();
                Directory.CreateDirectory(repo_path);
                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
                using (FileStream fs = new FileStream(Path.Combine(temp_dir, "Upfile.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, dummy);
                }
                ufh.InternalLoadFile();

                InstalledPackage package_A = new InstalledPackage() { Name = "packageA", Install = new InstallSpec[0], Version = "0.0.0" };
                Upbring test = new Upbring() { InstalledPackage = new InstalledPackage[] { package_A } };

                test.SaveFile();

                serializer = new XmlSerializer(typeof(Upbring));
                Upbring parsed;
                using (FileStream fs = new FileStream(Path.Combine(repo_path, "Upbring.xml"), FileMode.Open))
                {
                    parsed = serializer.Deserialize(fs) as Upbring;
                }

                Assert.AreEqual(test.InstalledPackage[0].Name, parsed.InstalledPackage[0].Name);
                Assert.AreEqual(test.InstalledPackage[0].Version, parsed.InstalledPackage[0].Version);
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

                // UpfileHandler Setup
                string repo_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                UpfileHandler ufh = UpfileHandler.Instance();
                Directory.CreateDirectory(repo_path);
                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = repo_path } },
                    Dependencies = new DependencyDefinition[0],
                    Repositories = new Repository[0],
                    UnityVersion = "foo"
                };
                XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
                using (FileStream fs = new FileStream(Path.Combine(temp_dir, "Upfile.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, dummy);
                }
                ufh.InternalLoadFile();

                InstalledPackage package_A = new InstalledPackage() { Name = "packageA", Install = new InstallSpec[0], Version = "0.0.0" };
                Upbring test = new Upbring() { InstalledPackage = new InstalledPackage[] { package_A } };
                string upbring_path = Path.Combine(repo_path, "Upbring.xml");
                serializer = new XmlSerializer(typeof(Upbring));
                using (FileStream fs = new FileStream(upbring_path, FileMode.Create))
                {
                    serializer.Serialize(fs, test);
                }
                Assert.IsTrue(File.Exists(upbring_path));
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
