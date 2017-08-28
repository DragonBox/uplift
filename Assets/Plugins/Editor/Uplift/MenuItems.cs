using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Windows;
using Uplift.Schemas;
using Uplift.Packages;
using Uplift.Common;
using System.Linq;

namespace Uplift
{
    public class MenuItems : MonoBehaviour
    {
        private static string[] _sampleUpliftLocation = {"Assets", "Plugins", "Editor", "Uplift", "Schemas", "Upfile.Sample.xml"};
        
        static MenuItems()
        {

        }

        [MenuItem("Uplift/Refresh Upfile", false, 0)]
        private static void RefreshUpfile()
        {
            Upfile.InitializeInstance();
        }


        [MenuItem("Uplift/Show Update Window", false, 1)]
        private static void ShowUpdateWindow()
        {
            EditorWindow.GetWindow(typeof(UpdateUtility));
        }

        [MenuItem("Uplift/Generate Upfile", true, 101)]
        private static bool CheckForUpfile()
        {
            return !Upfile.CheckForUpfile();
        }

        [MenuItem("Uplift/Generate Upfile", false, 101)]
        private static void GenerateUpfile()
        {

            XmlDocument sampleFile = new XmlDocument();

            sampleFile.Load(string.Join(Path.DirectorySeparatorChar.ToString(), _sampleUpliftLocation));

            XmlNode versionNode = sampleFile.SelectSingleNode("/Upfile/UnityVersion");
            
            versionNode.InnerText = Application.unityVersion.ToString();

            sampleFile.Save("Upfile.xml");
            
        }

        [MenuItem("Uplift/Check Dependencies", false, 102)]
        private static void CheckDependencies()
        {
            Upbring upbring = Upbring.Instance();
            Upfile upfile = Upfile.Instance();
            PackageList packageList = PackageList.Instance();
            PackageRepo[] packageRepos = packageList.GetAllPackages();

            bool any_installed =
                        upbring.InstalledPackage != null &&
                        upbring.InstalledPackage.Length != 0;

            foreach (DependencyDefinition dependency in upfile.Dependencies)
            {
                string name = dependency.Name;
                bool installed =
                        any_installed &&
                        upbring.InstalledPackage.Any(ip => ip.Name == name);
                bool installable = packageRepos.Any(pr => pr.Package.PackageName == name);
                string latest = installable ? packageList.GetLatestPackage(name).Package.PackageVersion : "";
                string string_latest = string.IsNullOrEmpty(latest) ? "No version available in any repository" : "Latest version is " + latest;
                if (installed)
                {
                    string installed_version = upbring.GetInstalledPackage(name).Version;
                    if (installed_version != latest)
                    {
                        Debug.Log(string.Format("Package {0} is outdated: installed version is {1}, latest is {2}", name, installed_version, string_latest));
                    }
                    else
                    {
                        Debug.Log(string.Format("Package {0} is up-to-date ({1})", name, installed_version));
                    }
                }
                else
                {
                    Debug.Log(string.Format("Package {0} is not installed ({1})", name, string_latest));
                }
            }
        }

        [MenuItem("Uplift/Install Dependencies", false, 103)]
        private static void InstallDependencies()
        {
            UpliftManager.Instance().InstallDependencies();
            AssetDatabase.Refresh();
        }

        [MenuItem("Uplift/Debug/Nuke All Packages", false, 152)]
        private static void NukePackages()
        {
            Debug.LogWarning("Nuking all packages!");
            UpliftManager.Instance().NukeAllPackages();
            AssetDatabase.Refresh();
        }
    }
}
