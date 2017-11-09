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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Uplift.Common;

using System.Diagnostics;
using System.Text;
using System.Linq;

namespace BuildTool {
	public class UnityInstallation {
		string rootPath;
		BuildPaths buildPaths;

		public UnityInstallation(string rootPath) {
			this.rootPath = rootPath;
		}

		public string RootPath {
			get {
				return rootPath;
			}
		}

		public BuildPaths Paths() {
			if (buildPaths == null) {
				buildPaths = new BuildPaths (this);
			}
			return buildPaths;
		}

		public void BuildLibrary(BuildLibraryData data) {
			FileSystemUtil.EnsureParentExists (data.OutFile);

			string ReferenceString = string.Join(",", data.References.Select(s => Helper.ArgEscape(s)).ToArray());

			List<string> args = new List<string>();
			args.Add ("-r:" + ReferenceString);
			args.Add ("-target:library");
			args.Add ("-sdk:" + data.SdkLevel.ToString ());
			args.Add ("-out:" + data.OutFile);
			args.AddRange (data.Files);

			Helper.RunProcess (Helper.ArgEscape(Paths().Mcs ()), args.ToArray ()); 
			UnityEngine.Debug.LogFormat("Library '{0}' built!", data.OutFile);
		}
	}

	public class UnityInstallationUtils {
		public static UnityInstallation Current() {
			string exe = System.Environment.GetCommandLineArgs ()[0];
			string RootPath;
			if (Helper.IsMac()) {
				RootPath = exe.Substring(0, exe.IndexOf("/Unity.app"));
			} else if (Helper.IsWindows()) {
				DirectoryInfo exeInfo = new DirectoryInfo(exe);
				RootPath = exeInfo.Parent.Parent.FullName;
			} else {
				throw new System.Exception ("TODO: Implement RootPath finder from " + exe);
			}
			UnityEngine.Debug.Log ("RootPath: " + RootPath);
			return new UnityInstallation (RootPath);
		}
	}

}
