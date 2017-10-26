using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace BuildTool
{
	public class Helper
	{
		public static bool isMac() {
			return Application.platform == RuntimePlatform.OSXEditor;
		}

		public static string PathCombine(params string[] values) {
			return string.Join (Path.DirectorySeparatorChar.ToString (), values);
		}

		public static string ArgEscape(string s) {
			// FIXME for spaces
			if (s.Contains (" ")) {
				Debug.LogWarning ("ArgEscape not yet implemented yet file " + s + " contains spaces");
			}
			return s;
		}
	
		public static void RunCommand(string ExeName, string[] args) {
			using (var process = new System.Diagnostics.Process ()) {
				process.StartInfo.FileName = ExeName;
				process.StartInfo.Arguments = string.Join (" ", args);
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;

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
					throw new Exception ("Failed running " + ExeName + " error: " + process.ExitCode + " : " + error);
				}
			}
		}}
}

