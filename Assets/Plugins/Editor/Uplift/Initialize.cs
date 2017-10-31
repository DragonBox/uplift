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

using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Schemas;
using Uplift.Windows;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

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
                UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.ONLY_LOCKFILE, refresh: true);
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

	[InitializeOnLoad]
	[ExecuteInEditMode]
	public class CheckForUpdate : MonoBehaviour
	{
		static IEnumerator myCoroutine;

		static CheckForUpdate()
		{
			EditorApplication.update += EditorUpdate;
			myCoroutine = CheckForUpdateRoutine();
		}

		static void EditorUpdate()
		{
			myCoroutine.MoveNext ();
		}

		static IEnumerator CheckForUpdateRoutine() {
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

		static IEnumerator GetReleasesJson() {
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

		static string ExtractUnityPackageUrl(Dictionary<string, object> release)
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
