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
        private bool showPathspathsToExport = true;

        public void OnEnable()
        {
            packageExportData = (PackageExportData)target;
            if(packageExportData.pathsToExport == null)
                packageExportData.pathsToExport = new string[0];
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
            packageExportData.templateUpsetFile = AssetDatabase.GetAssetPath(
                EditorGUILayout.ObjectField(
                    "Template Upset file",
                    AssetDatabase.LoadMainAssetAtPath(packageExportData.templateUpsetFile),
                    typeof(UnityEngine.Object),
                    false
                )
            );

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);
            packageExportData.targetDir = EditorGUILayout.TextField("Build destination directory", packageExportData.targetDir);
            showPathspathsToExport = EditorGUILayout.Foldout(showPathspathsToExport, "Paths to export");
            if(showPathspathsToExport)
            {
                EditorGUI.indentLevel += 1;
                for(int i = 0; i < packageExportData.pathsToExport.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    packageExportData.pathsToExport[i] = AssetDatabase.GetAssetPath(
                        EditorGUILayout.ObjectField(
                            "Item to export",
                            AssetDatabase.LoadMainAssetAtPath(packageExportData.pathsToExport[i]),
                            typeof(UnityEngine.Object),
                            false
                        )
                    );
                    if(GUILayout.Button("X", GUILayout.Width(20.0f)))
                    {
                        var tempPathspathsToExport = new List<string>(packageExportData.pathsToExport);
                        tempPathspathsToExport.RemoveAt(i);
                        packageExportData.pathsToExport = tempPathspathsToExport.ToArray();
                        EditorUtility.SetDirty(packageExportData);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel -= 1;
                if(GUILayout.Button("+"))
                {
                    Array.Resize<string>(ref packageExportData.pathsToExport, packageExportData.pathsToExport.Length + 1);
                    EditorUtility.SetDirty(packageExportData);
                }
            }

            EditorUtility.SetDirty(packageExportData);
        }
    }
}