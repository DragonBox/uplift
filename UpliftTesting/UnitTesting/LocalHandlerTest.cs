using Uplift.Packages;
using NUnit.Framework;
using Uplift.Schemas;
using System.IO;
using Uplift;
using System.Xml.Serialization;

namespace UpliftTesting.UnitTesting
{
    [TestFixture]
    class LocalHandlerTest
    {
        [Test]
        public void NukeAllPackagesTest()
        {
            string pwd = Directory.GetCurrentDirectory();
            string temp_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string installed_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string installed_path = Path.Combine(installed_dir, "target.file");
            string installed_path_meta = installed_path + ".meta";
            try
            {
                // Upbring Setup
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);
                Directory.CreateDirectory(installed_dir);
                File.Create(installed_path).Dispose();
                File.Create(installed_path_meta).Dispose();
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
                InstalledPackage package_A = new InstalledPackage()
                {
                    Name = "packageA",
                    Install = new InstallSpec[]
                    {
                        new InstallSpec()
                        {
                            Type = InstallSpecType.Base,
                            Path = installed_path
                        }
                    },
                    Version = "0.0.0"
                };
                Upbring test = new Upbring() { InstalledPackage = new InstalledPackage[] { package_A } };
                serializer = new XmlSerializer(typeof(Upbring));
                using (FileStream fs = new FileStream(Path.Combine(repo_path, "Upbring.xml"), FileMode.Create))
                {
                    serializer.Serialize(fs, test);
                }

                Assert.IsTrue(File.Exists(installed_path));
                Assert.IsTrue(File.Exists(installed_path_meta));

                LocalHandler.NukeAllPackages();

                Assert.IsFalse(File.Exists(installed_path));
                Assert.IsFalse(File.Exists(installed_path_meta));
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void GetRepositoryInstallPathTest()
        {
            string pwd = Directory.GetCurrentDirectory();
            string temp_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(temp_dir);
                Directory.SetCurrentDirectory(temp_dir);
                UpfileHandler ufh = UpfileHandler.Instance();
                Upfile dummy = new Upfile()
                {
                    Configuration = new Configuration() { RepositoryPath = new PathConfiguration() { Location = "bar" } },
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
                Upset dummy_package = new Upset()
                {
                    PackageName = "test_package",
                    PackageVersion = "1.1.1"
                };

                Assert.AreEqual(Path.Combine("bar", "test_package~1.1.1"), LocalHandler.GetRepositoryInstallPath(dummy_package));
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(temp_dir, true);
            }
        }

        [Test]
        public void GetPackageDirectory()
        {
            Upset dummy = new Upset()
            {
                PackageName = "test_package",
                PackageVersion = "1.1.1"
            };

            Assert.AreEqual("test_package~1.1.1", LocalHandler.GetPackageDirectory(dummy));
        }
    }
}
