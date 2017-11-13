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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Uplift.Common;
using Uplift.MiniJSON;

namespace Uplift.GitHubModule
{
    public class GitHub
    {
        public static IEnumerator LoadReleases(string url)
        {
            WWW www = new WWW (url);
            while (www.isDone == false)
                yield return null;
            yield return www;

            if(!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
            }
            else
            {
                yield return ParseReleases(www.text);
            }
        }

        private static GitHubRelease[] ParseReleases(string jsonString)
        {
            if (jsonString == null) return null;
            
            List<GitHubRelease> releaseList = new List<GitHubRelease>();

            System.Object obj = MiniJSON.Json.Deserialize (jsonString);
            List<System.Object> releases = (List<System.Object>)obj;
            Dictionary<string, object> latest = releases[0] as Dictionary<string, object>;

            foreach (Dictionary<string, object> releaseObject in releases)
            {
                GitHubRelease release = new GitHubRelease
                {
                    tag = (string)releaseObject["tag_name"],
                    body = (string)releaseObject["body"],
                    id = (long)releaseObject["id"]
                };
                release.assets = ExtractAssets(releaseObject);
                releaseList.Add(release);
            }

            releaseList.Sort((relA, relB) => relA.id.CompareTo(relB.id));
            return releaseList.ToArray();
        }

        private static GitHubAsset[] ExtractAssets(Dictionary<string, object> release)
        {
            if (release["assets"] == null) return null;

            List<GitHubAsset> assetList = new List<GitHubAsset>();

            List<System.Object> assets = (List<System.Object>)release["assets"];
            foreach (Dictionary<string, object> asset in assets) {
                assetList.Add(new GitHubAsset
                {
                    htmlURL = (string)asset["browser_download_url"],
                    name = (string)asset["name"]
                });
            }

            return assetList.ToArray();
        }
    }

    // Based on https://developer.github.com/v3/repos/releases/
    public struct GitHubRelease
    {
        public string tag;
        public string body;
        public long id;
        public GitHubAsset[] assets;
    }

    public struct GitHubAsset
    {
        public string htmlURL;
        public string name;
    }
}
