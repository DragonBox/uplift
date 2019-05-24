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
using Uplift.Schemas;

namespace Uplift.Windows
{
	internal class UpdateUtility : EditorWindow
	{
		private Vector2 scrollPosition;
		private UpliftManager.DependencyState[] states = new UpliftManager.DependencyState[0];

		public void Init()
		{
			UpliftManager.ResetInstances();
			states = UpliftManager.Instance().GetDependenciesState();
			Repaint();
		}

		protected void OnGUI()
		{
#if UNITY_5_1_OR_NEWER
			titleContent.text = "Update Utility";
#endif
			EditorGUILayout.HelpBox("Please note that this window is not currently supported, and still experimental. Using it may cause unexpected issues. Use with care.", MessageType.Warning);

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			foreach (UpliftManager.DependencyState state in states)
			{
				if (state.transitive)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical();
				}

				DependencyStateBlock(
					state.definition,
					state.bestMatch,
					state.latest,
					state.installed
				);

				if (state.transitive)
				{
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndScrollView();
		}

		private void DependencyStateBlock(
			DependencyDefinition definition,
			PackageRepo bestMatch,
			PackageRepo latest,
			InstalledPackage installed
		)
		{
			EditorGUILayout.LabelField(definition.Name + ":", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Requirement: " + definition.Requirement.ToString());
			if (installed != null)
			{
				EditorGUILayout.LabelField("Installed version: " + installed.Version);

				if (VersionParser.GreaterThan(bestMatch.Package.PackageVersion, installed.Version))
				{
					EditorGUILayout.HelpBox(
						string.Format(
							"Package is outdated. You can update it to {0} (from {1})",
							bestMatch.Package.PackageVersion,
							bestMatch.Repository.ToString()
						),
						MessageType.Info
					);
					if (GUILayout.Button("Update to version " + bestMatch.Package.PackageVersion))
					{
						UpliftManager.Instance().UpdatePackage(bestMatch, updateLockfile: true);
						Init();
						Repaint();
					}
				}
				else
					EditorGUILayout.HelpBox("Package is up to date!", MessageType.Info);

				if (!definition.Requirement.IsMetBy(installed.Version))
					EditorGUILayout.HelpBox(
						"The version of the package currently installed does not match the requirements of your project!",
						installed.Version != bestMatch.Package.PackageVersion ? MessageType.Warning : MessageType.Error
					);
			}
			else
				EditorGUILayout.LabelField("Not yet installed");

			if (latest.Package.PackageVersion != bestMatch.Package.PackageVersion)
				EditorGUILayout.HelpBox(
					string.Format(
						"Note: there is a more recent version of the package ({0} from {1}), but it doesn't match your requirement",
						latest.Package.PackageVersion,
						bestMatch.Repository.ToString()
					),
					MessageType.Info
				);
		}

		public void OnInspectorUpdate()
		{
			this.Repaint();
		}
	}
}