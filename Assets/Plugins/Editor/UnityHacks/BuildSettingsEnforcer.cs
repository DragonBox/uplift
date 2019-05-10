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

namespace UnityHacks
{
	/// <summary>
	/// Unity does not save correctly the scenes in Build Settings, resulting in errors when re-opening the project. The scenes are saved if some are added/removed or enabled/disabled, but changing the scene path or filename will not see its path saved correctly in the EditorBuildSettings.asset.
	/// A bug report has been opened: Case 954716
	/// </summary>
	public class BuildSettingsEnforcer
	{
		/// <summary>
		/// Forces the EditorBuildSettings.asset to be saved and keep track of the correct path of the scenes in the Build Settings.
		/// </summary>
		public static void EnforceAssetSave()
		{
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
			if (scenes.Length == 0) return;

			scenes[0].enabled = !scenes[0].enabled;
			EditorBuildSettings.scenes = scenes;
			AssetDatabase.Refresh();

			AssetDatabase.SaveAssets();

			scenes[0].enabled = !scenes[0].enabled;
			EditorBuildSettings.scenes = scenes;
			AssetDatabase.Refresh();

			AssetDatabase.SaveAssets();
		}
	}
}