using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Windows;
using Uplift.Schemas;
using Uplift.Packages;

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
            Upfile.InitializeInstance();
        }

        [MenuItem("Uplift/Generate Upfile", true, 0)]
        private static bool CheckForUpfile()
        {
            return !Upfile.CheckForUpfile();
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
            UpliftManager.Instance().InstallDependencies();
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
            foreach(Upset package in Upfile.Instance().ListPackages())
            {
                Debug.Log("Package: " + package.PackageName + " Version: " + package.PackageVersion);
            }
        }

        [MenuItem("Uplift/Debug/Nuke All Packages", false, 50)]
        private static void NukePackages()
        {
            Debug.LogWarning("Nuking all packages!");
            UpliftManager.Instance().NukeAllPackages();
            AssetDatabase.Refresh();
        }

        [MenuItem("Uplift/Debug/Install -> Nuke Loop %g", false, 10)]
        private static void RefreshPackages()
        {
            var manager = UpliftManager.Instance();
            Debug.ClearDeveloperConsole();
            Debug.Log("Doing full Install -> Nuke Loop");
            manager.InstallDependencies();
            AssetDatabase.Refresh();
            manager.NukeAllPackages();
            AssetDatabase.Refresh();
        }

    }
}
