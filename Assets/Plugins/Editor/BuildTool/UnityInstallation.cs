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
			string targetDir = Path.GetDirectoryName (data.OutFile);
			FileSystemUtil.EnsureParentExists (data.OutFile);

			string ReferenceString = string.Join(",", data.References.Select(s => Helper.ArgEscape(s)).ToArray());

			List<string> args = new List<string>();
			args.Add ("-r:" + ReferenceString);
			args.Add ("-target:library");
			args.Add ("-sdk:" + data.SdkLevel.ToString ());
			args.Add ("-out:" + data.OutFile);
			args.AddRange (data.Files);

			Helper.RunCommand (Helper.ArgEscape(Paths().Mcs ()), args.ToArray ()); 
			UnityEngine.Debug.LogFormat("Library '{0}' built!", data.OutFile);
		}
	}

	public class UnityInstallationUtils {
		public static UnityInstallation Current() {
			string exe = System.Environment.GetCommandLineArgs ()[0];
			string RootPath;
			if (Helper.isMac()) {
				UnityEngine.Debug.Log(exe);
				RootPath = exe.Substring(0, exe.IndexOf("/Unity.app"));
			} else {
				throw new System.Exception ("TODO: Implement RootPath finder from " + exe);
			}
			UnityEngine.Debug.Log ("RootPath: " + RootPath);
			return new UnityInstallation (RootPath);
		}
	}

}