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
