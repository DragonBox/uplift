using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Windows;
using Uplift.Schemas;

namespace Uplift
{
    public class MenuItems : MonoBehaviour
    {


        static MenuItems()
        {

        }

        [MenuItem("Uplift/Refresh", false, -15)]
        private static void RefreshUpfile()
        {
            UpfileHandler.Instance().Initialize();
        }


        [MenuItem("Uplift/Generate Upfile", true, 0)]
        private static bool CheckForUpfile()
        {
            return !UpfileHandler.Instance().CheckForUpfile();
        }
        [MenuItem("Uplift/Generate Upfile", false, 0)]
        private static void GenerateUpfile()
        {
            Debug.Log("Hi, I Generate upfile!");

            Upfile upfile = new Upfile {UnityVersion = Application.unityVersion};


            XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Upfile));
            serializer.Serialize(new FileStream(UpfileHandler.upfilePath, FileMode.CreateNew), upfile);
            Debug.Log("Done");
        }

        [MenuItem("Uplift/Check Dependencies", false, 20)]
        private static void CheckDependencies()
        {

        }

        [MenuItem("Uplift/Install Dependencies", false, 20)]
        private static void InstallDependencies()
        {
            UpfileHandler.Instance().InstallDependencies();
            AssetDatabase.Refresh();
        }


        [MenuItem("Uplift/Show Update Window", false, 30)]
        private static void ShowUpdateWindow()
        {
            UpdateUtility uw = new UpdateUtility();
            uw.ShowUtility();
            //EditorWindow.GetWindow(typeof(UpdateUtility));
        }

        [MenuItem("Uplift/Debug/List Packages", false, 50)]
        private static void ListPackages()
        {
            UpfileHandler.Instance().ListPackages();
        }

        [MenuItem("Uplift/Debug/Nuke All Packages", false, 50)]
        private static void NukePackages()
        {
            UpfileHandler.Instance().NukePackages();
            AssetDatabase.Refresh();
        }

    }
}