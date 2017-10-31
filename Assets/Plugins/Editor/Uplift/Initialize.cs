using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Schemas;
using System;

namespace Uplift
{
    [InitializeOnLoad]
    public class Initialize : MonoBehaviour
    {
        private static readonly string env_variable = "UPLIFT_INSTALLATION_DONE";
        static Initialize()
        {
            Debug.Log("Upfile loading...");
            if (!Upfile.CheckForUpfile())
            {
                Debug.Log("No Upfile was found at the root of your project, Uplift created a sample one for you to start working on");
                SampleFile.CreateSampleUpfile();
            }
            
            if(!IsInitialized())
            {
                UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.INCOMPLETE_LOCKFILE, refresh: true);
                MarkAsInitialized();
            }
        }

        private static bool IsInitialized()
        {
            return string.Equals(Environment.GetEnvironmentVariable(env_variable), "true");
        }

        private static void MarkAsInitialized()
        {
            Environment.SetEnvironmentVariable(env_variable, "true");
        }
    }
}
