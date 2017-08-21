using Uplift;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UpliftTesting.Helpers;
using System.IO;
using System.Linq;
using Uplift.Packages;
using Uplift.Schemas;
using Uplift.Common;

namespace UpliftTesting.IntegrationTesting
{
    [TestFixture]
    class PackageNuking
    {
        private UpliftManager manager;
        private Upfile upfile;
        private string upfile_path;
        private string[] original_snapshot;

        [OneTimeSetUp]
        protected void Given()
        {
            manager = UpliftManager.Instance();

            // Upfile Setup for filler package
            UpfileExposer.ClearInstance();
            upfile_path = Helper.GetLocalXMLFile(new string[]
                {
                    "IntegrationTesting",
                    "PackageNuking",
                    "Init_Upfile.xml"
                });

            try
            {
                UpfileExposer.SetInstance(UpfileExposer.LoadTestXml(upfile_path));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Make sure you are running the test from UpliftTesting/TestResults. The Upfile.xml uses the current path to register the repositories.");
                Assert.IsTrue(false, "The test could not run correctly. See console message.");
            }
            upfile = UpfileExposer.TestingInstance();

            // Creating original state
            Directory.CreateDirectory("Assets");
            Directory.CreateDirectory("Assets/Media");

            File.Create("Assets/scriptA.cs").Dispose();
            File.Create("Assets/scriptB.cs").Dispose();
            File.Create("Assets/Media/mediaA.txt").Dispose();
            File.Create("Assets/Media/mediaB.txt").Dispose();

            // Install Filler Package
            manager.InstallDependencies();

            // Save the snapshot
            original_snapshot = GetSnapshot();

            // Proper Upfile Setup
            UpfileExposer.ClearInstance();
            upfile_path = Helper.GetLocalXMLFile(new string[]
                {
                    "IntegrationTesting",
                    "PackageNuking",
                    "Upfile.xml"
                });

            try
            {
                UpfileExposer.SetInstance(UpfileExposer.LoadTestXml(upfile_path));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Make sure you are running the test from UpliftTesting/TestResults. The Upfile.xml uses the current path to register the repositories.");
                Assert.IsTrue(false, "The test could not run correctly. See console message.");
            }
            upfile = Upfile.Instance();
        }

        [SetUp]
        protected void BeforeEach()
        {
            //Install Package D
            manager.InstallDependencies();
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
            UpfileExposer.ClearInstance();

            // Remove (hopefully) installed files
            if (Directory.Exists("Assets")) Directory.Delete("Assets", true);
            if (Directory.Exists("UPackages")) Directory.Delete("UPackages", true);
        }

        [Test]
        public void WhenNoFileIsAdded()
        {
            throw new NotSupportedException("Nuking from tests is not currently supported");
            manager.NukePackage("package_d");
            
            CollectionAssert.AreEquivalent(original_snapshot, GetSnapshot());
        }

        [Test]
        public void WhenFilesAreAdded()
        {
            throw new NotSupportedException("Nuking from tests is not currently supported");
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

            manager.NukePackage("package_d");

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
