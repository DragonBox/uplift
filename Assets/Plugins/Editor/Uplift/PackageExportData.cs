using UnityEngine;
using UnityEditor;

namespace Uplift {
    class PackageExportData : ScriptableObject {
        public Object[] pathsToExport = null;
        public string   packageName = "";
        public string   packageVersion = "";
        public string   license = "";
        public string   targetDir = "target";

        protected string[] rawPaths;

        public string[] paths {
            get { return pathsToStringArray(); }
            set {
                rawPaths = value;
            }
        }

        protected string[] pathsToStringArray() {
            string[] result = new string[pathsToExport.Length + rawPaths.Length];

            for(int i=0; i<pathsToExport.Length;i++) {
                result[i] = AssetDatabase.GetAssetPath(pathsToExport[i]);
            }

            for(int i=0; i<rawPaths.Length;i++) {
                result[i] = rawPaths[i];
            }

            return result;
        }
    }
}
