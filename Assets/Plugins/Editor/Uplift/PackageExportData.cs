using UnityEngine;
using UnityEditor;

namespace Uplift {
    class PackageExportData : ScriptableObject {
        public Object[] pathsToExport = null;
        public string   packageName = "";
        public string   packageVersion = "";
        public string   license = "";

        public string[] pathsToStringArray() {
            string[] result = new string[(pathsToExport.Length)];

            for(int i=0; i<pathsToExport.Length;i++) {
                result[i] = AssetDatabase.GetAssetPath(pathsToExport[i]);
            }

            return result;
        }
    }
}
