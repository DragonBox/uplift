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
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Schemas;

namespace Uplift.Windows
{
	public class RawUpfileEditor : EditorWindow
	{
		private Upfile upfile;
		private Vector2 scrollPosition;
		private string upfileText;

		protected void OnGUI()
		{
#if UNITY_5_1_OR_NEWER
			titleContent.text = "Edit Upfile (raw)";
#endif
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			if (upfileText == null)
				upfileText = System.IO.File.ReadAllText(Upfile.upfilePath);

			upfileText = EditorGUILayout.TextArea(upfileText);

			EditorGUILayout.EndScrollView();

			if (GUILayout.Button("Save Upfile"))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
				using (StringReader reader = new StringReader(upfileText))
				{
					(serializer.Deserialize(reader) as Upfile).SaveFile();
				}
			}
		}
	}
}