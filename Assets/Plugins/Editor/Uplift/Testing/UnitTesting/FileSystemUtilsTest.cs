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
using NUnit.Framework;
using Uplift.Common;
using System.Collections.Generic;
using Uplift.Testing.Helpers;

namespace Uplift.Testing.Unit
{
    [TestFixture]
    public class FileSystemUtilsTest
    {
        [Test]
        public void JoinPathsTest()
        {
            string[] parameters = new string[] { "foo", "bar", "baz" };
            string result = FileSystemUtil.JoinPaths(parameters);
            bool correct = result.Equals("foo\\bar\\baz") || result.Equals("foo/bar/baz");
            Assert.IsTrue(correct);
        }

        [Test]
        public void EnsureMissingParentExistsTest()
        {
            string tempParentDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string tempChildDirectory = Path.Combine(tempParentDirectory, Path.GetRandomFileName());
            try
            {
                FileSystemUtil.EnsureParentExists(tempChildDirectory);
                Assert.IsTrue(Directory.Exists(tempParentDirectory));
            }
            finally
            {
                Directory.Delete(tempParentDirectory, true);
            }
        }

        [Test]
        public void EnsurePresentParentExistsTest()
        {
            string tempParentDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string tempChildDirectory = Path.Combine(tempParentDirectory, Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tempParentDirectory);
                FileSystemUtil.EnsureParentExists(tempChildDirectory);
                Assert.IsTrue(Directory.Exists(tempParentDirectory));
            }
            finally
            {
                Directory.Delete(tempParentDirectory, true);
            }
        }

        [Test]
        public void GetAbsolutePathTest()
        {
            // Mono has a bug so that Path.GetFullPath(Path.GetTempPath())
            // doesn't return the same as GetFullPath(".") if Current Directory is Path.GetTempPath()
            string TempPathFullName = Helper.SetCurrentDirectory(Path.GetTempPath(),
                () => { return Directory.GetCurrentDirectory(); } );

            string dirName = Path.GetRandomFileName();
            string dirPath = Path.Combine(TempPathFullName, dirName);

            Helper.SetCurrentDirectory<Object>(Path.GetTempPath(), () => {
                try
                {
                    Directory.CreateDirectory(dirPath);
                    Assert.AreEqual(dirPath, FileSystemUtil.GetAbsolutePath(dirName));
                    return null;
                }
                finally
                {
                    Directory.Delete(dirPath, true);
                }
            } );
        }

        [Test]
        public void GetAbsolutePathTestFail()
        {
            string mainDirName = Path.GetRandomFileName();
            string intermediateDirName = Path.GetRandomFileName();
            string path = Path.Combine(intermediateDirName, mainDirName);
            Assert.IsNull(FileSystemUtil.GetAbsolutePath(path));
        }

        [Test]
        public void SaveTxtInFileTest()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                string testString = "foo\nbar\nbaz";
                FileSystemUtil.SaveTxtInFile(testString, tempFile);
                using (StreamReader sr = new StreamReader(tempFile))
                {
                    Assert.AreEqual(testString, sr.ReadToEnd());
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void ReadTxtInFileTest()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                string testString = "foo\nbar\nbaz";
                using (StreamWriter sw = new StreamWriter(tempFile))
                {
                    sw.Write(testString);
                }
                Assert.AreEqual(testString, FileSystemUtil.ReadTxtInFile(tempFile));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Test]
        [Platform("Win")]
        public void RecursivelyDirPathsTestWindows()
        {
            string testPath = "C:\\Users\\User\\Documents\\foo\\bar.baz";

            string[] dirs = new string[]
            {
                "C:\\Users\\User\\Documents\\foo\\bar.baz",
                "C:\\Users\\User\\Documents\\foo",
                "C:\\Users\\User\\Documents",
                "C:\\Users\\User",
                "C:\\Users",
                "C:\\"
            };

            CollectionAssert.AreEqual(dirs, FileSystemUtil.RecursivelyDirPaths(new List<string>() { testPath }));
        }

        [Test]
        [Platform("Linux")]
        public void RecursivelyDirPathsTestLinux()
        {
            string testPath = "/home/user/Documents/foo/bar.baz";

            string[] dirs = new string[]
            {
                "/home/user/Documents/foo/bar.baz",
                "/home/user/Documents/foo",
                "/home/user/Documents",
                "/home/user",
                "/home",
                "/"
            };

            CollectionAssert.AreEqual(dirs, FileSystemUtil.RecursivelyDirPaths(new List<string>() { testPath }));
        }

        [Test]
        [Platform(Exclude="Win,Linux")]
        public void RecursivelyDirPathsTestMac()
        {
            string testPath = "/Users/user/Documents/foo/bar.baz";

            string[] dirs = new string[]
            {
                "/Users/user/Documents/foo/bar.baz",
                "/Users/user/Documents/foo",
                "/Users/user/Documents",
                "/Users/user",
                "/Users",
                "/"
            };

            CollectionAssert.AreEqual(dirs, FileSystemUtil.RecursivelyDirPaths(new List<string>() { testPath }));
        }

