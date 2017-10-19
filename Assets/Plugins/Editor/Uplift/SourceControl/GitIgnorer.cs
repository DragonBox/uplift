using System;
using System.Collections.Generic;
using System.IO;

namespace Uplift.SourceControl
{
	public class GitIgnorer : ISourceControlHandler
	{
		public delegate void FileEdit(object sender, string path);
		public event FileEdit OnEditGitignore;
		private readonly string ignoreTemplateHeader = "# == UPLIFT GITIGNORE START ==";
		private readonly string ignoreTemplateComment = "# This section of the .gitignore has been created automatically by Uplift. Do not modify it or remove it.";
		private readonly string ignoreTemplateFooter = "# == UPLIFT GITIGNORE END ==";
		public void HandleDirectory(string path)
		{
			Ignore(path, "*");
		}

		public void HandleFile(string path)
		{
			Ignore(
				Path.GetDirectoryName(path),
				Path.GetFileName(path)				
			);
		}

		private void Ignore(string path, string pattern)
		{
			string gitIgnorePath = Path.Combine(path, ".gitignore");
			string[] upliftPatterns;
			string[] userLines = ExtractExistingLines(gitIgnorePath, out upliftPatterns);
			string[] temp = new string[upliftPatterns.Length + 1];
			Array.Copy(upliftPatterns, temp, upliftPatterns.Length);
			temp[upliftPatterns.Length] = pattern;
			upliftPatterns = temp;

			using(StreamWriter sw = new StreamWriter(gitIgnorePath, false))
			{
				foreach(string line in userLines)
					sw.WriteLine(line);
				if(userLines.Length > 0) sw.WriteLine("\n");
				sw.WriteLine(ignoreTemplateHeader);
				sw.WriteLine(ignoreTemplateComment);
				foreach(string line in upliftPatterns)
				{
					sw.WriteLine(line);
				}
				sw.WriteLine(ignoreTemplateFooter);
			}

			OnEditGitignore(this, gitIgnorePath);
		}

		private string[] ExtractExistingLines(string path, out string[] upliftPatterns)
		{
			List<string> userLines = new List<string>();
			List<string> upliftLines = new List<string>();

			upliftPatterns = new string[0];

			if(File.Exists(path))
			{
				foreach(string line in File.ReadAllLines(path))
					userLines.Add(line);

				int headerIndex = userLines.IndexOf(ignoreTemplateHeader);
				int footerIndex = userLines.IndexOf(ignoreTemplateFooter);
				if(headerIndex != -1 && footerIndex != -1 && headerIndex < footerIndex)
				{
					for(int i = headerIndex + 1; i < footerIndex; i++)
					{
						if(userLines[i].StartsWith(ignoreTemplateComment)) continue;
						upliftLines.Add(userLines[i]);
					}

					userLines.RemoveRange(headerIndex, footerIndex - headerIndex + 1);
				}
				else if(((headerIndex == -1) ^ (footerIndex == -1)) || (footerIndex < headerIndex))
				{
					// One of the line is present, not the other one OR the footer is before the header
					UnityEngine.Debug.LogError((headerIndex == -1) ^ (footerIndex == -1));
					UnityEngine.Debug.LogError((footerIndex < headerIndex));
					UnityEngine.Debug.LogError(((headerIndex == -1) ^ (footerIndex == -1)) || (footerIndex < headerIndex));
					UnityEngine.Debug.LogError("headerIndex: " + headerIndex);
					UnityEngine.Debug.LogError("footerIndex: " + footerIndex);
					UnityEngine.Debug.LogError(string.Format("The .gitignore at {0} is not properly formed", path));
				}
			}

			upliftPatterns = upliftLines.ToArray();
			return userLines.ToArray();
		}
	}
}
