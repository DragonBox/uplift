#if UNITY_5_3_OR_NEWER
using Uplift;
using Uplift.Schemas;
using Uplift.Common;
using NUnit.Framework;
using System;
using Uplift.Testing.Helpers;
using System.IO;
using System.Linq;

namespace Uplift.Testing.Integration
{
    [TestFixture]
    class BasicPackageInstallation
    {
        private UpliftManager manager;
        private string upfile_path;
        private string pwd;

        [OneTimeSetUp]
        protected void Given()
        {
            UpliftManagerExposer.ClearAllInstances();

            pwd = Directory.GetCurrentDirectory();
        }

        [SetUp]
        protected void BeforeEach()
        {
            // Upfile Cleanup
            UpfileExposer.ClearInstance();

            // Move to test running directory
            Helper.InitializeRunDirectory();
            Directory.SetCurrentDirectory(Helper.testRunDirectoryName);
        }

        [TearDown]
        protected void AfterEach()
        {
            Directory.SetCurrentDirectory(pwd);
        }

        [OneTimeTearDown]
        protected void CleanUp()
        {
            // Remove (hopefully) installed files
            UpfileExposer.ClearInstance();
            Helper.ClearRunDirectory();
        }

        [Test]
        public void WhenInstalling()
        {
            upfile_path = Helper.GetLocalFilePath ("..", "TestData", "BasicPackageInstallation", "Upfile.xml");

            try
            {
                UpfileExposer.SetInstance(UpfileExposer.LoadTestXml(upfile_path));
            }
            catch (FileNotFoundException)
            {
                UnityEngine.Debug.LogError("The Upfile.xml uses the current path to register the repositories.");
            }
            Upfile.Instance();
            manager = UpliftManager.Instance();

            manager.InstallDependencies();

            // Directories existence
            Assert.IsTrue(Directory.Exists("UPackages"), "Directory UPackages does not exist");
            Assert.IsTrue(Directory.Exists("UPackages/package_a~1.0.0"), "Package directory does not exist under UPackages");
            Assert.IsTrue(Directory.Exists("Assets"), "Directory Assets does not exist");
            Assert.IsTrue(Directory.Exists("Assets/UPackages"), "Directory UPackages under Assets does not exist");
            Assert.IsTrue(Directory.Exists("Assets/UPackages/package_a~1.0.0"), "Package directory does not exist under Assets/UPackages");

            // Files under Assets/UPackages
            Assert.IsTrue(File.Exists("Assets/UPackages/package_a~1.0.0/A1.cs"), "File A1 did not get copied to Assets/UPackages/package_a~1.0.0");
            Assert.IsTrue(File.Exists("Assets/UPackages/package_a~1.0.0/A2.cs"), "File A2 did not get copied to Assets/UPackages/package_a~1.0.0");
            Assert.IsTrue(File.Exists("Assets/UPackages/package_a~1.0.0/A3.cs"), "File A3 did not get copied to Assets/UPackages/package_a~1.0.0");
            Assert.IsTrue(File.Exists("Assets/UPackages/package_a~1.0.0/Upset.xml"), "Upset file did not get copied to Assets/UPackages/package_a~1.0.0");

            // Files under UPackages
            Assert.IsTrue(File.Exists("UPackages/Upbring.xml"), "Upbring file has not been created");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.0/A1.cs"), "File A1 did not get copied to UPackages/package_a~1.0.0");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.0/A2.cs"), "File A2 did not get copied to UPackages/package_a~1.0.0");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.0/A3.cs"), "File A3 did not get copied to UPackages/package_a~1.0.0");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.0/Upset.xml"), "Upset file did not get copied to UPackages/package_a~1.0.0");

            // Upbring validity
            Upbring upbring = Upbring.Instance();
            Assert.IsNotEmpty(upbring.InstalledPackage, "Upbring file did not registered the installation");
            Assert.That(upbring.InstalledPackage.Any(p =>
            p.Name == "package_a" &&
            p.Version == "1.0.0"
            ), "Upbring did not register an installation with the proper package Name and Version");
            Assert.IsNotEmpty(upbring.InstalledPackage[0].Install, "Upbring file did not register file dependencies");

            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i is InstallSpecPath &&
            (i as InstallSpecPath).Path == "UPackages/package_a~1.0.0" &&
            i.Type == InstallSpecType.Root
            ), "Root installation did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i is InstallSpecPath &&
            (i as InstallSpecPath).Path == "Assets/UPackages/package_a~1.0.0/A1.cs" &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A1.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i is InstallSpecPath &&
            (i as InstallSpecPath).Path == "Assets/UPackages/package_a~1.0.0/A2.cs" &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A2.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i is InstallSpecPath &&
            (i as InstallSpecPath).Path == "Assets/UPackages/package_a~1.0.0/A3.cs" &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A3.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i is InstallSpecPath &&
            (i as InstallSpecPath).Path == "Assets/UPackages/package_a~1.0.0/Upset.xml" &&
            i.Type == InstallSpecType.Base
            ), "Base installation of Upset.xml did not get registered");
        }
    }
}
#endif
