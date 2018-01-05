using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Uplift.Export
{
    [CustomEditor(typeof(PackageExportData))]
    public class PackageExportDataInspector : Editor
    {
        private PackageExportData packageExportData;
        private bool showPaths = true;

        public void OnEnable()
        {
            packageExportData = (PackageExportData)target;
            if(packageExportData.paths == null)
                packageExportData.paths = new string[0];
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Basic Package Information", EditorStyles.boldLabel);
            packageExportData.packageName  = EditorGUILayout.TextField("Package name", packageExportData.packageName);
            packageExportData.packageVersion  = EditorGUILayout.TextField("Package version", packageExportData.packageVersion);
            packageExportData.license  = EditorGUILayout.TextField("License", packageExportData.license);
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Additional Package Information", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The template upset file is used to get the dependencies and the configuration of the package", MessageType.Info);
            packageExportData.templateUpsetPath = AssetDatabase.GetAssetPath(
                EditorGUILayout.ObjectField(
                    "Template Upset file",
                    AssetDatabase.LoadMainAssetAtPath(packageExportData.templateUpsetPath),
                    typeof(UnityEngine.Object),
                    false
                )
            );

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);
            packageExportData.targetDir = EditorGUILayout.TextField("Build destination directory", packageExportData.targetDir);
            showPaths = EditorGUILayout.Foldout(showPaths, "Paths to export");
            if(showPaths)
            {
                EditorGUI.indentLevel += 1;
                for(int i = 0; i < packageExportData.paths.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    packageExportData.paths[i] = AssetDatabase.GetAssetPath(
                        EditorGUILayout.ObjectField(
                            "Item to export",
                            AssetDatabase.LoadMainAssetAtPath(packageExportData.paths[i]),
                            typeof(UnityEngine.Object),
                            false
                        )
                    );
                    if(GUILayout.Button("X", GUILayout.Width(20.0f)))
                    {
                        var tempPaths = new List<string>(packageExportData.paths);
                        tempPaths.RemoveAt(i);
                        packageExportData.paths = tempPaths.ToArray();
                        EditorUtility.SetDirty(packageExportData);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel -= 1;
                if(GUILayout.Button("+"))
                {
                    Array.Resize<string>(ref packageExportData.paths, packageExportData.paths.Length + 1);
                    EditorUtility.SetDirty(packageExportData);
                }
            }

            EditorUtility.SetDirty(packageExportData);
        }
    }
}