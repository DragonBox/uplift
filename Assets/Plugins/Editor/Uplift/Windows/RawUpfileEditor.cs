using System;
using System.Collections.Generic;
using Uplift.Schemas;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;

namespace Uplift.Windows
{
    public class RawUpfileEditor : EditorWindow
    {
        private Upfile upfile;
        private Vector2 scrollPosition;
        private string upfileText;

        protected void OnGUI()
        {
            titleContent.text = "Edit Upfile (raw)";
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            if(upfileText == null)
                upfileText = System.IO.File.ReadAllText(Upfile.upfilePath);

            upfileText = EditorGUILayout.TextArea(upfileText);

            EditorGUILayout.EndScrollView();
            
            if(GUILayout.Button("Save Upfile"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Upfile));
                using(StringReader reader = new StringReader(upfileText))
                {
                    (serializer.Deserialize(reader) as Upfile).SaveFile();
                }
            }
        }
    }
}