using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;



namespace Uplift {
    [CreateAssetMenuAttribute(fileName = "PackageExport.asset", menuName = "Uplift/Package Export Definition", order = 250)]
    class PackageExportData : ScriptableObject, ICloneable {

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
            get { return PathsToStringArray(); }
            set {
                rawPaths = value;
            }
        }

        protected string[] PathsToStringArray() {
            string[] result = new string[pathsToExport.Length + rawPaths.Length];

            for(int i=0; i<pathsToExport.Length;i++) {
                result[i] = AssetDatabase.GetAssetPath(pathsToExport[i]);
            }

            for(int i=0; i<rawPaths.Length;i++) {
                result[i] = rawPaths[i];
            }

            return result;
        }

        public object Clone() {
            return UnityEngine.Object.Instantiate(this) as PackageExportData;
        }

        public void SetOrCheckOverridenDefaults(PackageExportData defaults) {
            string[] overridableDefaults = {"packageName", "packageVersion", "license", "targetDir"};

            foreach(var fieldName in overridableDefaults) {
                System.Reflection.FieldInfo field = this.GetType().GetField(fieldName);

                // I know those are strings, so...
                var defaultValue = field.GetValue(defaults) as string;
                var currentValue = field.GetValue(this) as string;

                if(!string.IsNullOrEmpty(currentValue) && currentValue != defaultValue) {
                    Debug.Log(string.Format("NOTE: {0} overriden by Package Export Specification ({1} -> {2})",
                                            fieldName, defaultValue, currentValue ));

                } else {
                    field.SetValue(this, defaultValue);
                }
            }

        }

    }
}
