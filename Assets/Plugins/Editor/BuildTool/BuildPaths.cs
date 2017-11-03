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
			} else {
				ManagedPath = Helper.PathCombine(unity.RootPath, "Editor", "Data", "MonoBleedingEdge", "bin", "mcs");
			}
			EnsureFilePath (ManagedPath);
			return ManagedPath;
		}
	}
}