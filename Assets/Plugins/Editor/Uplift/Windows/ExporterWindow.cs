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

using System.IO;
using UnityEditor;
using UnityEngine;
using Uplift.Export;

namespace Uplift.Windows
{
	public class ExporterWindow : EditorWindow
	{
		private struct PackageInfoHelper
		{
			public PackageExportData exportSpec;
			public bool selected;
		}

		// TODO: Make Window scrollable if too many lines
		// QUESTION: UI has not been thought in term of effiency, should we rework it?
		private PackageInfoHelper[] potentialPackages;
		private bool[] expanded;
		private const bool defaultExpanded = true;

		public void Init()
		{
			string[] directories = System.IO.Directory.GetDirectories(GetSelectedPathOrFallback());

			potentialPackages = new PackageInfoHelper[directories.Length];
			expanded = new bool[directories.Length];

			string path;
			for (int i = 0; i < directories.Length; i++)
			{
				// TODO: Instead of assuming from context we should try to fetch existing Upfile.xml and parse info from it
				path = directories[i];

				potentialPackages[i] = new PackageInfoHelper()
				{
					selected = true
				};

				potentialPackages[i].exportSpec = new PackageExportData()
				{
					pathsToExport = new string[] { path },
					packageName = System.IO.Path.GetFileName(path),
					packageVersion = "0.0.1",
					license = "Undefined"

				};

				expanded[i] = defaultExpanded;
			}
		}

		public void OnGUI()
		{
#if UNITY_5_1_OR_NEWER
			titleContent.text = "Export Utility";
#endif
			for (int i = 0; i < potentialPackages.Length; i++)
			{
#if !UNITY_5_5_OR_NEWER
				expanded[i] = EditorGUILayout.Foldout (expanded[i], potentialPackages[i].exportSpec.pathsToExport[0]);
#else
				expanded[i] = EditorGUILayout.Foldout(expanded[i], potentialPackages[i].exportSpec.pathsToExport[0], true);
#endif
				if (expanded[i])
				{
					PackageExportData ed = potentialPackages[i].exportSpec;

					potentialPackages[i].selected = EditorGUILayout.Toggle("Export?", potentialPackages[i].selected);
					GUI.enabled = potentialPackages[i].selected;

					ed.packageName = EditorGUILayout.TextField("Package Name", ed.packageName);
					ed.packageVersion = EditorGUILayout.TextField("Package Version", ed.packageVersion);
					ed.license = EditorGUILayout.TextField("Package License", ed.license);

					GUI.enabled = true;
				}

				EditorGUILayout.Space();
			}

			if (GUILayout.Button("Export selected packages"))
			{
				foreach (PackageInfoHelper pInfoHelper in potentialPackages)
				{
					if (!pInfoHelper.selected) continue;

					Exporter exporter = new Exporter();
					exporter.SetExportSpec(pInfoHelper.exportSpec);
					exporter.Export();
				}
			}
		}

		public void OnInspectorUpdate()
		{
			this.Repaint();
		}

		private string GetSelectedPathOrFallback()
		{
			string path = "Assets";

			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}
			return path;
		}

	}
}