        [Test]
        public void GetFilesTest()
        {
            int file_amount = 5;
            string[] files = new string[file_amount];
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tempDirectory);
                for (int i = 0; i < file_amount; i++)
                {
                    string filename = Path.Combine(tempDirectory, Path.GetRandomFileName());
                    File.Create(filename).Dispose();
                    files[i] = filename;
                }
                CollectionAssert.AreEquivalent(files, FileSystemUtil.GetFiles(tempDirectory));
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void GetFilesRecursivelyTest()
        {
            int file_amount = 6;
            string[] files = new string[file_amount];
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tempDirectory);
                for (int i = 0; i < file_amount - 1; i++)
                {
                    string filename = Path.Combine(tempDirectory, Path.GetRandomFileName());
                    File.Create(filename).Dispose();
                    files[i] = filename;
                }
                string innerDirectory = Path.Combine(tempDirectory, Path.GetRandomFileName());
                Directory.CreateDirectory(innerDirectory);
                string innerTempFile = Path.Combine(innerDirectory, Path.GetRandomFileName());
                File.Create(innerTempFile).Dispose();
                files[file_amount - 1] = innerTempFile;
                CollectionAssert.AreEquivalent(files, FileSystemUtil.GetFiles(tempDirectory));
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void GetFilesEmptyTest()
        {
            List<string> files = new List<string>();
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tempDirectory);
                CollectionAssert.AreEquivalent(files, FileSystemUtil.GetFiles(tempDirectory));
            }
            finally
            {
                Directory.Delete(tempDirectory);
            }
        }

        [Test]
        public void GetFilesFromNonExistentDirectoryTest()
        {
            List<string> files = new List<string>();
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            CollectionAssert.AreEquivalent(files, FileSystemUtil.GetFiles(tempDirectory));
        }

        [Test]
        public void CopyDirectoryTest()
        {
            string temp_path = Path.GetTempPath();
            string source_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            string dest_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(source_dir);
                FileSystemUtil.CopyDirectory(source_dir, dest_dir);
                Assert.IsTrue(Directory.Exists(dest_dir));
            }
            finally
            {
                Directory.Delete(source_dir);
                Directory.Delete(dest_dir);
            }
        }

        [Test]
        public void CopyToExitstingDirectoryTest()
        {
            string temp_path = Path.GetTempPath();
            string source_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            string dest_dir = Path.Combine(temp_path, Path.GetRandomFileName()) + Path.DirectorySeparatorChar;
            try
            {
                Directory.CreateDirectory(source_dir);
                Directory.CreateDirectory(dest_dir);
                FileSystemUtil.CopyDirectory(source_dir, dest_dir);
                Assert.IsTrue(Directory.Exists(dest_dir));
            }
            finally
            {
                Directory.Delete(source_dir);
                Directory.Delete(dest_dir);
            }
        }

        [Test]
        public void CopyDirectoryRecursivelyTest()
        {
            string temp_path = Path.GetTempPath();
            string source_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            string dest_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            try
            {
                string innerDirName = Path.GetRandomFileName();
                Directory.CreateDirectory(source_dir);
                Directory.CreateDirectory(Path.Combine(source_dir, innerDirName));
                FileSystemUtil.CopyDirectory(source_dir, dest_dir);
                Assert.IsTrue(Directory.Exists(Path.Combine(dest_dir, innerDirName)));
            }
            finally
            {
                Directory.Delete(source_dir, true);
                Directory.Delete(dest_dir, true);
            }
        }
        
        [Test]
        public void CopyAndIgnoreDirectoryTest()
        {
            string temp_path = Path.GetTempPath();
            string source_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            string dest_dir = Path.Combine(temp_path, Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(source_dir);
                string ignoredFile = "foobarbaz";
                string notIgnoredFile = "NotToBeIgnored";
                File.Create(Path.Combine(source_dir, ignoredFile)).Dispose();
                File.Create(Path.Combine(source_dir, notIgnoredFile)).Dispose();
                FileSystemUtil.CopyDirectory(source_dir, dest_dir, new string[] { "foo", "bar", "baz" });
                Assert.IsFalse(File.Exists(Path.Combine(dest_dir, ignoredFile)));
                Assert.IsTrue(File.Exists(Path.Combine(dest_dir, notIgnoredFile)));
            }
            finally
            {
                Directory.Delete(source_dir, true);
                Directory.Delete(dest_dir, true);
            }
        }

        [Test]
        public void RecursivelyListFilesTest()
        {
            string dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            int depth_testing = 3;
            try
            {
                List<string> files = new List<string>();
                Directory.CreateDirectory(dir);
                string innerPath = dir;
                string fileName;
                for(int i = 0; i < depth_testing; i++)
                {
                    innerPath = Path.Combine(innerPath, Path.GetRandomFileName());
                    fileName = Path.Combine(innerPath, Path.GetRandomFileName());
                    Directory.CreateDirectory(innerPath);
                    File.Create(fileName).Dispose();
                    files.Add(fileName);
                }
                CollectionAssert.AreEquivalent(files, FileSystemUtil.RecursivelyListFiles(dir));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Test]
        public void RecursivelyListFilesRelativeTest()
        {
            string dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(dir);
                string[] files = new string[] { "foo.foo", "bar.bar", "baz.baz" };
                foreach(string file in files)
                {
                    File.Create(Path.Combine(dir, file)).Dispose();
                }
                CollectionAssert.AreEquivalent(files, FileSystemUtil.RecursivelyListFiles(dir, true));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
#endif
