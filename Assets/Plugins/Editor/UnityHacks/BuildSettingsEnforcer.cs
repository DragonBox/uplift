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
