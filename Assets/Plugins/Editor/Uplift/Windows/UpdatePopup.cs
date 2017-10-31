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
using Uplift.Updating;

namespace Uplift.Windows
{
    public class UpdatePopup : EditorWindow
    {
        private string newerVersion, downloadUrl, updateBody;
        private readonly string updateMessage = @"Uplift has been updated to version {0}
We have detected that you run an outdated version of Uplift, and you can update it.";
        public void SetInformations(string newerVersion, string downloadUrl, string updateBody)
        {
            this.newerVersion = newerVersion;
            this.downloadUrl = downloadUrl;
            this.updateBody = updateBody;
            Repaint();
        }
        public void OnGUI()
        {
#if UNITY_5_1_OR_NEWER
            titleContent.text = "Update Uplift";
#endif
            EditorGUILayout.LabelField("Update available", EditorStyles.largeLabel, GUILayout.Height(25f));
            EditorGUILayout.HelpBox(string.Format(updateMessage, newerVersion), MessageType.Warning);

            EditorGUILayout.LabelField("Release notes for version " + newerVersion, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(updateBody, MessageType.None);

            if(GUILayout.Button("Udpate Uplift"))
            {
                Updater.UpdateUplift(downloadUrl);
            }
        }
    }
}
