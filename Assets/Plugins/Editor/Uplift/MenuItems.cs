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

using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Export;
using Uplift.Schemas;
using Uplift.Updating;
using Uplift.Windows;

namespace Uplift
{
	public class MenuItems : MonoBehaviour
	{
		static MenuItems()
		{

		}

		[MenuItem("Tools/Uplift/Check Dependencies", false, 1)]
		private static void CheckDependencies()
		{
			using (LogAggregator LA = LogAggregator.InUnity(
				"Dependencies state:",
				"Dependencies state:",
				"Dependencies state:"
				))
			{
				UpliftManager.DependencyState[] states = UpliftManager.Instance().GetDependenciesState();

				foreach (UpliftManager.DependencyState state in states)
				{
					string message = string.Format("Package {0} ({1}) ", state.definition.Name, state.definition.Version);
					if (state.installed != null)
					{
						if (state.installed.Version != state.bestMatch.Package.PackageVersion)
							message += string.Format(
								"is outdated. Best available version is {0} (from {1})",
								state.bestMatch.Package.PackageVersion,
								state.bestMatch.Repository.ToString()
							);
						else
							message += "is up to date.";

						if (!state.definition.Requirement.IsMetBy(state.installed.Version))
							message += "\nWarning: the package currently installed does not match your requirements";

					}
					else
						message += string.Format(
							"is not installed. Best available version is {0} (from {1})",
							state.bestMatch.Package.PackageVersion,
							state.bestMatch.Repository.ToString()
						);

					if (state.latest.Package.PackageVersion != state.bestMatch.Package.PackageVersion)
						message += string.Format(
							"\nNote: there is a more recent version of the package ({0} from {1}), but it doesn't match your requirement",
							state.latest.Package.PackageVersion,
							state.bestMatch.Repository.ToString()
						);

					if (state.transitive)
						message = "`--> " + string.Join("    \n", message.Split('\n'));

					Debug.Log(message);
				}
			}
		}


		[MenuItem("Tools/Uplift/Install dependencies (as specified in lockfile)", false, 2)]
		private static void InstallLockfile()
		{
			Debug.Log("Installing from Lockfile : ");
			UpliftManager.ResetInstances();
			UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.ONLY_LOCKFILE);

			Debug.Log("-> Resfreshing AssetDatabase");
			AssetDatabase.Refresh();
		}

		[MenuItem("Tools/Uplift/Update dependencies", false, 3)]
		private static void InstallDependencies()
		{
			UpliftManager.ResetInstances();
			UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.INCOMPLETE_LOCKFILE);

			Debug.Log("-> Resfreshing AssetDatabase");
			AssetDatabase.Refresh();
		}

		[MenuItem("Tools/Uplift/Show Update Window (experimental)", true, 4)]
		private static bool EnableShowUpdateWindow()
		{
			return UpliftPreferences.FromDefaultFile().UseExperimentalFeatures;
		}

		[MenuItem("Tools/Uplift/Show Update Window (experimental)", false, 4)]
		private static void ShowUpdateWindow()
		{
			UpdateUtility window = EditorWindow.GetWindow(typeof(UpdateUtility)) as UpdateUtility;
			window.Init();
		}

		[MenuItem("Tools/Uplift/Edit Upfile (experimental)", true, 101)]
		private static bool EnableEditUpfile()
		{
			return UpliftPreferences.FromDefaultFile().UseExperimentalFeatures;
		}

		[MenuItem("Tools/Uplift/Edit Upfile (experimental)", false, 101)]
		private static void EditUpfile()
		{
			EditorWindow.GetWindow(typeof(UpfileEditor));
		}

		[MenuItem("Tools/Uplift/Packaging/Create Export Package Definition", false, 201)]
		private static void CreatePackageExportData()
		{

			PackageExportData asset = ScriptableObject.CreateInstance<PackageExportData>();

			AssetDatabase.CreateAsset(asset, "Assets/PackageExport.asset");
			AssetDatabase.SaveAssets();

			//Show Asset in Inspector
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/PackageExport.asset");
		}

		[MenuItem("Tools/Uplift/Packaging/Export Defined Packages", false, 202)]
		private static void ExportPackage()
		{
			Exporter.PackageEverything();
		}

		[MenuItem("Tools/Uplift/Packaging/Export Package Utility", false, 203)]
		private static void ExportPackageWindow()
		{
			ExporterWindow window = EditorWindow.GetWindow(typeof(ExporterWindow), true) as ExporterWindow;
			window.Init();
			window.Show();
		}


		[MenuItem("Tools/Uplift/Debug/Refresh Upfile", false, 1001)]
		private static void RefreshUpfile()
		{
			UpliftManager.ResetInstances();
			Debug.Log("Upfile refreshed");
		}

		[MenuItem("Tools/Uplift/Debug/Nuke All Packages", false, 1002)]
		private static void NukePackages()
		{
			Debug.LogWarning("Nuking all packages!");
			UpliftManager.Instance().NukeAllPackages();
			AssetDatabase.Refresh();
		}

		[MenuItem("Tools/Uplift/About Uplift", false, 2000)]
		private static void AboutUplift()
		{
			EditorWindow.GetWindow(typeof(AboutWindow));
		}

		[MenuItem("Tools/Uplift/Try to update Uplift", false, 2001)]
		private static void TryUpdateUplift()
		{
			Updater.CheckForUpdate(true);
		}

		[MenuItem("Tools/Uplift/Edit settings", false, 2002)]
		private static void EditSettings()
		{
			EditorWindow.GetWindow(typeof(SettingsWindow));
		}
	}
}
