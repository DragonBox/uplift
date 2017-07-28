using System;
using Uplift.Schemas;
using NUnit.Framework;
using System.IO;
using System.Xml.Serialization;
using Uplift.Common;
using System.Linq;
using System.Reflection;

namespace UpliftTesting.UnitTesting
{
    [TestFixture]
    class FileRepositoryTest
    {
        private FileRepository fr;
        private string fr_path;

        [SetUp]
        public void Init()
        {
            Uplift.TestingProperties.SetLogging(false);
            fr = new FileRepository();
            fr_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            fr.Path = fr_path;
        }

        [Test]
        public void ListPackagesFromUnityPackagesTest()
        {
            try
            {
                Directory.CreateDirectory(fr_path);
                string package_path = Path.Combine(fr_path, "TestPackage-1.2.3.unitypackage");
                string bad_package_path = Path.Combine(fr_path, "BadPackage7.3.unitypackage");
                string not_package_path = Path.Combine(fr_path, "NotAPackage.foo");
                File.Create(package_path).Dispose();
                File.Create(bad_package_path).Dispose();
                File.Create(not_package_path).Dispose();
                Upset[] result = fr.ListPackages();
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual("TestPackage", result[0].PackageName);
                Assert.AreEqual("1.2.3", result[0].PackageVersion);
            }
            finally
            {
                Directory.Delete(fr_path, true);
            }
        }

        [Test]
        public void ListPackagesFromDirectoriesTest()
        {
            try
            {
                Directory.CreateDirectory(fr_path);
                string package_dir = Path.Combine(fr_path, "DirectoryA");
                string no_package_dir = Path.Combine(fr_path, "DirectoryB");
                Directory.CreateDirectory(package_dir);
                Directory.CreateDirectory(no_package_dir);
                string upset_path = Path.Combine(package_dir, "Upset.xml");
                File.Create(upset_path).Dispose();
                Upset test_package = new Upset() {
                    PackageLicense = "foo",
                    PackageName = "bar",
                    PackageVersion = "baz"
                };
                XmlSerializer serializer = new XmlSerializer(typeof(Upset));
                using(FileStream file = new FileStream(upset_path, FileMode.Create))
                {
                    serializer.Serialize(file, test_package);
                }
                Upset[] result = fr.ListPackages();
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual("foo", result[0].PackageLicense);
                Assert.AreEqual("bar", result[0].PackageName);
                Assert.AreEqual("baz", result[0].PackageVersion);
            }
            finally
            {
                Directory.Delete(fr_path, true);
            }
        }

        [Test]
        public void DownloadPackageFromDirectoryTest()
        {
            try
            {
                Directory.CreateDirectory(fr_path);
                string inside_dir = "InsideDirectory";
                string inside_path = Path.Combine(fr_path, inside_dir);
                Directory.CreateDirectory(inside_path);
                Upset test_package = new Upset() {
                    PackageLicense = "foo",
                    PackageName = "bar",
                    PackageVersion = "baz"
                };
                test_package.MetaInformation.dirName = inside_dir;
                string file_dummy = "Testing.foobar";
                File.Create(Path.Combine(inside_path, file_dummy)).Dispose();
                TemporaryDirectory result = fr.DownloadPackage(test_package);
                Assert.IsTrue(Directory.Exists(result.Path));
                CollectionAssert.Contains(Directory.GetFiles(result.Path).Select(Path.GetFileName).ToArray(), file_dummy);
            }
            finally
            {
                Directory.Delete(fr_path, true);
            }
        }

        [Test]
        public void DownloadPackageFromUnityPackageTest()
        {
            try
            {
                Directory.CreateDirectory(fr_path);
                Upset test_package = new Upset()
                {
                    PackageLicense = "foo",
                    PackageName = "bar",
                    PackageVersion = "baz"
                };
                string package_name = "bar-baz.unitypackage";
                test_package.MetaInformation.dirName = package_name;
                DirectoryInfo current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent;
                string package_path = Path.Combine(Path.Combine(current.FullName, "Data"), "testpackage.unitypackage");
                File.Copy(package_path, Path.Combine(fr_path, package_name));
                TemporaryDirectory result = fr.DownloadPackage(test_package);
                Assert.IsTrue(Directory.Exists(result.Path));
                string asset_path = Path.Combine(result.Path, "Assets");
                Assert.IsTrue(Directory.Exists(asset_path));
                CollectionAssert.Contains(Directory.GetFiles(asset_path), Path.Combine(asset_path, "TestScript.cs"));
                CollectionAssert.Contains(Directory.GetFiles(asset_path), Path.Combine(asset_path, "TestMat.mat")); 
            }
            finally
            {
                Directory.Delete(fr_path, true);
            }
        }

        [Test]
        public void DownloadPackageFromBadFormatTest()
        {
            try
            {
                Directory.CreateDirectory(fr_path);
                Upset test_package = new Upset()
                {
                    PackageLicense = "foo",
                    PackageName = "bar",
                    PackageVersion = "baz"
                };
                test_package.MetaInformation.dirName = "Bad.format";
                TemporaryDirectory result = fr.DownloadPackage(test_package);
                Assert.IsTrue(Directory.Exists(result.Path));
                Assert.IsEmpty(Directory.GetFiles(result.Path));
            }
            finally
            {
                Directory.Delete(fr_path, true);
            }
        }
    }
}
