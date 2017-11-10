using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Uplift.Common;
using Uplift.Windows;
using System.Net;
using System.Linq;

namespace Uplift.Updating
{
    public class Updater : MonoBehaviour
    {
        private static IEnumerator updateCoroutine;

        public static void CheckForUpdate()
        {
            EditorApplication.update += EditorUpdate;
			updateCoroutine = CheckForUpdateRoutine();
        }
        
        private static void EditorUpdate()
        {
            updateCoroutine.MoveNext();
        }

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
        
        private static IEnumerator CheckForUpdateRoutine() {
			IEnumerator e = GetReleasesJson();
			while (e.MoveNext()) yield return e.Current;

			string json = (string) e.Current;

			if (json == null) {
				Debug.LogError ("Unable to check for Uplift updates");
			} else {
				System.Object obj = MiniJSON.Json.Deserialize (json);
				List<System.Object> releases = (List<System.Object>)obj;
				foreach (Dictionary<string, object> release in releases)
				{
                    if(VersionParser.GreaterThan((string)release ["tag_name"], About.Version))
                    {
                        Debug.Log("There is a new version of Uplift available!");
                        UpdatePopup popup = EditorWindow.GetWindow(typeof(UpdatePopup), true) as UpdatePopup;
                        popup.SetInformations(
                            (string)release["tag_name"],
                            ExtractUnityPackageUrl(release),
                            (string)release["body"]
                        );
                    }
                    else
                    {
                        Debug.Log("No update for Uplift available");
                    }
				}
			}
			EditorApplication.update -= EditorUpdate;
		}

		private static IEnumerator GetReleasesJson() {
			string url = "https://api.github.com/repos/DragonBox/uplift/releases";
			WWW www = new WWW (url);
			while (www.isDone == false)
				yield return null;
			yield return www;
			if(!string.IsNullOrEmpty(www.error)) {
				Debug.Log(www.error);
			}
			else {
				yield return www.text;
			}
		}

		private static string ExtractUnityPackageUrl(Dictionary<string, object> release)
		{
			if (release["assets"] == null) return string.Empty;
			List<System.Object> assets = (List<System.Object>)release["assets"];
			foreach (Dictionary<string, object> asset in assets) {
				string downloadUrl = (string)asset["browser_download_url"];
				if(downloadUrl.EndsWith(".unitypackage"))
					return downloadUrl;
			}

			return string.Empty;
		}
    }
}