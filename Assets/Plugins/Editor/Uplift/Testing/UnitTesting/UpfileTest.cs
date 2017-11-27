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
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Xml;
using Uplift.Schemas;
using Uplift.Testing.Helpers;

namespace Uplift.Testing.Unit
{
    [TestFixture]
    public class UpfileTest
    {
        [SetUp]
        protected void BeforeEachTest()
        {
            UpfileExposer.ClearInstance();
        }

        [OneTimeTearDown]
        protected void AfterAllTests()
        {
            UpfileExposer.ClearInstance();
        }

        [Test]
        public void InstanceUnicityTest()
        {
            Upfile upA = Upfile.Instance();
            Upfile upB = Upfile.Instance();

            Assert.AreSame(upA, upB);
        }

        [Test]
        public void CheckForUpfilePresentTest()
        {
            string pwd = Directory.GetCurrentDirectory();
            string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmpDir);
            try
            {
                Directory.SetCurrentDirectory(tmpDir);

                // Create dummy Upfile
                File.Create("Upfile.xml").Dispose();

                Assert.IsTrue(Upfile.CheckForUpfile());
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(tmpDir, true);
            }
        }

        [Test]
        public void CheckForUpfileAbsentTest()
        {
            string pwd = Directory.GetCurrentDirectory();
            string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmpDir);
            try
            {
                Directory.SetCurrentDirectory(tmpDir);

                Assert.IsFalse(Upfile.CheckForUpfile());
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
                Directory.Delete(tmpDir, true);
            }
        }

        [Test]
        public void LoadXmlPresent()
        {
            string upfilePath = Helper.PathCombine("TestData", "UpfileTest", "Upfile.xml");

            Upfile parsed = Upfile.LoadXml(upfilePath);
            Assert.IsTrue(parsed.Dependencies.Any(dep => string.Equals(dep.Name, "test_package")), "No correct dependency found");
            Assert.IsTrue(parsed.Repositories.Any(repo => repo is FileRepository && string.Equals((repo as FileRepository).Path, Helper.PathCombine("Path", "To", "Some", "Repository"))), "No correct repository found");
        }

        [Test]
        public void LoadXmlAbsent()
        {
            string upfilePath = Helper.PathCombine("TestData", "UpfileTest", "NoUpfileInIt", "Upfile.xml");

            bool caught = false;
            try
            {
                Upfile.LoadXml(upfilePath);
            }
            catch
            {
                caught = true;
            }
            Assert.IsTrue(caught);
        }

        [Test]
        public void LoadPresentOverrideTest()
        {
            string upfilePath = Helper.PathCombine ("TestData", "UpfileTest", "Upfile.xml");
            string upfileOverridePath = Helper.PathCombine ("TestData", "UpfileTest", "UpliftSettings.xml");
            Upfile upfile = Upfile.LoadXml(upfilePath);
            upfile.overridePath = upfileOverridePath;
            upfile.LoadOverrides();

            Assert.IsTrue(upfile.Repositories.Any(repo => repo is FileRepository && string.Equals((repo as FileRepository).Path, Helper.PathCombine("Path","To","Some","Repository"))), "Original repository not found");
            Assert.IsTrue(upfile.Repositories.Any(repo => repo is FileRepository && string.Equals((repo as FileRepository).Path, Helper.PathCombine("Path","To","Another","Repository"))), "Override repository not found");
        }

        [Test]
        public void LoadAbsentOverrideTest()
        {
            string upfilePath = Helper.PathCombine ("TestData", "UpfileTest", "Upfile.xml");
            string upfileOverridePath = Helper.PathCombine ("TestData", "UpfileTest", "NoUpfileInIt", "UpliftSettings.xml");
            Upfile upfile = Upfile.LoadXml(upfilePath);
            upfile.overridePath = upfileOverridePath;
            upfile.LoadOverrides();

            Assert.IsTrue(upfile.Repositories.Any(repo => repo is FileRepository && string.Equals((repo as FileRepository).Path, Helper.PathCombine("Path","To","Some","Repository"))), "Original repository not found");
            Assert.IsFalse(upfile.Repositories.Any(repo => repo is FileRepository && string.Equals((repo as FileRepository).Path, Helper.PathCombine("Path","To","Another","Repository"))), "Loaded absent file");
        }

        [Test]
        public void GetDestinationForBase()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                BaseInstallPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.Base
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }

        [Test]
        public void GetDestinationForDocs()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                DocsPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.Docs
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }

        [Test]
        public void GetDestinationForEditorPlugin()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                EditorPluginPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.EditorPlugin
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }

        [Test]
        public void GetDestinationForExamples()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                ExamplesPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.Examples
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }
        
        [Test]
        public void GetDestinationForGizmo()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                GizmoPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.Gizmo
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }

        [Test]
        public void GetDestinationForMedia()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                MediaPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.Media
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }

        [Test]
        public void GetDestinationForPlugin()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                PluginPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath
            {
                Type = InstallSpecType.Plugin
            };
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }

        [Test]
        public void GetDestinationForDefault()
        {
            Upfile upfile = new Upfile();
            upfile.Configuration = new Configuration
            {
                BaseInstallPath = new PathConfiguration { Location = "foo" }
            };

            InstallSpec test_spec = new InstallSpecPath();
            Assert.AreEqual("foo", upfile.GetDestinationFor(test_spec).Location);
        }
    }
}
#endif
