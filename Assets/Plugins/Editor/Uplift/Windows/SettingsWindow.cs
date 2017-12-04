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

using Uplift.Schemas;
using Uplift.Common;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

namespace Uplift.Windows
{
    public class SettingsWindow : EditorWindow
    {
        private UpliftSettings settings;
        private Vector2 scrollPosition;
        private string settingsText;

        protected void OnGUI()
        {
#if UNITY_5_1_OR_NEWER
            titleContent.text = "Edit settings";
#endif
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (!File.Exists(UpliftSettings.GetDefaultLocation()))
            {
                EditorGUILayout.HelpBox("It seems that you do not have a settings.xml file under HOME/.uplift.", MessageType.Warning);
                if(GUILayout.Button("Create a sample settings file"))
                {
                    SampleFile.CreateSampleSettingsFile();
                }
            }
            else
            {
                if (settingsText == null)
                    settingsText = System.IO.File.ReadAllText(UpliftSettings.GetDefaultLocation());

                settingsText = EditorGUILayout.TextArea(settingsText);

                if (GUILayout.Button("Save settings file"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.InnerXml = settingsText;
                    doc.Save(UpliftSettings.GetDefaultLocation());
                    Repaint();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
