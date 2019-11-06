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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			if (path.EndsWith(".meta")) return; // Handled by base file
			string unixPath = "/" + Uplift.Common.FileSystemUtil.MakePathUnix(path);
			Ignore(unixPath);
			Ignore(unixPath + ".meta");
		}

		public void HandleFile(string path)
		{
			if (path.EndsWith(".meta")) return; // Handled by base file
			string unixPath = "/" + Uplift.Common.FileSystemUtil.MakePathUnix(path);
			Ignore(unixPath);
			Ignore(unixPath + ".meta");
		}

		public bool TryRemoveFile(string gitIgnorePath)
		{
			string[] upliftPatterns;
			string[][] userLines = ExtractExistingLines(gitIgnorePath, out upliftPatterns);

			if (userLines[0].Length == 0 && userLines[1].Length == 0 && upliftPatterns.Length <= 3)
			{
				bool deletable = false;
				if (upliftPatterns.Length == 1) deletable = true;
				if (upliftPatterns.Length == 2) deletable = string.Equals(upliftPatterns[0] + ".meta", upliftPatterns[1]) || string.Equals(upliftPatterns[0], upliftPatterns[1] + ".meta");

				if (deletable)
				{
					File.Delete(gitIgnorePath);
					return true;
				}
			}

			return false;
		}

		private void Ignore(string path)
		{
			string gitIgnorePath = ".gitignore";
			string[] upliftPatterns;
			string[][] userLines = ExtractExistingLines(gitIgnorePath, out upliftPatterns);

			// Avoid duplicated entries
			if (!upliftPatterns.Any(pat => pat == path))
			{
				string[] temp = new string[upliftPatterns.Length + 1];
				Array.Copy(upliftPatterns, temp, upliftPatterns.Length);
				temp[upliftPatterns.Length] = path;
				upliftPatterns = temp;
			}

			using (StreamWriter sw = new StreamWriter(gitIgnorePath, false))
			{
				sw.NewLine = "\n";
				foreach (string line in userLines[0])
					sw.WriteLine(line);
				sw.WriteLine(ignoreTemplateHeader);
				sw.WriteLine(ignoreTemplateComment);
				foreach (string line in upliftPatterns)
				{
					sw.WriteLine(line);
				}
				sw.WriteLine(ignoreTemplateFooter);
				foreach (string line in userLines[1])
					sw.WriteLine(line);
			}

			if (OnEditGitignore != null)
				OnEditGitignore(this, gitIgnorePath);
		}

		private string[][] ExtractExistingLines(string path, out string[] upliftPatterns)
		{
			List<string> readLines = new List<string>();
			List<string> upliftLines = new List<string>();
			string[][] userLines = new string[2][] {
				new string[0],
					new string[0]
			};

			upliftPatterns = new string[0];

			if (File.Exists(path))
			{
				foreach (string line in File.ReadAllLines(path))
					readLines.Add(line);
				if (readLines.Count == 0)
				{
					upliftPatterns = new string[0];
					return userLines;
				}

				int headerIndex = readLines.IndexOf(ignoreTemplateHeader);
				int footerIndex = readLines.IndexOf(ignoreTemplateFooter);
				if (headerIndex != -1 && footerIndex != -1 && headerIndex < footerIndex)
				{
					for (int i = headerIndex + 1; i < footerIndex; i++)
					{
						if (readLines[i].StartsWith(ignoreTemplateComment)) continue;
						upliftLines.Add(readLines[i]);
					}

					userLines[0] = new string[headerIndex];
					int lastIndex = readLines.Count - 1;
					userLines[1] = new string[lastIndex - footerIndex];
					Array.Copy(readLines.ToArray(), 0, userLines[0], 0, headerIndex);
					if (footerIndex != lastIndex)
						Array.Copy(readLines.ToArray(), footerIndex + 1, userLines[1], 0, lastIndex - footerIndex);
				}
				else if (((headerIndex == -1) ^ (footerIndex == -1)) || (footerIndex < headerIndex))
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