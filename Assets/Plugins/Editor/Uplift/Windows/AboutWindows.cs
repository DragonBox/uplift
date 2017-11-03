using UnityEditor;
using UnityEngine;

namespace Uplift.Windows
{
    public class AboutWindow : EditorWindow
    {
        public void OnGUI()
        {
            titleContent.text = "About Uplift";

            EditorGUILayout.LabelField("Uplift", EditorStyles.largeLabel, GUILayout.Height(25f));
            EditorGUILayout.LabelField("Version " + About.Version, EditorStyles.label);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Authors:", EditorStyles.boldLabel);
            foreach(string author in About.Authors)
                EditorGUILayout.LabelField(author);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Find Uplift on Github: " + About.Repository);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(About.Copyright);
            EditorGUILayout.LabelField(About.License);
        }
    }
}