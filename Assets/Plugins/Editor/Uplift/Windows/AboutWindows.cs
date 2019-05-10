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

namespace Uplift.Windows
{
	public class AboutWindow : EditorWindow
	{
		public void OnGUI()
		{
			EditorStyles.label.normal.textColor = Color.black;
#if UNITY_5_1_OR_NEWER
			titleContent.text = "About Uplift";
#endif
			EditorGUILayout.LabelField("Uplift", EditorStyles.largeLabel, GUILayout.Height(25f));
			EditorGUILayout.LabelField("Version " + About.Version, EditorStyles.label);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Authors:", EditorStyles.boldLabel);
			foreach (string author in About.Authors)
				EditorGUILayout.LabelField(author);
			EditorGUILayout.Space();
			EditorStyles.label.normal.textColor = Color.blue;
			if (GUILayout.Button("Find Uplift on Github!", EditorStyles.label))
			{
				Application.OpenURL(About.GithubRepository);
			}
			EditorStyles.label.normal.textColor = Color.black;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(About.Copyright);
			EditorGUILayout.LabelField(About.License);
		}
	}
}