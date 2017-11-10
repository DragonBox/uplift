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
        private static readonly string githubCertificateString =
@"-----BEGIN CERTIFICATE-----
MIIHeTCCBmGgAwIBAgIQC/20CQrXteZAwwsWyVKaJzANBgkqhkiG9w0BAQsFADB1
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMTQwMgYDVQQDEytEaWdpQ2VydCBTSEEyIEV4dGVuZGVk
IFZhbGlkYXRpb24gU2VydmVyIENBMB4XDTE2MDMxMDAwMDAwMFoXDTE4MDUxNzEy
MDAwMFowgf0xHTAbBgNVBA8MFFByaXZhdGUgT3JnYW5pemF0aW9uMRMwEQYLKwYB
BAGCNzwCAQMTAlVTMRkwFwYLKwYBBAGCNzwCAQITCERlbGF3YXJlMRAwDgYDVQQF
Ewc1MTU3NTUwMSQwIgYDVQQJExs4OCBDb2xpbiBQIEtlbGx5LCBKciBTdHJlZXQx
DjAMBgNVBBETBTk0MTA3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKQ2FsaWZvcm5p
YTEWMBQGA1UEBxMNU2FuIEZyYW5jaXNjbzEVMBMGA1UEChMMR2l0SHViLCBJbmMu
MRMwEQYDVQQDEwpnaXRodWIuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIB
CgKCAQEA54hc8pZclxgcupjiA/F/OZGRwm/ZlucoQGTNTKmBEgNsrn/mxhngWmPw
bAvUaLP//T79Jc+1WXMpxMiz9PK6yZRRFuIo0d2bx423NA6hOL2RTtbnfs+y0PFS
/YTpQSelTuq+Fuwts5v6aAweNyMcYD0HBybkkdosFoDccBNzJ92Ac8I5EVDUc3Or
/4jSyZwzxu9kdmBlBzeHMvsqdH8SX9mNahXtXxRpwZnBiUjw36PgN+s9GLWGrafd
02T0ux9Yzd5ezkMxukqEAQ7AKIIijvaWPAJbK/52XLhIy2vpGNylyni/DQD18bBP
T+ZG1uv0QQP9LuY/joO+FKDOTler4wIDAQABo4IDejCCA3YwHwYDVR0jBBgwFoAU
PdNQpdagre7zSmAKZdMh1Pj41g8wHQYDVR0OBBYEFIhcSGcZzKB2WS0RecO+oqyH
IidbMCUGA1UdEQQeMByCCmdpdGh1Yi5jb22CDnd3dy5naXRodWIuY29tMA4GA1Ud
DwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwdQYDVR0f
BG4wbDA0oDKgMIYuaHR0cDovL2NybDMuZGlnaWNlcnQuY29tL3NoYTItZXYtc2Vy
dmVyLWcxLmNybDA0oDKgMIYuaHR0cDovL2NybDQuZGlnaWNlcnQuY29tL3NoYTIt
ZXYtc2VydmVyLWcxLmNybDBLBgNVHSAERDBCMDcGCWCGSAGG/WwCATAqMCgGCCsG
AQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAcGBWeBDAEBMIGI
BggrBgEFBQcBAQR8MHowJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0
LmNvbTBSBggrBgEFBQcwAoZGaHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0Rp
Z2lDZXJ0U0hBMkV4dGVuZGVkVmFsaWRhdGlvblNlcnZlckNBLmNydDAMBgNVHRMB
Af8EAjAAMIIBfwYKKwYBBAHWeQIEAgSCAW8EggFrAWkAdgCkuQmQtBhYFIe7E6LM
Z3AKPDWYBPkb37jjd80OyA3cEAAAAVNhieoeAAAEAwBHMEUCIQCHHSEY/ROK2/sO
ljbKaNEcKWz6BxHJNPOtjSyuVnSn4QIgJ6RqvYbSX1vKLeX7vpnOfCAfS2Y8lB5R
NMwk6us2QiAAdgBo9pj4H2SCvjqM7rkoHUz8cVFdZ5PURNEKZ6y7T0/7xAAAAVNh
iennAAAEAwBHMEUCIQDZpd5S+3to8k7lcDeWBhiJASiYTk2rNAT26lVaM3xhWwIg
NUqrkIODZpRg+khhp8ag65B8mu0p4JUAmkRDbiYnRvYAdwBWFAaaL9fC7NP14b1E
sj7HRna5vJkRXMDvlJhV1onQ3QAAAVNhieqZAAAEAwBIMEYCIQDnm3WStlvE99GC
izSx+UGtGmQk2WTokoPgo1hfiv8zIAIhAPrYeXrBgseA9jUWWoB4IvmcZtshjXso
nT8MIG1u1zF8MA0GCSqGSIb3DQEBCwUAA4IBAQCLbNtkxuspqycq8h1EpbmAX0wM
5DoW7hM/FVdz4LJ3Kmftyk1yd8j/PSxRrAQN2Mr/frKeK8NE1cMji32mJbBqpWtK
/+wC+avPplBUbNpzP53cuTMF/QssxItPGNP5/OT9Aj1BxA/NofWZKh4ufV7cz3pY
RDS4BF+EEFQ4l5GY+yp4WJA/xSvYsTHWeWxRD1/nl62/Rd9FN2NkacRVozCxRVle
FrBHTFxqIP6kDnxiLElBrZngtY07ietaYZVLQN/ETyqLQftsf8TecwTklbjvm8NT
JqbaIVifYwqwNN+4lRxS3F5lNlA/il12IOgbRioLI62o8G0DaEUQgHNf8vSG
-----END CERTIFICATE-----
";
        private static bool trustUnknown;

        public static void UpdateUplift(string url)
        {
            trustUnknown = UpliftPreferences.TrustUnknownCertificates();
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

            X509Certificate githubCertificate = new X509Certificate();
            githubCertificate.Import(Encoding.ASCII.GetBytes(githubCertificateString));
            
            if(certificate.GetCertHashString() != githubCertificate.GetCertHashString() && !trustUnknown)
            {
                Debug.LogErrorFormat("The received certificate ({0}) is not known by Uplift. We cannot download the update package. You could update Uplift manually, or go to Preferences and set 'Trust unknown certificates' to true.", certificate.GetCertHashString());
                return false;
            }

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