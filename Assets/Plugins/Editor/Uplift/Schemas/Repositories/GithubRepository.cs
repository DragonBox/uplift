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
using System.Threading;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.GitHubModule;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Uplift.Schemas
{
    public partial class GithubRepository : Repository
    {
        private GitHubRelease release;

        public override TemporaryDirectory DownloadPackage(Upset package)
        {
            string sourceName = Regex.Replace(package.MetaInformation.dirName, ".Upset.xml$", ".unitypackage", RegexOptions.IgnoreCase);

            release = GetPackagesRelease();
            if(!release.assets.Any(asset => asset.name.Contains(sourceName)))
                throw new ArgumentException(string.Format("Package {0} is not present in this repository", package.PackageName));

            GitHubAsset packageAsset = release.assets.First(asset => asset.name.Contains(sourceName));
            TemporaryDirectory td = new TemporaryDirectory();
            using(StreamReader sr = new StreamReader(GitHub.GetAssetStream(packageAsset, GetToken())))
            {
                UnityPackage unityPackage = new UnityPackage();
                unityPackage.Extract(sr.BaseStream, td.Path);
            }
            return td;
        }

        public override Upset[] ListPackages()
        {
            release = GetPackagesRelease();

            GitHubAsset[] upsetAssets = release.assets.Where(asset => asset.name.EndsWith("Upset.xml")).ToArray();

            string progressBarTitle = "Parsing Upsets from GitHub repository";
            int index = 0;

            List<Upset> upsetList = new List<Upset>();
            EditorUtility.DisplayProgressBar(progressBarTitle, "Please wait a little bit while Uplift parses the Upset in the GitHub repository at " + urlField, 0f);
            foreach(GitHubAsset asset in upsetAssets)
            {
                StrictXmlDeserializer<Upset> deserializer = new StrictXmlDeserializer<Upset>();
                
                using(StreamReader sr = new StreamReader(GitHub.GetAssetStream(asset, GetToken())))
                {
                    Upset upset = deserializer.Deserialize(sr.BaseStream);
                    upset.MetaInformation.dirName = asset.name;
                    upsetList.Add(upset);
                }
                
                EditorUtility.DisplayProgressBar(
                    progressBarTitle,
                    "Parsing " + asset.name, 
                    (100f * (float)(++index)) / upsetAssets.Length
                );
            }

            EditorUtility.ClearProgressBar();
            return upsetList.ToArray();
        }

        private GitHubRelease GetPackagesRelease()
        {
            if(release == null)
            {
                IEnumerator e = GitHub.LoadReleases(urlField, GetToken());
                do { Thread.Sleep(1000); } while (e.MoveNext());

                GitHubRelease[] releases = (GitHubRelease[]) e.Current;
                if (releases == null)
                    throw new ApplicationException("This github repository does not have releases");

                release = releases.FirstOrDefault(rel => rel.tag == "packages");
                
                if (release == null)
                    throw new ApplicationException("This repository does not have a release tagged as 'packages'");

            }

            return release;    
        }

        private string GetToken()
        {
            UpliftSettings dotUplift = UpliftSettings.FromDefaultFile();
            if(dotUplift.AuthenticationMethods != null)
                foreach(RepositoryAuthentication auth in dotUplift.AuthenticationMethods)
                {
                    if(!(auth is RepositoryToken)) continue;
                    if(!(auth.Repository == urlField)) continue;

                    return (auth as RepositoryToken).Token;
                }

            Debug.LogWarning("Could not find authentication method for repository at " + urlField);
            return null;
        }
    }
}
