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
    class BasicPackageInstallation
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
                    "BasicPackageInstallation",
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
        public void WhenInstalling()
        {
            Cli.InstallDependencies();

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
            Upbring upbring = Upbring.FromXml();
            Assert.IsNotEmpty(upbring.InstalledPackage, "Upbring file did not registered the installation");
            Assert.That(upbring.InstalledPackage.Any(p =>
            p.Name == "package_a" &&
            p.Version == "1.0.0"
            ), "Upbring did not register an installation with the proper package Name and Version");
            Assert.IsNotEmpty(upbring.InstalledPackage[0].Install, "Upbring file did not register file dependencies");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i => 
            i.Path == Path.Combine("UPackages", "package_a~1.0.0") &&
            i.Type == InstallSpecType.Root
            ));
            Assert.That(upbring.InstalledPackage[0].Install.Any(i => 
            i.Path == Path.Combine("UPackages", "package_a~1.0.0") &&
            i.Type == InstallSpecType.Root
            ), "Root installation did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.0", "A1.cs" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A1.cs did not get registered");                
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.0", "A2.cs" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A2.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.0", "A3.cs" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of A3.cs did not get registered");
            Assert.That(upbring.InstalledPackage[0].Install.Any(i =>
            i.Path == Path.Combine(new string[] { "Assets", "UPackages", "package_a~1.0.0", "Upset.xml" }) &&
            i.Type == InstallSpecType.Base
            ), "Base installation of Upset.xml did not get registered");
        }
    }
}
