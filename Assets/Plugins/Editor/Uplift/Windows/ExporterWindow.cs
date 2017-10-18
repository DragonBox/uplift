using System.IO;
using UnityEditor;
using UnityEngine;

namespace Uplift.Windows
{
    public class ExporterWindow : EditorWindow
    {
        private struct PackageInfoHelper {
            public PackageExportData exportSpec;
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

                potentialPackages[i].exportSpec = new PackageExportData() {
                    paths = new string[]{path},
                    packageName = System.IO.Path.GetFileName(path),
                    packageVersion = "0.0.1",
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
                expanded[i] = EditorGUILayout.Foldout(expanded[i], potentialPackages[i].exportSpec.paths[0], true);
                if (expanded[i])
                {
                    PackageExportData ed = potentialPackages[i].exportSpec;

                    potentialPackages[i].selected = EditorGUILayout.Toggle("Export?", potentialPackages[i].selected);
                    GUI.enabled = potentialPackages[i].selected;
                    ed.packageName = EditorGUILayout.TextField("Package Name", ed.packageName);
                    ed.packageVersion = EditorGUILayout.TextField("Package Version", ed.packageVersion);
                    ed.license = EditorGUILayout.TextField("Package License", ed.license);
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
                    exporter.SetExportSpec(pInfoHelper.exportSpec);
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
