using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Uplift.Common;
using System.Linq;

namespace BuildTool {
	public class DllCompiler {
		[MenuItem("Uplift/DllCompiler/Test", false, 0)]
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
			Data.OutFile = "target/Uplift.dll";

			unity.BuildLibrary(Data);
		}
	}
}