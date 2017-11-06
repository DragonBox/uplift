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
            foreach(string author in About.Authors)
                EditorGUILayout.LabelField(author);
            EditorGUILayout.Space();
            EditorStyles.label.normal.textColor = Color.blue;
            if(GUILayout.Button("Find Uplift on Github!", EditorStyles.label))
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