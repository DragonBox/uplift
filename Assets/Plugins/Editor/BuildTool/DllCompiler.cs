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
using UnityEditor;
using Uplift.Common;
using System.Linq;
using System.IO;

namespace BuildTool {
	public class DllCompiler {
		static string UpliftDLL = "target/Uplift.dll";

		[MenuItem("Tools/Uplift/Build/BuildAndExportDll", false, 311)]
		public static void BuildPackage() {
			BuildUpliftDll ();
			PrepareExportArea();
		}

		[MenuItem("Tools/Uplift/Build/BuildDll", false, 312)]
		public static void BuildUpliftDll() {
			UnityInstallation unity = UnityInstallationUtils.Current ();

			BuildLibraryData Data = new BuildLibraryData ();

			Data.References = new string[] {
				unity.Paths().Managed("UnityEditor.dll"),
				unity.Paths().Managed("UnityEngine.dll")
			};

			// All under Uplift except Testing code
			List<string> FileList = new List<string> ();
			FileList.AddRange (FileSystemUtil.GetFiles (Helper.PathCombine ("Assets", "Plugins", "Editor", "Uplift")).Where (f => f.EndsWith(".cs") && !f.Contains ("Testing")));
			// FileList.AddRange (FileSystemUtil.GetFiles (Helper.PathCombine ("Assets", "Plugins", "Editor", "BuildTool")).Where (f => f.EndsWith(".cs")));
			FileList.AddRange (FileSystemUtil.GetFiles (Helper.PathCombine ("Assets", "Plugins", "Editor", "UnityHacks")).Where (f => f.EndsWith(".cs")));
			Data.Files = FileList.ToArray ();

			Data.SdkLevel = 2;
			Data.OutFile = UpliftDLL;
            Data.useUnsafe = true;

			unity.BuildLibrary(Data);
		}

		private static void PrepareExportArea() {
			string PackingDir = "Build";
			string EditorDir = Helper.PathCombine ("Assets", "Plugins", "Editor");
			string EditorPackingDir = Helper.PathCombine (PackingDir, EditorDir);
			FileSystemUtil.EnsureDirExists (EditorPackingDir);

			string UpliftDir = Helper.PathCombine ("Assets", "Plugins", "Editor", "Uplift");
			string UpliftPackingDir = Helper.PathCombine (PackingDir, UpliftDir);
			FileSystemUtil.EnsureDirExists (UpliftPackingDir);
			CopyFileExactly(UpliftDLL, Helper.PathCombine(UpliftPackingDir, "Uplift.dll"));
		}

		public static void CopyFileExactly(string copyFromPath, string copyToPath)
		{
			var origin = new FileInfo(copyFromPath);

			origin.CopyTo(copyToPath, true);

			var destination = new FileInfo(copyToPath);
			destination.CreationTime = origin.CreationTime;
			destination.LastWriteTime = origin.LastWriteTime;
			destination.LastAccessTime = origin.LastAccessTime;
		}
	}
}
