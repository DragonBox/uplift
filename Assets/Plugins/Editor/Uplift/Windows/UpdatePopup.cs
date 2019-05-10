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

using System.Linq;
using UnityEditor;
using UnityEngine;
using Uplift.GitHubModule;
using Uplift.Updating;

namespace Uplift.Windows
{
	public class UpdatePopup : EditorWindow
	{
		private GitHubRelease[] releases;
		private Vector2 scrollPosition;
		private readonly string updateMessage = @"Uplift has been updated!
We have detected that you run an outdated version of Uplift, and you can update it.";

		public void SetReleases(GitHubRelease[] releases)
		{
			this.releases = releases;
			Repaint();
		}

		public void OnGUI()
		{
#if UNITY_5_1_OR_NEWER
			titleContent.text = "Update Uplift";
#endif
			EditorGUILayout.LabelField("Update available", EditorStyles.largeLabel, GUILayout.Height(25f));
			EditorGUILayout.HelpBox(updateMessage, MessageType.Warning);

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			foreach (GitHubRelease release in releases)
			{
				EditorGUILayout.LabelField("Release notes for version " + release.tag, EditorStyles.boldLabel);
				EditorGUILayout.HelpBox(release.body, MessageType.None);

				if (release.assets != null && release.assets.Any(asset => asset.name.EndsWith(".unitypackage")))
				{
					if (GUILayout.Button("Update to this version"))
					{
						Updater.UpdateUplift(release.assets.First(asset => asset.name.EndsWith(".unitypackage")).htmlURL);
					}
				}
				else
				{
					EditorGUILayout.HelpBox("This release seems to have no .unitypackage attached to it", MessageType.Info);
				}
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndScrollView();
		}
	}
}