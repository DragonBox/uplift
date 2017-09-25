using System.Linq;
using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.Windows
{
    internal class UpdateUtility : EditorWindow
    {
        private Vector2 scrollPosition;
        protected void OnGUI()
        {
            titleContent.text = "Update Utility";

            UpliftManager manager = UpliftManager.Instance();
            Upbring upbring = Upbring.Instance();
            Upfile upfile = Upfile.Instance();
            PackageList packageList = PackageList.Instance();
            PackageRepo[] packageRepos = packageList.GetAllPackages();

            DependencyDefinition[] dependencies = upfile.Dependencies;
            bool any_installed =
                        upbring.InstalledPackage != null &&
                        upbring.InstalledPackage.Length != 0;

            if (dependencies.Length == 0)
            {
                EditorGUILayout.HelpBox("It seems that you didn't specify any dependency in the Upfile. Try refreshing it if you did.", MessageType.Warning);
            }
            else
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                foreach (DependencyDefinition dependency in dependencies)
                {
                    string name = dependency.Name;
                    EditorGUILayout.LabelField(name + ":", EditorStyles.boldLabel);
                    bool installable = packageRepos.Any(pr => pr.Package.PackageName == name);
                    bool installed =
                        any_installed &&
                        upbring.InstalledPackage.Any(ip => ip.Name == name);
                    string installed_version = installed ? upbring.GetInstalledPackage(name).Version : "";

                    if (installed)
                    {
                        EditorGUILayout.LabelField("- Installed version is " + installed_version);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("- Not yet installed");
                    }

                    if (!installable)
                    {
                        EditorGUILayout.HelpBox("No repository contains this package. Try specifying one whith this package in.", MessageType.Warning);
                    }
                    else
                    {
                        PackageRepo latestPackageRepo = packageList.GetLatestPackage(name);
                        string latest_version = latestPackageRepo.Package.PackageVersion;

                        EditorGUILayout.LabelField(string.Format("- Latest version is: {0} (from {1})", latest_version, latestPackageRepo.Repository.ToString()));
                        
                        GUI.enabled = installed && installed_version != latest_version;
                        if (GUILayout.Button("Update to " + latest_version))
                        {
                            Debug.Log(string.Format("Updating package {0} (to {1})", name, latest_version));
                            manager.UpdatePackage(latestPackageRepo);

                            AssetDatabase.Refresh();

                            Repaint();
                        }
                        GUI.enabled = true;
                    }

                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Install all dependencies"))
                {
                    manager.InstallDependencies();

                    AssetDatabase.Refresh();

                    Repaint();
                }
                GUI.enabled = any_installed;
                if (GUILayout.Button("Update all installed packages"))
                {
                    foreach(InstalledPackage package in upbring.InstalledPackage)
                    {
                        PackageRepo latestPackageRepo = packageList.GetLatestPackage(package.Name);
                        if (package.Version != latestPackageRepo.Package.PackageVersion)
                        {
                            Debug.Log(string.Format("Updating package {0} (to {1})", package.Name, latestPackageRepo.Package.PackageVersion));
                            manager.UpdatePackage(latestPackageRepo);
                        }
                    }

                    AssetDatabase.Refresh();

                    Repaint();
                }
                GUI.enabled = true;
                if (GUILayout.Button("Refresh Upfile"))
                {
                    Upfile.Instance();

                    Repaint();
                }
            }
        }
    }
}