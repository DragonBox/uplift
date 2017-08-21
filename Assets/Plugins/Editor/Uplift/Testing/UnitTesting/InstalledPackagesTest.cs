using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Serialization;
using Uplift;
using Uplift.Schemas;

namespace Uplift.Testing.Unit
{
    [TestFixture]
    class InstalledPackagesTest
    {
        [OneTimeSetUp]
        protected void FixtureInit()
        {
            Uplift.TestingProperties.SetLogging(false);
        }

        [Test]
        public void NukeRootInstallTest()
        {
            InstalledPackage ip = new InstalledPackage();
            string installed_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(installed_path);

            ip.Install = new InstallSpec[]
            {
                new InstallSpecPath()
                {
                    Type = InstallSpecType.Root,
                    Path = installed_path
                }
            };

            ip.Nuke();

            Assert.IsFalse(Directory.Exists(installed_path));

            // Make sure that the directory is deleted even if the test fails
            if(Directory.Exists(installed_path)) { Directory.Delete(installed_path); }            
        }

        [Test]
        public void NukeOtherInstallSimpleFileTest()
        {
            InstalledPackage ip = new InstalledPackage();
            string installed_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string installed_path = Path.Combine(installed_dir, "target.file");
            string installed_path_meta = installed_path + ".meta";
            try
            {
                Directory.CreateDirectory(installed_dir);
                File.Create(installed_path).Dispose();
                File.Create(installed_path_meta).Dispose();

                ip.Install = new InstallSpec[]
                {
                new InstallSpecPath()
                {
                    Type = InstallSpecType.Media,
                    Path = installed_path
                }
                };

                Assert.IsTrue(File.Exists(installed_path));
                Assert.IsTrue(File.Exists(installed_path_meta));

                ip.Nuke();

                Assert.IsFalse(File.Exists(installed_path), "The file did not get properly removed");
                Assert.IsFalse(File.Exists(installed_path_meta), "The .meta file did not get properly removed");
            }
            finally
            {
                if (Directory.Exists(installed_dir)) { Directory.Delete(installed_dir, true); }
            }
        }

        [Test]
        public void NukeOtherInstallComplexTest()
        {
            InstalledPackage ip = new InstalledPackage();
            string installed_dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string installed_path = Path.Combine(installed_dir, "target.file");
            string installed_path_meta = installed_path + ".meta";
            try
            {
                Directory.CreateDirectory(installed_dir);
                File.Create(installed_path).Dispose();
                File.Create(installed_path_meta).Dispose();

                ip.Install = new InstallSpec[]
                {
                new InstallSpecPath()
                {
                    Type = InstallSpecType.Media,
                    Path = installed_path
                }
                };

                Assert.IsTrue(File.Exists(installed_path));
                Assert.IsTrue(File.Exists(installed_path_meta));

                ip.Nuke();

                Assert.IsFalse(File.Exists(installed_path), "The file did not get properly removed");
                Assert.IsFalse(File.Exists(installed_path_meta), "The .meta file did not get properly removed");
            }
            finally
            {
                if (Directory.Exists(installed_dir)) { Directory.Delete(installed_dir, true); }
            }            
        }

        [Test]
        public void NukeOtherInstallWhenAbsentTest()
        {
            InstalledPackage ip = new InstalledPackage();
            string installed_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string installed_path_meta = installed_path + ".meta";

            ip.Install = new InstallSpec[]
            {
                new InstallSpecPath()
                {
                    Type = InstallSpecType.Media,
                    Path = installed_path
                }
            };

            Assert.DoesNotThrow(() => ip.Nuke());
        }
    }
}
