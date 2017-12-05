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
using Uplift.Common;
using Uplift.GitHubModule;
using Uplift.Windows;
using System.Net;
using System.Linq;
using System;
using System.Globalization;

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
            
            ServicePointManager.ServerCertificateValidationCallback = GitHub.CertificateValidationCallback;
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
    }
}
