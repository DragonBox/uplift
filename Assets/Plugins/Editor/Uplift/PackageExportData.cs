using UnityEngine;
using UnityEditor;

namespace Uplift {
    [CreateAssetMenuAttribute(fileName = "PackageExport.asset", menuName = "Uplift/Package Export Definition", order = 250)]
    class PackageExportData : ScriptableObject {

        [Header("Basic Package Information")]
        public  string    packageName     =  "";
        public  string    packageVersion  =  "";
        public  string    license         =  "";

        [Header("Paths")]
        public  Object[]  pathsToExport   =  new Object[0];

        [Header("Export Settings")]
        public  string    targetDir       =  "target";

        protected string[] rawPaths       = new string[0];

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
