using Uplift;
using Uplift.Schemas;
using Uplift.Common;
using NUnit.Framework;
using System;
using UpliftTesting.Helpers;
using System.IO;
using System.Linq;

namespace UpliftTesting.IntegrationTesting
{
    [TestFixture]
    class PackageUpdating
    {
        private UpfileHandler uph;
        private string upfile_path;

        [OneTimeSetUp]
        protected void Given()
        {
            // Turn off logging
            TestingProperties.SetLogging(false);

            // Upfile Setup
            UpfileHandlerExposer.ResetSingleton();
            upfile_path = Helper.GetLocalXMLFile(new string[]
                {
                    "IntegrationTesting",
                    "PackageUpdating",
                    "Upfile.xml"
                });

            uph = UpfileHandlerExposer.Instance();
            (uph as UpfileHandlerExposer).test_upfile_path = upfile_path;
            try
            {
                uph.Initialize();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Make sure you are running the test from UpliftTesting/TestResults. The Upfile.xml uses the current path to register the repositories.");
                Assert.IsTrue(false, "The test could not run correctly. See console message.");
            }

            (uph as UpfileHandlerExposer).GetUpfile().Dependencies[0].Version = "1.0.0";
            Cli.InstallDependencies();
            (uph as UpfileHandlerExposer).GetUpfile().Dependencies[0].Version = "1.0.1";
        }

        [OneTimeTearDown]
        protected void CleanUp()
        {
            // Clean the UpfileHandler
            UpfileHandlerExposer.ResetSingleton();

            // Remove (hopefully) installed files
            Directory.Delete("Assets", true);
            Directory.Delete("UPackages", true);
        }

        [Test]
        public void WhenUpdating()
        {
            Cli.InstallDependencies();
            Upbring upbring = Upbring.FromXml();

            // -- 1.0.0 UNINSTALLATION --
            // Directories absence
            Assert.IsFalse(Directory.Exists("UPackages/package_a~1.0.0"), "Package directory still exists under UPackages");
            Assert.IsFalse(Directory.Exists("Assets/UPackages/package_a~1.0.0"), "Package directory still exists under Assets/UPackages");

            // Upbring validity
            Assert.That((upbring.InstalledPackage.Count(p =>
            p.Name == "package_a" &&
            p.Version == "1.0.0"
            )), Is.EqualTo(0), "Upbring did not properly forget the outdated installation");

            // -- 1.0.1 INSTALLATION --
            // Directories existence
            Assert.IsTrue(Directory.Exists("UPackages"), "Directory UPackages does not exist");
            Assert.IsTrue(Directory.Exists("UPackages/package_a~1.0.1"), "Package directory does not exist under UPackages");
            Assert.IsTrue(Directory.Exists("Assets"), "Directory Assets does not exist");
            Assert.IsTrue(Directory.Exists("Assets/UPackages"), "Directory UPackages under Assets does not exist");
            Assert.IsTrue(Directory.Exists("Assets/UPackages/package_a~1.0.1"), "Package directory does not exist under Assets/UPackages");

            // Files under UPackages
            Assert.IsTrue(File.Exists("UPackages/Upbring.xml"), "Upbring file has not been created");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.1/A1.cs"), "File A1 did not get copied to UPackages/package_a~1.0.1");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.1/A2.cs"), "File A2 did not get copied to UPackages/package_a~1.0.1");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.1/A3.cs"), "File A3 did not get copied to UPackages/package_a~1.0.1");
            Assert.IsTrue(File.Exists("UPackages/package_a~1.0.1/Upset.xml"), "Upset file did not get copied to UPackages/package_a~1.0.1");

            // Upbring validity
            Assert.IsNotEmpty(upbring.InstalledPackage, "Upbring file did not registered the new installation");
            Assert.That(upbring.InstalledPackage.Any(p =>
            p.Name == "package_a" &&
            p.Version == "1.0.1"
            ), "Upbring did not register an installation with the proper package Name and Version");
            Assert.IsNotEmpty(upbring.InstalledPackage[0].Install, "Upbring file did not register file dependencies");
            // FIXME: Refactor the test to take into account the GUID tracking
            /*
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine("UPackages", "package_a~1.0.1") &&
            i.Type == InstallSpecType.Root
            ), "Root installation did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.1", "A1.cs" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A1.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.1", "A2.cs" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A2.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.1", "A3.cs" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A3.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.1", "Upset.xml" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of Upset.xml did not get registered");
            */
        }
    }
}
