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
using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
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

			ip.Install = new InstallSpec[] {
				new InstallSpecPath () {
					Type = InstallSpecType.Root,
						Path = installed_path
				}
			};

			ip.Nuke();

			Assert.IsFalse(Directory.Exists(installed_path));

			// Make sure that the directory is deleted even if the test fails
			if (Directory.Exists(installed_path)) { Directory.Delete(installed_path); }
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

				ip.Install = new InstallSpec[] {
					new InstallSpecPath () {
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

				ip.Install = new InstallSpec[] {
					new InstallSpecPath () {
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

			ip.Install = new InstallSpec[] {
				new InstallSpecPath () {
					Type = InstallSpecType.Media,
						Path = installed_path
				}
			};

			Assert.DoesNotThrow(() => ip.Nuke());
		}
	}
}
#endif