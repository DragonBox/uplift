// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Uplift.Common;
using Uplift.GitHubModule;
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
        private static readonly string[] githubCertificatesString = new string[]
        {
// github.com
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
",
// DigiCert High Assurance EV Root CA
@"-----BEGIN CERTIFICATE-----
MIIDxTCCAq2gAwIBAgIQAqxcJmoLQJuPC3nyrkYldzANBgkqhkiG9w0BAQUFADBs
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSswKQYDVQQDEyJEaWdpQ2VydCBIaWdoIEFzc3VyYW5j
ZSBFViBSb290IENBMB4XDTA2MTExMDAwMDAwMFoXDTMxMTExMDAwMDAwMFowbDEL
MAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3
LmRpZ2ljZXJ0LmNvbTErMCkGA1UEAxMiRGlnaUNlcnQgSGlnaCBBc3N1cmFuY2Ug
RVYgUm9vdCBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMbM5XPm
+9S75S0tMqbf5YE/yc0lSbZxKsPVlDRnogocsF9ppkCxxLeyj9CYpKlBWTrT3JTW
PNt0OKRKzE0lgvdKpVMSOO7zSW1xkX5jtqumX8OkhPhPYlG++MXs2ziS4wblCJEM
xChBVfvLWokVfnHoNb9Ncgk9vjo4UFt3MRuNs8ckRZqnrG0AFFoEt7oT61EKmEFB
Ik5lYYeBQVCmeVyJ3hlKV9Uu5l0cUyx+mM0aBhakaHPQNAQTXKFx01p8VdteZOE3
hzBWBOURtCmAEvF5OYiiAhF8J2a3iLd48soKqDirCmTCv2ZdlYTBoSUeh10aUAsg
EsxBu24LUTi4S8sCAwEAAaNjMGEwDgYDVR0PAQH/BAQDAgGGMA8GA1UdEwEB/wQF
MAMBAf8wHQYDVR0OBBYEFLE+w2kD+L9HAdSYJhoIAu9jZCvDMB8GA1UdIwQYMBaA
FLE+w2kD+L9HAdSYJhoIAu9jZCvDMA0GCSqGSIb3DQEBBQUAA4IBAQAcGgaX3Nec
nzyIZgYIVyHbIUf4KmeqvxgydkAQV8GK83rZEWWONfqe/EW1ntlMMUu4kehDLI6z
eM7b41N5cdblIZQB2lWHmiRk9opmzN6cN82oNLFpmyPInngiK3BD41VHMWEZ71jF
hS9OMPagMRYjyOfiZRYzy78aG6A9+MpeizGLYAiJLQwGXFK3xPkKmNEVX58Svnw2
Yzi9RKR/5CYrCsSXaQ3pjOLAEFe4yHYSkVXySGnYvCoCWw9E1CAx2/S6cCZdkGCe
vEsXCS+0yx5DaMkHJ8HSXPfqIbloEpw8nL+e/IBcm2PN7EeqJSdnoDfzAIJ9VNep
+OkuE6N36B9K
-----END CERTIFICATE-----
",
// DigiCert SHA2 Extended Validation Server CA
@"-----BEGIN CERTIFICATE-----
MIIEtjCCA56gAwIBAgIQDHmpRLCMEZUgkmFf4msdgzANBgkqhkiG9w0BAQsFADBs
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSswKQYDVQQDEyJEaWdpQ2VydCBIaWdoIEFzc3VyYW5j
ZSBFViBSb290IENBMB4XDTEzMTAyMjEyMDAwMFoXDTI4MTAyMjEyMDAwMFowdTEL
MAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3
LmRpZ2ljZXJ0LmNvbTE0MDIGA1UEAxMrRGlnaUNlcnQgU0hBMiBFeHRlbmRlZCBW
YWxpZGF0aW9uIFNlcnZlciBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoC
ggEBANdTpARR+JmmFkhLZyeqk0nQOe0MsLAAh/FnKIaFjI5j2ryxQDji0/XspQUY
uD0+xZkXMuwYjPrxDKZkIYXLBxA0sFKIKx9om9KxjxKws9LniB8f7zh3VFNfgHk/
LhqqqB5LKw2rt2O5Nbd9FLxZS99RStKh4gzikIKHaq7q12TWmFXo/a8aUGxUvBHy
/Urynbt/DvTVvo4WiRJV2MBxNO723C3sxIclho3YIeSwTQyJ3DkmF93215SF2AQh
cJ1vb/9cuhnhRctWVyh+HA1BV6q3uCe7seT6Ku8hI3UarS2bhjWMnHe1c63YlC3k
8wyd7sFOYn4XwHGeLN7x+RAoGTMCAwEAAaOCAUkwggFFMBIGA1UdEwEB/wQIMAYB
Af8CAQAwDgYDVR0PAQH/BAQDAgGGMB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEF
BQcDAjA0BggrBgEFBQcBAQQoMCYwJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRp
Z2ljZXJ0LmNvbTBLBgNVHR8ERDBCMECgPqA8hjpodHRwOi8vY3JsNC5kaWdpY2Vy
dC5jb20vRGlnaUNlcnRIaWdoQXNzdXJhbmNlRVZSb290Q0EuY3JsMD0GA1UdIAQ2
MDQwMgYEVR0gADAqMCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5j
b20vQ1BTMB0GA1UdDgQWBBQ901Cl1qCt7vNKYApl0yHU+PjWDzAfBgNVHSMEGDAW
gBSxPsNpA/i/RwHUmCYaCALvY2QrwzANBgkqhkiG9w0BAQsFAAOCAQEAnbbQkIbh
hgLtxaDwNBx0wY12zIYKqPBKikLWP8ipTa18CK3mtlC4ohpNiAexKSHc59rGPCHg
4xFJcKx6HQGkyhE6V6t9VypAdP3THYUYUN9XR3WhfVUgLkc3UHKMf4Ib0mKPLQNa
2sPIoc4sUqIAY+tzunHISScjl2SFnjgOrWNoPLpSgVh5oywM395t6zHyuqB8bPEs
1OG9d4Q3A84ytciagRpKkk47RpqF/oOi+Z6Mo8wNXrM9zwR4jxQUezKcxwCmXMS1
oVWNWlZopCJwqjyBcdmdqEU79OX2olHdx3ti6G8MdOu42vi/hw15UJGQmxg7kVkn
8TUoE6smftX3eg==
-----END CERTIFICATE-----
",
// Amazon S3 (provider for the release download) *.s3.amazonaws.com
@"-----BEGIN CERTIFICATE-----
MIIFWjCCBEKgAwIBAgIQBVG1kvpTzyBSuLcPJ1zBWTANBgkqhkiG9w0BAQsFADBk
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSMwIQYDVQQDExpEaWdpQ2VydCBCYWx0aW1vcmUgQ0Et
MiBHMjAeFw0xNzA5MjIwMDAwMDBaFw0xOTAxMDMxMjAwMDBaMGsxCzAJBgNVBAYT
AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdTZWF0dGxlMRgwFgYD
VQQKEw9BbWF6b24uY29tIEluYy4xGzAZBgNVBAMMEiouczMuYW1hem9uYXdzLmNv
bTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKK5it42LH/8WFaEE7O9
4XJMg48TengCM25C9O0dUaPGOnZ1+oGf7DNvdo/MGVoAW5hfPX9WqjY30KyuKiFv
8uAbyX+9Bzq8KMLjdvGxzYMqbSxeAPSBf1yMdCyDEc8gW+vh2uXO+x9QePgiOAoO
qujLzyS/p7pSWPUKUH8kpMgP99RPoD/lRNbPAFr3QeJr7D/x3xNTKBHCbpE4SxBH
QM02GCu2DSYQtgm3hfZmD79BB1Ej4U1vM0hgiOu9eFLwmycbnrnU8sa4DkvmUJlv
xiIP5PvNweawm/QeoH6Qk/zDF30nr+5Av9IUhE4uAhl1H2OMqPp79SfJ2wxtvmNt
3z0CAwEAAaOCAf8wggH7MB8GA1UdIwQYMBaAFMASsih0aEZn6XAldBoARVsGfVxE
MB0GA1UdDgQWBBSQFZo7H1VA7uODvdTP1I6idMLqyDAvBgNVHREEKDAmghIqLnMz
LmFtYXpvbmF3cy5jb22CEHMzLmFtYXpvbmF3cy5jb20wDgYDVR0PAQH/BAQDAgWg
MB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjCBgQYDVR0fBHoweDA6oDig
NoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QmFsdGltb3JlQ0Et
MkcyLmNybDA6oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0
QmFsdGltb3JlQ0EtMkcyLmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwBATAqMCgG
CCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeBDAEC
AjB5BggrBgEFBQcBAQRtMGswJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2lj
ZXJ0LmNvbTBDBggrBgEFBQcwAoY3aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29t
L0RpZ2lDZXJ0QmFsdGltb3JlQ0EtMkcyLmNydDAMBgNVHRMBAf8EAjAAMA0GCSqG
SIb3DQEBCwUAA4IBAQBydwnbmrwTTwhtCckZ9lDGoE+tdDFchjFG59dyzX1I4bHV
thVUwjEoc3VxngvmKb3cqLcHzXHNdoEdffRtw0IiEerPRsk7Nxh2p1HWR3cu/Hyp
kqGlObJADiw1DX4UDob735hevqcGN9M0rn9P/AbPK0HCn5uokLDmbeqe6u1YgdJL
m4skA/WsUsMCfsZY27RiZ+B162+laDD0oRiovOx5zKrzG/3yAFVutz2Bfud3QenU
3zt14ttnzFCcMlglW9oqffHMwQ1Smx/30ccDonoa1E02hnNmxeLyBwomPqd/ZYy6
zSJW0aSi1DadkZsifpr65AwgSuN5uGEhQas0glsN
-----END CERTIFICATE-----
"
        };
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
            IEnumerator e = GitHub.LoadReleases("https://api.github.com/repos/DragonBox/uplift/releases");
            while (e.MoveNext()) yield return e.Current;

            GitHubRelease[] releases = (GitHubRelease[]) e.Current;

            if (releases == null)
            {
                Debug.LogError ("Unable to check for Uplift updates");
            }
            else if(!releases.Any(release => VersionParser.GreaterThan(release.tag, About.Version)))
            {
                Debug.Log("No udpate for Uplift available");
            }
            else
            {
                UpdatePopup popup = EditorWindow.GetWindow(typeof(UpdatePopup), true) as UpdatePopup;
                popup.SetReleases(releases.Where(release => VersionParser.GreaterThan(release.tag, About.Version)).ToArray());
            }
            EditorApplication.update -= EditorUpdate;
            EditorPrefs.SetString(
                lastUpdateCheckKey,
                DateTime.UtcNow.ToString(dateFormat, provider)
            );
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

            X509Certificate[] githubCertificates = new X509Certificate[githubCertificatesString.Length];
            for(int i = 0; i < githubCertificatesString.Length; i++)
            {
                githubCertificates[i] = new X509Certificate();
                githubCertificates[i].Import(Encoding.ASCII.GetBytes(githubCertificatesString[i]));
            }            
            
            if(!(githubCertificates.Any(cert => cert.GetCertHashString() == certificate.GetCertHashString()) || trustUnknown))
            {
                Debug.LogErrorFormat("The received certificate ({0}) is not known by Uplift. We cannot download the update package. You could update Uplift manually, or go to Preferences and set 'Trust unknown certificates' to true.", certificate.GetCertHashString());
                Debug.Log("Known certificates are:");
                foreach(X509Certificate cert in githubCertificates)
                    Debug.Log(" -- " + cert.GetCertHashString());
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