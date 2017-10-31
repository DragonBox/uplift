using UnityEngine;
using UnityEditor;
using System.IO;
using Uplift.Common;
using System.Net;
using System.Linq;

namespace Uplift.Updating
{
    public class Updater
    {
        public static void UpdateUplift(string url)
        {
            string unitypackageName = url.Split('/').Last();
            DirectoryInfo assetsPath = new DirectoryInfo(Application.dataPath);
            string destination = Path.Combine(assetsPath.Parent.FullName, unitypackageName);
            
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (var client = new WebClient())
            {
                client.DownloadFile(url, destination);
            }

            AssetDatabase.ImportPackage(destination, false);
        }
    }
}