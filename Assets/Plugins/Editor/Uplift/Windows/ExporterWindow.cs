using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Uplift.Schemas;

namespace Uplift.Windows
{
    public class ExporterWindow : EditorWindow
    {
        // TODO: Make Window scrollable if too many lines
        // QUESTION: UI has not been thought in term of effiency, should we rework it?
        private PackageInfo[] packageInfos;
        private bool[] expanded;
        private string packageFormat = "{0}~{1}";
        private const bool defaultExpanded = true;

        public void Init()
        {
            string[] directories = System.IO.Directory.GetDirectories(GetSelectedPathOrFallback());
            packageInfos = new PackageInfo[directories.Length];
            expanded = new bool[directories.Length];

            string path;
            for(int i = 0; i < directories.Length; i++)
            {
                // TODO: Instead of assuming from context we should try to fetch existing Upfile.xml and parse info from it
                path = directories[i];
                packageInfos[i].selected = true;
                packageInfos[i].path = path;
                packageInfos[i].name = System.IO.Path.GetFileName(path);
                packageInfos[i].version = "0.0.1";
                packageInfos[i].license = "Undefined";

                expanded[i] = defaultExpanded;
            }
        }

        public void OnGUI()
        {
            titleContent.text = "Export Utility";            

            for(int i = 0; i < packageInfos.Length; i++)
            {
                expanded[i] = EditorGUILayout.Foldout(expanded[i], packageInfos[i].path, true);
                if (expanded[i])
                {
                    packageInfos[i].selected = EditorGUILayout.Toggle("Export?", packageInfos[i].selected);
                    GUI.enabled = packageInfos[i].selected;
                    packageInfos[i].name = EditorGUILayout.TextField("Package Name", packageInfos[i].name);
                    packageInfos[i].version = EditorGUILayout.TextField("Package Version", packageInfos[i].version);
                    packageInfos[i].license = EditorGUILayout.TextField("Package License", packageInfos[i].license);
                    GUI.enabled = true;
                }

                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Export selected packages"))
            {
                foreach(PackageInfo pInfo in packageInfos)
                {
                    if (!pInfo.selected) continue;
                    string[] files = System.IO.Directory.GetFiles(pInfo.path, "*.*", System.IO.SearchOption.AllDirectories);
                    string name = string.Format(packageFormat, pInfo.name, pInfo.version);

                    // Create Upset file for the package
                    Upset file = new Upset()
                    {
                        UnityVersion = Application.unityVersion,
                        PackageName = pInfo.name,
                        PackageLicense = pInfo.license,
                        PackageVersion = pInfo.version
                    };

                    XmlSerializer serializer = new XmlSerializer(typeof(Upset));
                    using (FileStream fs = new FileStream(name + ".Upset.xml", FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                        {
                            serializer.Serialize(sw, file);
                        }
                    }

                    // Export .unitypackage
                    AssetDatabase.ExportPackage(files, name + ".unitypackage", ExportPackageOptions.Default);
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

        private struct PackageInfo {
            public bool selected;
            public string path;
            public string name;
            public string version;
            public string license;
        }
    }
}
