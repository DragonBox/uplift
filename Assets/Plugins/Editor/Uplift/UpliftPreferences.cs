using UnityEditor;
using UnityEngine;

namespace Uplift
{
    public class UpliftPreferences : MonoBehaviour
    {
        private static bool prefsLoaded = false;
        private static readonly string useExperimentalFeaturesKey = "UpliftExperimentalFeatures";

        private static bool useExperimentalFeatures;

        [PreferenceItem("Uplift")]
        public static void PreferencesGUI()
        {
            if(!prefsLoaded)
            {
                useExperimentalFeatures = EditorPrefs.GetBool(useExperimentalFeaturesKey, false);
                prefsLoaded = true;
            }

            EditorGUILayout.HelpBox(
                "Experimental features are not thoroughly tested and could induce bugs. Use at your own risk!",
                MessageType.Warning
            );
            useExperimentalFeatures = EditorGUILayout.Toggle("Use experimental features", useExperimentalFeatures);

            if(GUI.changed)
                EditorPrefs.SetBool(useExperimentalFeaturesKey, useExperimentalFeatures);
        }

        public static bool UseExperimental()
        {
            return EditorPrefs.GetBool(useExperimentalFeaturesKey, false);
        }
    }
}