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

namespace BuildTool {
	public class BuildPaths {
		private UnityInstallation unity;

		public BuildPaths(UnityInstallation unity) {
			this.unity = unity;
		}

		public string Managed(string file) {
			string RootPath = ManagedPath();
			string Path = Helper.PathCombine(RootPath, file);
			EnsureFilePath (Path);
			return Path;
		}

		private void EnsureFilePath(string path) {
			if (!File.Exists(path)) {
				throw new System.Exception("File " + path + " not found.");
			}
		}
		private void EnsureDirPath(string path) {
			if (!Directory.Exists(path)) {
				throw new System.Exception("Directory " + path + " not found.");
			}
		}

		public string ManagedPath() {
			string ManagedPath;
			if (Helper.IsMac ()) {
				ManagedPath = Helper.PathCombine(unity.RootPath, "Unity.app", "Contents", "Managed");
				if (!Directory.Exists(ManagedPath)) { // Pre 5.6
					ManagedPath = Helper.PathCombine(unity.RootPath, "Unity.app", "Contents", "Frameworks", "Managed");
				}
			} else {
				ManagedPath = Helper.PathCombine(unity.RootPath, "Editor", "Data", "Managed");
			}
			EnsureDirPath (ManagedPath);
			return ManagedPath;
		}

		public string Mcs() {
			string ManagedPath;
			if (Helper.IsMac ()) {
				ManagedPath = Helper.PathCombine(unity.RootPath, "Unity.app", "Contents", "MonoBleedingEdge", "bin", "mcs");
				if (!File.Exists(ManagedPath)) { // Pre 5.6
					ManagedPath = Helper.PathCombine(unity.RootPath, "Unity.app", "Contents", "Frameworks", "MonoBleedingEdge", "bin", "mcs");
				}
			} else {
				ManagedPath = Helper.PathCombine(unity.RootPath, "Editor", "Data", "MonoBleedingEdge", "bin", "mcs");
			}
			EnsureFilePath (ManagedPath);
			return ManagedPath;
		}
	}
}
