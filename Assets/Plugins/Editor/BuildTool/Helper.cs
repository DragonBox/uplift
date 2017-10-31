using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

namespace BuildTool
{
	public class Helper
	{
		public static bool IsMac() {
			return Application.platform == RuntimePlatform.OSXEditor;
		}

		public static bool IsWindows() {
			return Application.platform == RuntimePlatform.WindowsEditor;
		}

		public static string PathCombine(params string[] values) {
			return string.Join (Path.DirectorySeparatorChar.ToString (), values);
		}

		public static string ArgEscape(string s) {
			if (s.Contains (" ")) {
				string temp;
				if(Helper.IsWindows())
				{
					temp = "\"" + s + "\"";
				}
				else
				{
					temp = s.Replace(" ", "\\ ");
				}
				return temp;
			}
			return s;
		}

		public static void RunProcess(string exeName, string[] args) {
			if(Helper.IsWindows())
				RunCommand(
					"cmd.exe",
					new string[] { "/c \"" + exeName + " " + string.Join(" ", args) + "\"" }
				);
			else
			{
				RunCommand(exeName, args);
			}
		}
	
		public static void RunCommand(string exeName, string[] args) {
			using (var process = new System.Diagnostics.Process ()) {
				process.StartInfo.FileName = exeName;
				process.StartInfo.Arguments = string.Join(" ", args);
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;

				StringBuilder stdout = new StringBuilder ();
				StringBuilder stderr = new StringBuilder ();

				process.OutputDataReceived += (sender, e) => {
					if (e.Data == null)
						return;
					stdout.Append (e.Data).Append ("\n");
				};
				process.ErrorDataReceived += (sender, e) => {
					if (e.Data == null)
						return;
					stderr.Append (e.Data).Append ("\n");
				};
				process.EnableRaisingEvents = true;

				Debug.Log ("Running " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

				process.Start ();

				process.BeginOutputReadLine ();
				process.BeginErrorReadLine ();

				process.WaitForExit ();

				string output = stdout.ToString ();
				if (output.Length > 0)
					Debug.Log ("** output: \n" + output);
				string error = stderr.ToString ();
				if (error.Length > 0)
					Debug.Log ("** error: \n" + error);

				if (process.ExitCode != 0) {
					throw new Exception ("Failed running " + exeName + " error: " + process.ExitCode + " : " + error);
				}
			}
		}
	}
}

