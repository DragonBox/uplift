using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
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
	
		public static string GetAbsolutePath(string dir) {
			DirectoryInfo parent = Directory.GetParent(dir);
			if (parent.Exists) {
				return parent.FullName + "/" + dir;
			}
			return null; // we should really fail here...
		}	

		public static void CopyDirectory(string src,string dst) {
			CopyDirectory(src, dst, new string [] {});	
		}

		public static void CopyDirectory(string src,string dst,string[] ignorePatterns) {
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
			if(Application.dataPath.Contains(".app")) //On est sur un exe Mac
				return Application.dataPath + "/../../Data";
			return Application.dataPath + "/../Data";
		}


		public static List<string> RecursivelyListFiles(string dir, bool relative = false)
		{

			var files = new List<string>(Directory.GetFiles(dir));

			foreach (var subdir in Directory.GetDirectories(dir))
			{
				files.AddRange(RecursivelyListFiles(subdir));
			}
			
			
			return relative ? files.Select(d => d.Replace(dir, "").Trim('/')).ToList() : files;
		}
	}
}


