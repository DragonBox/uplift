using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Uplift.Common;
using Uplift.Windows;
using System.Net;
using System.Linq;
using System;
using System.Globalization;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Uplift.Updating
{
    public class Updater : MonoBehaviour
    {
        private static readonly string dateFormat = @"MM\/dd\/yyyy HH:mm";
        private static readonly CultureInfo provider = CultureInfo.InvariantCulture;
        private static readonly string lastUpdateCheckKey = "UpliftLastUpdateCheck";
        private static IEnumerator updateCoroutine;

        public static void UpdateUplift(string url)
        {
            string unitypackageName = url.Split('/').Last();
            DirectoryInfo assetsPath = new DirectoryInfo(Application.dataPath);
            string destination = Path.Combine(assetsPath.Parent.FullName, unitypackageName);
            
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;
            using (var client = new WebClient())
            {
                client.DownloadFile(url, destination);
            }

            AssetDatabase.ImportPackage(destination, false);
        }

        public static void CheckForUpdate(bool forceCheck = false)
        {
            if(forceCheck || ShouldCheck())
            {
                EditorApplication.update += EditorUpdate;
    			updateCoroutine = CheckForUpdateRoutine();
            }
        }
        
        private static void EditorUpdate()
        {
            updateCoroutine.MoveNext();
        }

        private static bool ShouldCheck()
        {
            string lastCheck = EditorPrefs.GetString(lastUpdateCheckKey);
            if(string.IsNullOrEmpty(lastCheck)) return true;
            
            try
            {
                TimeSpan timeFromLastCheck = DateTime.UtcNow - DateTime.ParseExact(lastCheck, dateFormat, provider);
                return timeFromLastCheck > TimeSpan.FromDays(1);
            }
            catch(FormatException e)
            {
                Debug.LogErrorFormat("The date stored for the last check of Uplift ({0}) is not in the correct format:\n{1}", lastCheck, e);
            }
            return true;
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
            EditorPrefs.SetString(
                lastUpdateCheckKey,
                DateTime.UtcNow.ToString(dateFormat, provider)
            );
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

        private static bool CertificateValidationCallback(
            System.Object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            bool correct = true;
            if(sslPolicyErrors == SslPolicyErrors.None) return true;

            for (int i=0; i < chain.ChainStatus.Length; i++) {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build ((X509Certificate2)certificate);
                if (!chainIsValid) {
                    correct = false;
                    break;
                }
            }

            return correct;
        }
    }
}