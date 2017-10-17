using System;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Schemas;
using Uplift;
using PackageInfo = Uplift.Common.PackageInfo;

namespace Uplift.Windows
{
    public class ExporterWindow : EditorWindow
    {
        private struct PackageInfoHelper {
            public PackageInfo packageInfo;
            public bool selected;
        }

        // TODO: Make Window scrollable if too many lines
        // QUESTION: UI has not been thought in term of effiency, should we rework it?
        private PackageInfoHelper[] potentialPackages;
        private bool[] expanded;
        private const bool defaultExpanded = true;

        public void Init()
        {
            string[] directories = System.IO.Directory.GetDirectories(GetSelectedPathOrFallback());

            potentialPackages = new PackageInfoHelper[directories.Length];
            expanded = new bool[directories.Length];

            string path;
            for(int i = 0; i < directories.Length; i++)
            {
                // TODO: Instead of assuming from context we should try to fetch existing Upfile.xml and parse info from it
                path = directories[i];


                potentialPackages[i] = new PackageInfoHelper() {
                    selected = true
                };

                potentialPackages[i].packageInfo = new PackageInfo() {
                    paths = new string[]{path},
                    name = System.IO.Path.GetFileName(path),
                    version = "0.0.1",
                    license = "Undefined"

                };

                expanded[i] = defaultExpanded;
            }
        }

        public void OnGUI()
        {
            titleContent.text = "Export Utility";

            for(int i = 0; i < potentialPackages.Length; i++)
            {
                expanded[i] = EditorGUILayout.Foldout(expanded[i], potentialPackages[i].packageInfo.paths[0], true);
                if (expanded[i])
                {
                    PackageInfo pi = potentialPackages[i].packageInfo;

                    potentialPackages[i].selected = EditorGUILayout.Toggle("Export?", potentialPackages[i].selected);
                    GUI.enabled = potentialPackages[i].selected;
                    pi.name = EditorGUILayout.TextField("Package Name", pi.name);
                    pi.version = EditorGUILayout.TextField("Package Version", pi.version);
                    pi.license = EditorGUILayout.TextField("Package License", pi.license);
                    GUI.enabled = true;
                }

                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Export selected packages"))
            {
                foreach(PackageInfoHelper pInfoHelper in potentialPackages)
                {
                    if (!pInfoHelper.selected) continue;

                    Exporter exporter = new Exporter();
                    exporter.SetPackageInfo(pInfoHelper.packageInfo);
                    exporter.Export();
                }
            }
        }

        public void OnInspectorUpdate()
        {
            this.Repaint();
        }

        private string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

    }
}
