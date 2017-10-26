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

		[MenuItem("Uplift/Build/BuildAndExportDll", false, 0)]
		public static void BuildPackage() {
			BuildUpliftDll ();
			PrepareExportArea();
		}

		[MenuItem("Uplift/Build/BuildDll", false, 1)]
		public static void BuildUpliftDll() {
			UnityInstallation unity = UnityInstallationUtils.Current ();

			BuildLibraryData Data = new BuildLibraryData ();

			Data.References = new string[] {
				unity.Paths().Managed("UnityEditor.dll"),
				unity.Paths().Managed("UnityEngine.dll"),
				Helper.PathCombine("Assets", "Plugins", "Editor", "SharpCompress.dll")
			};

			// All under Uplift except Testing code
			List<string> FileList = new List<string> ();
			FileList.AddRange (FileSystemUtil.GetFiles (Helper.PathCombine ("Assets", "Plugins", "Editor", "Uplift")).Where (f => f.EndsWith(".cs") && !f.Contains ("Testing")));
			FileList.AddRange (FileSystemUtil.GetFiles (Helper.PathCombine ("Assets", "Plugins", "Editor", "BuildTool")).Where (f => f.EndsWith(".cs")));
			FileList.AddRange (FileSystemUtil.GetFiles (Helper.PathCombine ("Assets", "Plugins", "Editor", "UnityHacks")).Where (f => f.EndsWith(".cs")));
			Data.Files = FileList.ToArray ();

			Data.SdkLevel = 2;
			Data.OutFile = UpliftDLL;

			unity.BuildLibrary(Data);
		}

		private static void PrepareExportArea() {
			string PackingDir = "Build";
			string EditorDir = Helper.PathCombine ("Assets", "Plugins", "Editor");
			string EditorPackingDir = Helper.PathCombine (PackingDir, EditorDir);
			FileSystemUtil.EnsureDirExists (EditorPackingDir);

			// copy packaged dependencies from current project into packing dir
			string[] DllAndMeta = System.IO.Directory.GetFiles(EditorDir, "SharpCompress.dll*", SearchOption.AllDirectories);
			foreach (string path in DllAndMeta) {
				CopyFileExactly(path, Helper.PathCombine (PackingDir, path));
			}

			string UpliftDir = Helper.PathCombine ("Assets", "Plugins", "Editor", "Uplift");
			string UpliftPackingDir = Helper.PathCombine (PackingDir, UpliftDir);
			FileSystemUtil.EnsureDirExists (UpliftPackingDir);
			CopyFileExactly(UpliftDLL, Helper.PathCombine(UpliftPackingDir, "Uplift.dll"));

			/*
			string[] exportEntries = ListAllFiles (EditorDir);
			AssetDatabase.ExportPackage(exportEntries,
				Path.Combine("target", "Uplift.unitypackage"),
				ExportPackageOptions.Default);
				*/
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
		/*
		private static string[] ListAllFiles(params string[] paths) {
			var exportEntries = new List<string>();
			for (int i = 0; i < paths.Length; i++) {

				string path = paths [i];

				if (System.IO.File.Exists (path)) {
					exportEntries.Add (path);

				} else if (System.IO.Directory.Exists (path)) {
					string[] tFiles = System.IO.Directory.GetFiles (path, "*.*", SearchOption.AllDirectories);
					string[] tDirectories = System.IO.Directory.GetDirectories (path, "*", SearchOption.AllDirectories);

					exportEntries.AddRange (tFiles);
					exportEntries.AddRange (tDirectories);
				}
			}
			return exportEntries.ToArray ();
		}
		*/
	}
}