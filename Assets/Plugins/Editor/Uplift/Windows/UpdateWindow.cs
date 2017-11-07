// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using System.Linq;
using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.DependencyResolution;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.Windows
{
    internal class UpdateUtility : EditorWindow
    {
        private Vector2 scrollPosition;
        private UpliftManager manager;
        private Upbring upbring;
        private Upfile upfile;
        private PackageList packageList;
        private PackageRepo[] packageRepos;

        protected void OnGUI()
        {
#if UNITY_5_1_OR_NEWER
            titleContent.text = "Update Utility";
#endif
            EditorGUILayout.HelpBox("Please note that this window is not currently supported, and still experimental. Using it may cause unexpected issues. Use with care.", MessageType.Warning);
            EditorGUILayout.Space();
            
            manager = UpliftManager.Instance();
            upbring = Upbring.Instance();
            upfile = Upfile.Instance();
            packageList = PackageList.Instance();
            packageRepos = packageList.GetAllPackages();

            IDependencySolver solver = manager.GetDependencySolver();
            DependencyDefinition[] dependencies = new DependencyDefinition[0];

            try
            {
                dependencies = solver.SolveDependencies(upfile.Dependencies);
                DependencyDefinition[] directDependencies = new DependencyDefinition[upfile.Dependencies.Length];
                for(int i = 0; i < upfile.Dependencies.Length; i++)
                {
                    directDependencies[i] = dependencies.First(dep => dep.Name == upfile.Dependencies[i].Name);
                }

                bool any_installed =
                        upbring.InstalledPackage != null &&
                        upbring.InstalledPackage.Length != 0;

                if (directDependencies.Length == 0)
                {
                    EditorGUILayout.HelpBox("It seems that you didn't specify any dependency in the Upfile. Try refreshing it if you did.", MessageType.Warning);
                }
                else
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                    DependencyDefinition[] packageDependencies;
                    for(int i = 0; i < directDependencies.Length; i++)
                    {
                        DependencyBlock(directDependencies[i], any_installed);

                        packageDependencies = packageList.ListDependenciesRecursively(directDependencies[i]);
                        if(packageDependencies.Length != 0)
                        {
                            EditorGUILayout.LabelField("Dependencies:");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical();
                            foreach (DependencyDefinition packageDependency in packageList.ListDependenciesRecursively(directDependencies[i]))
                                DependencyBlock(packageDependency, any_installed);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
                        }
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
                        foreach (InstalledPackage package in upbring.InstalledPackage)
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
                        UpliftManager.ResetInstances();

                        Repaint();
                    }
                }
            }
            catch(IncompatibleRequirementException e)
            {
                EditorGUILayout.HelpBox("There is a conflict in your dependency tree:\n" + e.ToString(), MessageType.Error);
            }
            catch(MissingDependencyException e)
            {
                EditorGUILayout.HelpBox("A dependency cannot be found in any of your specified repository:\n" + e.ToString(), MessageType.Error);
            }
        }

        private void DependencyBlock(DependencyDefinition definition, bool any_installed)
        {
            string name = definition.Name;
            EditorGUILayout.LabelField(name + ":", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Requirement: " + definition.Requirement.ToString());
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
                if (!definition.Requirement.IsMetBy(latest_version))
                {
                    EditorGUILayout.HelpBox("The latest available version does not meet the requirement of the dependency.", MessageType.Warning);
                }

                GUI.enabled = definition.Requirement.IsMetBy(latest_version) && installed && installed_version != latest_version;
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
    }
}
