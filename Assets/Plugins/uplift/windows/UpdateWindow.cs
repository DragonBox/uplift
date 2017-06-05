using UnityEditor;
using UnityEngine;
using System;

namespace Uplift
{

    using Schemas;
    
    class UpdateUtility : EditorWindow
    {

        void OnGUI()
        {
            titleContent.text = "Update Utility";

            string packageFormat = "{0} ({1})";


            Upbring upbring = Upbring.FromXml();

            PackageList packageList = PackageList.Instance();


            foreach (var package in upbring.InstalledPackage)
            {
                PackageRepo latestPackageRepo = packageList.GetLatestPackage(package.Name);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(String.Format(packageFormat, package.Name, package.Version), "Latest: " + latestPackageRepo.Package.PackageVersion);

                if (package.Version != latestPackageRepo.Package.PackageVersion)
                {
                    if (GUILayout.Button("Update"))
                    {
                        Debug.Log(String.Format("Updating package {0} with version {1}", package.Name, latestPackageRepo.Package.PackageVersion));
                        LocalHandler.UpdatePackage(latestPackageRepo);

                        UnityEditor.AssetDatabase.Refresh();

                        this.Repaint();
                    }
                }
                else
                {
                    if (GUILayout.Button("Reinstall"))
                    {
                        Debug.Log(String.Format("Reinstalling package {0} ({1})", package.Name, latestPackageRepo.Package.PackageVersion));
                        LocalHandler.UpdatePackage(latestPackageRepo);

                        UnityEditor.AssetDatabase.Refresh();

                        this.Repaint();
                    }
                }


                EditorGUILayout.EndHorizontal();



            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Everything"))
            {
                foreach (var package in upbring.InstalledPackage)
                {
                    PackageRepo latestPackageRepo = packageList.GetLatestPackage(package.Name);
                    if (package.Version != latestPackageRepo.Package.PackageVersion)
                    {
                        Debug.Log(String.Format("Updating package {0} with version {1}", package.Name, latestPackageRepo.Package.PackageVersion));
                        LocalHandler.UpdatePackage(latestPackageRepo);

                        UnityEditor.AssetDatabase.Refresh();

                        this.Repaint();
                    }
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh"))
            {
                PackageList.Instance().RefreshPackages();
                UnityEditor.AssetDatabase.Refresh();
                this.Repaint();
            }


        }
    }
}