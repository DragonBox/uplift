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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Uplift.Common
{
    public class FileSystemUtil
	{
		public static string JoinPaths(params string[] parameters)
		{
			return string.Join(Path.DirectorySeparatorChar.ToString(), parameters);
		}

		public static IEnumerable<string> GetFiles(string path) {
			Queue<string> queue = new Queue<string>();
			queue.Enqueue(path);
			while (queue.Count > 0) {
				path = queue.Dequeue();
				if (!Directory.Exists(path)) continue;
				foreach (string subDir in Directory.GetDirectories(path)) {
					queue.Enqueue(subDir);
				}

				string[] files = Directory.GetFiles(path);

				if (files.Length == 0) continue;

				foreach (string t in files)
				{
					yield return t;
				}
			}
		}

		public static void EnsureParentExists(string targetDir) {
			DirectoryInfo parent = Directory.GetParent(targetDir);
			if (!parent.Exists) {
				Directory.CreateDirectory(parent.FullName);
			}
		}

		public static void EnsureDirExists(string targetDir) {
			if (!Directory.Exists(targetDir)) {
				Directory.CreateDirectory(targetDir);
			}
		}

		public static string GetAbsolutePath(string dir) {
			DirectoryInfo parent = Directory.GetParent(dir);
			if (parent.Exists) {
				return Path.Combine(parent.FullName, dir);
			}
			return null; // we should really fail here...
		}

		public static void CopyDirectory(string src,string dst) {
			CopyDirectory(src, dst, new string [] {});
		}

		public static void CopyDirectory(string src,string dst, string[] ignorePatterns) {
			string[] files;

			if(dst[dst.Length-1]!=Path.DirectorySeparatorChar)
				dst+=Path.DirectorySeparatorChar;
			if(!Directory.Exists(dst)) Directory.CreateDirectory(dst);
			files=Directory.GetFileSystemEntries(src);
			foreach(string element in files) {
				if (ContainsAny(element, ignorePatterns)) continue;

				if(Directory.Exists(element))
					CopyDirectory(element,dst+Path.GetFileName(element), ignorePatterns);
				else
					File.Copy(element,dst+Path.GetFileName(element),true);
			}
		}

        public static void CopyDirectoryWithMeta(string src, string dst)
        {
            string[] files;

            if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(dst)) Directory.CreateDirectory(dst);
            files = Directory.GetFileSystemEntries(src);
            foreach (string element in files)
            {
                if (element.EndsWith(".meta")) continue;

                if (!File.Exists(element+".meta"))
                {
                    // Do nothing right now, but we may want to not support this in the future
                }
                if (Directory.Exists(element))
                {
                    string dir_dest = dst + Path.GetFileName(element);
                    CopyDirectoryWithMeta(element, dir_dest);
                    TryCopyMeta(element, dir_dest);
                }
                else
                {
                    string file_dest = dst + Path.GetFileName(element);
                    File.Copy(element, file_dest , true);
                    TryCopyMeta(element, file_dest);
                }                    
            }
        }

        public static void TryCopyMeta(string src, string dst)
        {
            if (File.Exists(src + ".meta"))
            {
                File.Copy(src + ".meta", dst + ".meta", true);
            }
        }

        public static void SaveTxtInFile(string txt,string fileName) {
			using (StreamWriter sw = new StreamWriter(fileName))
			{
				sw.Write(txt);
			}
		}

		public static string ReadTxtInFile(string fileName) {
			using (StreamReader sr = new StreamReader(fileName))
			{
				return sr.ReadToEnd();
			}
		}

		// returns true if the specfied Element contains at least one of the ContainPatterns, false otherwise
		private static bool ContainsAny (string element, string[] containPatterns)
		{
			bool contains = false;
			foreach(string containPattern in containPatterns) {
				if (element.Contains(containPattern)) {
					contains = true;
					break;
				}
			}
			return contains;
		}

		public static string GetExternDataPath() {
			if(Application.dataPath.Contains(".app"))
				return Application.dataPath + "/../../Data";
			return Application.dataPath + "/../Data";
        }

        public static string GetAppDataPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        }


        public static List<string> RecursivelyListFiles(string dir, bool relative = false)
	    {

			var files = new List<string>(Directory.GetFiles(dir));

			foreach (var subdir in Directory.GetDirectories(dir))
			{
				files.AddRange(RecursivelyListFiles(subdir));
			}


			return relative ? files.Select(d => d.Replace(dir, "").Trim(Path.DirectorySeparatorChar)).ToList() : files;
		}


		// Based on dirPaths build recursive dirPaths
		public static List<string> RecursivelyDirPaths(List<string> dirPaths)
		{
			var finalList = new List<string>();
			foreach (var directory in dirPaths)
			{
				finalList.Add(directory);
				var dirIterator = directory;

				while (!string.IsNullOrEmpty(Path.GetDirectoryName(dirIterator)))
				{
					dirIterator = Path.GetDirectoryName(dirIterator);
					finalList.Add(dirIterator);
				}
			}

			return finalList;
		}

        public static string MakePathOSFriendly(string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            return JoinPaths(path.Split('/', '\\'));
        }

        public static string MakePathWindowsFriendly(string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            return JoinPaths(path.Split('/'));
        }

        public static string MakePathUnix(string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            return string.Join("/", path.Split('/', '\\'));
        }

        public static string GetFileMD5(string path)
        {
            using(MD5 md5hash = MD5.Create())
            using(StreamReader file = new StreamReader(path))
            {
                byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(file.ReadToEnd()));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}
