// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

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
    class PackageUpdating
    {
        private UpliftManager manager;
        private Upfile upfile;
        private string upfile_path;
        private string pwd;

        [OneTimeSetUp]
        protected void Given()
        {
            UpliftManagerExposer.ClearAllInstances();

            pwd = Directory.GetCurrentDirectory();
            Helper.InitializeRunDirectory();

            try
            {
                Directory.SetCurrentDirectory(Helper.testRunDirectoryName);

                // Upfile Setup
                upfile_path = Helper.GetLocalFilePath("..", "TestData", "PackageUpdating", "Upfile.xml");

                try
                {
                    UpfileExposer.SetInstance(UpfileExposer.LoadTestXml(upfile_path));
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Make sure you are running the test from UpliftTesting/TestResults. The Upfile.xml uses the current path to register the repositories.");
                }
                upfile = Upfile.Instance();
                manager = UpliftManager.Instance();

                upfile.Dependencies[0].Version = "1.0.0";
                manager.InstallDependencies();
                upfile.Dependencies[0].Version = "1.0.1";
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
            }
        }

        [SetUp]
        protected void BeforeEach()
        {
            // Move to test running directory
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
            // Clean the UpfileHandler
            UpfileExposer.ClearInstance();

            Helper.ClearRunDirectory();
        }

        [Test]
        public void WhenUpdating()
        {
            manager.InstallDependencies();
            Upbring upbring = Upbring.Instance();

            // -- 1.0.0 UNINSTALLATION --
            // Directories absence
            Assert.IsFalse(Directory.Exists("UPackages/package_a~1.0.0"), "Package directory still exists under UPackages");
            Assert.IsFalse(Directory.Exists("Assets/UPackages/package_a~1.0.0"), "Package directory still exists under Assets/UPackages");

            // Upbring validity
            Assert.IsFalse((upbring.InstalledPackage.Any(p =>
            p.Name == "package_a" &&
            p.Version == "1.0.0"
            )), "Upbring did not properly forget the outdated installation");
            
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
#endif
