using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.Windows
{
    internal class UpdateUtility : EditorWindow
    {

       protected void OnGUI()
        {
            titleContent.text = "Update Utility";

            const string packageFormat = "{0} ({1})";


            Upbring upbring = Upbring.Instance();

            PackageList packageList = PackageList.Instance();


            foreach (InstalledPackage package in upbring.InstalledPackage)
            {
                PackageRepo latestPackageRepo = packageList.GetLatestPackage(package.Name);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format(packageFormat, package.Name, package.Version), "Latest: " + latestPackageRepo.Package.PackageVersion);

                if (package.Version != latestPackageRepo.Package.PackageVersion)
                {
                    if (GUILayout.Button("Update"))
                    {
                        Debug.Log(string.Format("Updating package {0} with version {1}", package.Name, latestPackageRepo.Package.PackageVersion));
                        LocalHandler.UpdatePackage(latestPackageRepo);

                        AssetDatabase.Refresh();

                        Repaint();
                    }
                }
                else
                {
                    if (GUILayout.Button("Reinstall"))
                    {
                        Debug.Log(string.Format("Reinstalling package {0} ({1})", package.Name, latestPackageRepo.Package.PackageVersion));
                        LocalHandler.UpdatePackage(latestPackageRepo);

                        AssetDatabase.Refresh();

                        Repaint();
                    }
                }


                EditorGUILayout.EndHorizontal();



            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Everything"))
            {
                foreach (InstalledPackage package in upbring.InstalledPackage)
                {
                    PackageRepo latestPackageRepo = packageList.GetLatestPackage(package.Name);
                    if (package.Version != latestPackageRepo.Package.PackageVersion)
                    {
                        Debug.Log(string.Format("Updating package {0} with version {1}", package.Name, latestPackageRepo.Package.PackageVersion));
                        LocalHandler.UpdatePackage(latestPackageRepo);

                        AssetDatabase.Refresh();

                        Repaint();
                    }
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh"))
            {
                PackageList.Instance().RefreshPackages();
                AssetDatabase.Refresh();
                Repaint();
            }


        }
    }
}