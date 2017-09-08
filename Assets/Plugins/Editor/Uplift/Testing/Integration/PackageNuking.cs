using Uplift;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Uplift.Testing.Helpers;
using System.IO;
using System.Linq;
using Uplift.Packages;
using Uplift.Schemas;
using Uplift.Common;

namespace Uplift.Testing.Integration
{
    [TestFixture]
    class PackageNuking
    {
        private UpliftManager manager;
        private Upfile upfile;
        private string upfile_path;
        private string[] original_snapshot;
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

                // Upfile Setup for filler package
                upfile_path = Helper.GetLocalFilePath(new string[]
                    {
                        "..",
                        "TestData",
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
                manager = UpliftManager.Instance();

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
                UpliftManagerExposer.ClearAllInstances();
                UpfileExposer.ClearInstance();
                upfile_path = Helper.GetLocalFilePath(new string[]
                    {
                        "..",
                        "TestData",
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
                manager = UpliftManager.Instance();
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

            Directory.SetCurrentDirectory(pwd);
        }

        [OneTimeTearDown]
        protected void CleanUp()
        {
            // Clean the Upfile
            UpfileExposer.ClearInstance();
            Helper.ClearRunDirectory();
        }

        [Test]
        public void WhenNoFileIsAdded()
        {
            manager.NukePackage("package_d");
            
            CollectionAssert.AreEquivalent(original_snapshot, GetSnapshot());
        }

        [Test]
        public void WhenFilesAreAdded()
        {
            string[] extra_files = new string[]
            {
                Helper.PathCombine("Assets", "scriptC.cs"),
                Helper.PathCombine("Assets", "scriptD.cs"),
                Helper.PathCombine("Assets","Media","mediaC.txt"),
                Helper.PathCombine("Assets","Media","mediaD.txt")
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
