using Uplift;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UpliftTesting.Helpers;
using System.IO;
using System.Linq;
using Uplift.Packages;

namespace UpliftTesting.IntegrationTesting
{
    [TestFixture]
    class PackageNuking
    {
        private UpfileHandler uph;
        private string upfile_path;
        private string[] original_snapshot;

        [OneTimeSetUp]
        protected void Given()
        {
            // Turn off logging
            TestingProperties.SetLogging(false);

            // Upfile Setup for filler package
            UpfileHandlerExposer.ResetSingleton();
            upfile_path = Helper.GetLocalXMLFile(new string[]
                {
                    "IntegrationTesting",
                    "PackageNuking",
                    "Init_Upfile.xml"
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
                throw new InvalidOperationException("The test setup is not correct. Please run this test from UpliftTesting/TestResults");
            }

            // Creating original state
            Directory.CreateDirectory("Assets");
            Directory.CreateDirectory("Assets/Media");

            File.Create("Assets/scriptA.cs").Dispose();
            File.Create("Assets/scriptB.cs").Dispose();
            File.Create("Assets/Media/mediaA.txt").Dispose();
            File.Create("Assets/Media/mediaB.txt").Dispose();

            // Install Filler Package
            uph.InstallDependencies();

            // Save the snapshot
            original_snapshot = GetSnapshot();

            // Proper Upfile Setup
            UpfileHandlerExposer.ResetSingleton();
            upfile_path = Helper.GetLocalXMLFile(new string[]
                {
                    "IntegrationTesting",
                    "PackageNuking",
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
                throw new InvalidOperationException("The test setup is not correct. Please run this test from UpliftTesting/TestResults");
            }
        }

        [SetUp]
        protected void BeforeEach()
        {
            //Install Package D
            uph.InstallDependencies();
        }

        [TearDown]
        protected void AfterEach()
        {
            // Remove all files that were not in the original snapshot
            string[] current_snapshot = GetSnapshot();
            foreach(string file in current_snapshot)
            {
                if(Array.IndexOf(original_snapshot, file) == -1)
                {
                    File.Delete(file);
                }
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
        public void WhenNoFileIsAdded()
        {
            LocalHandler.NukePackage("package_d");
            
            CollectionAssert.AreEquivalent(original_snapshot, GetSnapshot());
        }

        [Test]
        public void WhenFilesAreAdded()
        {
            string[] extra_files = new string[]
            {
                "Assets\\scriptC.cs",
                "Assets\\scriptD.cs",
                "Assets\\Media\\mediaC.txt",
                "Assets\\Media\\mediaD.txt"
            };
            foreach (string file in extra_files)
            {
                File.Create(file).Dispose();
            }

            LocalHandler.NukePackage("package_d");

            string[] expected = new string[original_snapshot.Length + extra_files.Length];
            Array.Copy(original_snapshot, expected, original_snapshot.Length);
            Array.Copy(extra_files, 0, expected, original_snapshot.Length, extra_files.Length);

            CollectionAssert.AreEquivalent(expected, GetSnapshot());
        }

        private string[] GetSnapshot()
        {
            return RecursiveListing("Assets").Concat(RecursiveListing("UPackages")).ToArray();
        }

        private List<string> RecursiveListing(string path)
        {
            if (!Directory.Exists(path))
            {
                return new List<string>();
            }

            List<string> inner = new List<string>();

            foreach(string dir in Directory.GetDirectories(path))
            {
                List<string> dir_files = RecursiveListing(dir);
                inner = inner.Concat(dir_files).ToList();
            }

            foreach (string file in Directory.GetFiles(path))
            {
                inner.Add(file);
            }

            return inner;
        }
    }
}
