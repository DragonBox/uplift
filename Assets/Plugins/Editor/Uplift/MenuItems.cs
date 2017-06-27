using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Windows;
using Uplift.Schemas;

namespace Uplift
{
    public class MenuItems : MonoBehaviour
    {
        private static string[] _sampleUpliftLocation = {"Assets", "Plugins", "Editor", "Uplift", "Schemas", "Upfile.Sample.xml"};
        
        static MenuItems()
        {

        }

        [MenuItem("Uplift/Refresh Upfile", false, -15)]
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

            XmlDocument sampleFile = new XmlDocument();

            sampleFile.Load(string.Join(Path.DirectorySeparatorChar.ToString(), _sampleUpliftLocation));

            XmlNode versionNode = sampleFile.SelectSingleNode("/Upfile/UnityVersion");
            
            versionNode.InnerText = Application.unityVersion.ToString();

            sampleFile.Save("Upfile.xml");
            
        }

        [MenuItem("Uplift/Check Dependencies", false, 20)]
        private static void CheckDependencies()
        {
            Debug.Log("Do nothing right now");
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
            EditorWindow.GetWindow(typeof(UpdateUtility));
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
