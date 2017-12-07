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

using System;
using Uplift.Schemas;
using UnityEngine;
using UnityEditor;

namespace Uplift.Windows
{
    public class UpfileEditor : EditorWindow
    {
        private Upfile upfile;
        private Vector2 scrollPosition;

        protected void OnGUI()
        {
#if UNITY_5_1_OR_NEWER
            titleContent.text = "Edit Upfile";
#endif
            upfile = Upfile.Instance();

            EditorGUILayout.Space();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            upfile.UnityVersion = EditorGUILayout.TextField("Unity version:", upfile.UnityVersion);
            EditorGUILayout.Separator();
            if(upfile.Repositories != null)
            {
                EditorGUILayout.LabelField("Repositories:", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This sections specifies where Uplift will fetch the packages from.", MessageType.Info);
                // FIXME: Do not care about Upfile Override repositories
                upfile.Repositories = ArrayField<Repository>(
                    upfile.Repositories,
                    "Remove repository",
                    "Add another repository",
                    new FileRepository { Path = "Enter a path "},
                    repo => RepositoryField(repo)
                );
            }
            else
            {
                upfile.Repositories = new Repository[0];
                Repaint();
            }

            EditorGUILayout.Separator();
            if(upfile.Configuration != null)
            {
                EditorGUILayout.LabelField("Configuration:", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This sections specifies where Uplift will install the packages into your project.", MessageType.Info);
                upfile.Configuration.RepositoryPath = PathField("Repository path:", upfile.Configuration.RepositoryPath);
                upfile.Configuration.DocsPath = PathField("Documentation path:", upfile.Configuration.DocsPath);
                upfile.Configuration.ExamplesPath = PathField("Examples path:", upfile.Configuration.ExamplesPath);
                upfile.Configuration.BaseInstallPath = PathField("Base install path:", upfile.Configuration.BaseInstallPath);
                upfile.Configuration.MediaPath = PathField("Media path:", upfile.Configuration.MediaPath);
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Warning: the following path have special behaviours in Unity. Modify them at your own risk!");
                upfile.Configuration.GizmoPath = PathField("Gizmo path:", upfile.Configuration.GizmoPath);
                upfile.Configuration.PluginPath = PathField("Plugin path:", upfile.Configuration.PluginPath);
                upfile.Configuration.EditorPluginPath = PathField("Plugin path for the Editor:", upfile.Configuration.EditorPluginPath);
            }

            EditorGUILayout.Separator();
            if(upfile.Dependencies != null)
            {
                EditorGUILayout.LabelField("Dependencies:", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This sections specifies which packages your project depends on.", MessageType.Info);
                upfile.Dependencies = ArrayField<DependencyDefinition>(
                    upfile.Dependencies,
                    "Remove dependency",
                    "Declare another dependency",
                    new DependencyDefinition
                    {
                        Name = "Enter a package name",
                        Version = "Enter a version"
                    },
                    def => DependencyField(def)
                );
            }
            else
            {
                upfile.Dependencies = new DependencyDefinition[0];
                Repaint();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            
            if(GUILayout.Button("Refresh Upfile"))
            {
                Upfile.InitializeInstance();
            }

            if(GUILayout.Button("Edit raw Upfile"))
            {
                EditorWindow.GetWindow(typeof(RawUpfileEditor));
            }

            if(GUILayout.Button("Save Upfile"))
            {
                upfile.SaveFile();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private Repository RepositoryField(Repository repository) 
        {
            if(repository is FileRepository)
            {
                FileRepository temp = (FileRepository) repository;
                temp.Path = EditorGUILayout.TextField("Path to file repository:", temp.Path);
                return temp;
            }

            if(repository is GithubRepository)
            {
                GithubRepository temp = (GithubRepository) repository;
                temp.Url = EditorGUILayout.TextField("Url of github repository", temp.Url);

                if(temp.TagList != null)
                {
                    temp.TagList = ArrayField<string>(
                        temp.TagList,
                        "Fetch from tag",
                        "Add another tag",
                        "sometag",
                        tag => EditorGUILayout.TextField(tag)
                    );
                }
                return temp;
            }

            EditorGUILayout.HelpBox(string.Format("The repository {0} is not a known type of repository!"), MessageType.Error);
            return repository;
        }

        private PathConfiguration PathField(string label, PathConfiguration path)
        {
            PathConfiguration temp = path;
            EditorGUILayout.BeginHorizontal();
            temp.Location = EditorGUILayout.TextField(label, temp.Location);
            temp.SkipPackageStructure = EditorGUILayout.Toggle("Skip Package structure?", temp.SkipPackageStructure, GUILayout.Width(180f));
            temp.SkipPackageStructureSpecified = temp.SkipPackageStructure == true;
            EditorGUILayout.EndHorizontal();
            return temp;
        }

        private SkipInstallSpec SkipInstallField(SkipInstallSpec spec)
        {
            SkipInstallSpec temp = spec;
            temp.Type = (InstallSpecType)EditorGUILayout.EnumPopup("Type to skip", temp.Type);
            return temp;
        }

        private OverrideDestinationSpec OverrideDestinationField(OverrideDestinationSpec spec)
        {
            OverrideDestinationSpec temp = spec;
            temp.Type = (InstallSpecType)EditorGUILayout.EnumPopup("Type to skip", temp.Type);
            temp.Location = EditorGUILayout.TextField("Overriden location", temp.Location);
            return temp;
        }

        private DependencyDefinition DependencyField(DependencyDefinition def)
        {
            DependencyDefinition temp = def;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            temp.Name = EditorGUILayout.TextField("Package Name:", temp.Name);
            temp.Version = EditorGUILayout.TextField("Package Version:", temp.Version);
            EditorGUILayout.EndHorizontal();
            OverrideDestinationSpec defaultODS = new OverrideDestinationSpec { Type = InstallSpecType.Base, Location = "Enter a location" };
            SkipInstallSpec defaultSIS = new SkipInstallSpec { Type = InstallSpecType.Docs };
            if(temp.OverrideDestination != null)
            {
                temp.OverrideDestination = ArrayField<OverrideDestinationSpec>(
                    temp.OverrideDestination,
                    "Do not override this",
                    "Override another type",
                    defaultODS,
                    ods => OverrideDestinationField(ods)
                );
            }
            else
            {
                if(GUILayout.Button("Declare some installation overrides"))
                    {
                        temp.OverrideDestination = new OverrideDestinationSpec[] { defaultODS };
                    }
            }
            if(temp.SkipInstall != null)
            {
                temp.SkipInstall = ArrayField<SkipInstallSpec>(
                    temp.SkipInstall,
                    "Do not skip this",
                    "Skip another type",
                    defaultSIS,
                    sis => SkipInstallField(sis)
                );
            }
            else
            {
                if(GUILayout.Button("Declare some installation skips"))
                {
                    temp.SkipInstall = new SkipInstallSpec[] { defaultSIS };
                }
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            return temp;
        }

        private T[] ArrayField<T>(T[] source, string removeMessage, string addMessage, T defaultElement, Func<T, T> elementField)
        {
            T[] array = source;
            for(int i = 0; i < array.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                array[i] = elementField.Invoke(array[i]);
                if(GUILayout.Button(removeMessage, GUILayout.Width(180f)))
                {
                    if(array.Length <= 1)
                    {
                        array = null;
                        break;
                    }
                    else
                    {
                        T[] temp = array;
                        array = new T[temp.Length - 1];
                        Array.Copy(temp, array, i);
                        Array.Copy(temp, i + 1, array, i, temp.Length - i - 1);
                        Repaint();
                    }                    
                }
                EditorGUILayout.EndHorizontal();
            }
                
            if(GUILayout.Button(addMessage))
            {
                T[] temp = array;
                array = new T[temp.Length + 1];
                Array.Copy(temp, array, temp.Length);
                array[temp.Length] = defaultElement;
                Repaint();
            }
            return array;
        }
    }
}
