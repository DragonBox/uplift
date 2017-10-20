using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uplift.Schemas;

namespace Uplift.SourceControl
{
	public class GitIgnorer : ISourceControlHandler
	{
		public delegate void FileEdit(object sender, string path);
		public event FileEdit OnEditGitignore = null;
		private readonly string ignoreTemplateHeader = "# == UPLIFT GITIGNORE START ==";
		private readonly string ignoreTemplateComment = "# This section of the .gitignore has been created automatically by Uplift. Do not modify it or remove it.";
		private readonly string ignoreTemplateFooter = "# == UPLIFT GITIGNORE END ==";

		public void HandleDirectory(string path)
		{
			Ignore(
				Path.GetDirectoryName(path),
				"/" + new DirectoryInfo(path).Name
			);
			Ignore(
				Path.GetDirectoryName(path),
				"/" + new DirectoryInfo(path).Name + ".meta"
			);

		}

		public void HandleFile(string path)
		{
			Ignore(
				Path.GetDirectoryName(path),
				"/" + Path.GetFileName(path)
			);
			Ignore(
				Path.GetDirectoryName(path),
				"/" + Path.GetFileName(path) + ".meta"
			);
		}

		public bool TryRemoveFile(string gitIgnorePath)
		{
			string[] upliftPatterns;
			string[][] userLines = ExtractExistingLines(gitIgnorePath, out upliftPatterns);

			if(userLines[0].Length == 0 && userLines[1].Length == 0 && upliftPatterns.Length == 1)
			{
				File.Delete(gitIgnorePath);
				return true;
			}
			return false;
		}

		private void Ignore(string path, string pattern)
		{
			string gitIgnorePath = Path.Combine(path, ".gitignore");
			string[] upliftPatterns;
			string[][] userLines = ExtractExistingLines(gitIgnorePath, out upliftPatterns);

			// Avoid duplicated entries
			if(!upliftPatterns.Any(pat => pat == pattern))
			{
				string[] temp = new string[upliftPatterns.Length + 1];
				Array.Copy(upliftPatterns, temp, upliftPatterns.Length);
				temp[upliftPatterns.Length] = pattern;
				upliftPatterns = temp;
			}

			using(StreamWriter sw = new StreamWriter(gitIgnorePath, false))
			{
				foreach(string line in userLines[0])
					sw.WriteLine(line);
				sw.WriteLine(ignoreTemplateHeader);
				sw.WriteLine(ignoreTemplateComment);
				foreach(string line in upliftPatterns)
				{
					sw.WriteLine(line);
				}
				sw.WriteLine(ignoreTemplateFooter);
				foreach(string line in userLines[1])
					sw.WriteLine(line);
			}

			if(OnEditGitignore != null)
				OnEditGitignore(this, gitIgnorePath);
		}

		private string[][] ExtractExistingLines(string path, out string[] upliftPatterns)
		{
			List<string> readLines = new List<string>();
			List<string> upliftLines = new List<string>();
			string[][] userLines = new string[2][]{
				new string[0],
				new string[0]
			};

			upliftPatterns = new string[0];

			if(File.Exists(path))
			{
				foreach(string line in File.ReadAllLines(path))
					readLines.Add(line);
				if(readLines.Count == 0)
				{
					upliftPatterns = new string[0];
					return userLines;
				}

				int headerIndex = readLines.IndexOf(ignoreTemplateHeader);
				int footerIndex = readLines.IndexOf(ignoreTemplateFooter);
				if(headerIndex != -1 && footerIndex != -1 && headerIndex < footerIndex)
				{
					for(int i = headerIndex + 1; i < footerIndex; i++)
					{
						if(readLines[i].StartsWith(ignoreTemplateComment)) continue;
						upliftLines.Add(readLines[i]);
					}

					userLines[0] = new string[headerIndex];
					int lastIndex = readLines.Count - 1;
					userLines[1] = new string[lastIndex - footerIndex];
					Array.Copy(readLines.ToArray(), 0, userLines[0], 0, headerIndex);
					if(footerIndex != lastIndex)
						Array.Copy(readLines.ToArray(), footerIndex + 1, userLines[1], 0, lastIndex - footerIndex);
				}
				else if(((headerIndex == -1) ^ (footerIndex == -1)) || (footerIndex < headerIndex))
				{
					// One of the line is present, not the other one OR the footer is before the header
					UnityEngine.Debug.LogErrorFormat("The .gitignore at {0} is not properly formed", path);
				}
				else
				{
					userLines[0] = readLines.ToArray();
				}
			}

			upliftPatterns = upliftLines.ToArray();
			return userLines;
		}
	}
}
