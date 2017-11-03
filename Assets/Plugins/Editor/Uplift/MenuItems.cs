using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Uplift.Windows;
using Uplift.Schemas;
using Uplift.Packages;
using Uplift.Common;
using System.Linq;

using Uplift.Export;


namespace Uplift
{
    public class MenuItems : MonoBehaviour
    {
        static MenuItems()
        {

        }

        [MenuItem("Tools/Uplift/Show Update Window", false, 1)]
        private static void ShowUpdateWindow()
        {
            EditorWindow.GetWindow(typeof(UpdateUtility));
        }

        [MenuItem("Tools/Uplift/Check Dependencies", false, 102)]
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

        [MenuItem("Tools/Uplift/Install Dependencies", false, 103)]
        private static void InstallDependencies()
        {
            Debug.Log("Installing Upfile dependencies");
            UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.INCOMPLETE_LOCKFILE, refresh: true);
            AssetDatabase.Refresh();
        }


        [MenuItem("Tools/Uplift/Debug/Refresh Upfile", false, 153)]
        private static void RefreshUpfile()
        {
            UpliftManager.ResetInstances();
            Debug.Log("Upfile refreshed");
        }

        [MenuItem("Tools/Uplift/Packaging/Create Export Package Definition", false, 200)]
        private static void CreatePackageExportData() {

            PackageExportData asset = ScriptableObject.CreateInstance<PackageExportData>();

            AssetDatabase.CreateAsset(asset, "Assets/PackageExport.asset");
            AssetDatabase.SaveAssets();

        }

        [MenuItem("Tools/Uplift/Packaging/Export Defined Packages", false, 201)]
        private static void ExportPackage() {
            Exporter.PackageEverything();
        }

        [MenuItem("Tools/Uplift/Debug/Install from lockfile", false, 154)]
        private static void InstallLockfile()
        {
            UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.ONLY_LOCKFILE, refresh: true);
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Uplift/Packaging/Export Package Utility", false, 250)]
        private static void ExportPackageWindow()
        {
            ExporterWindow window = EditorWindow.GetWindow(typeof(ExporterWindow), true) as ExporterWindow;
            window.Init();
            window.Show();
        }

        [MenuItem("Tools/Uplift/Debug/Nuke All Packages", false, 1000)]
        private static void NukePackages()
        {
            Debug.LogWarning("Nuking all packages!");
            UpliftManager.Instance().NukeAllPackages();
            AssetDatabase.Refresh();
        }

        [MenuItem("Uplift/About Uplift", false, 2000)]
        private static void AboutUplift()
        {
            EditorWindow.GetWindow(typeof(AboutWindow));
        }
    }
}
