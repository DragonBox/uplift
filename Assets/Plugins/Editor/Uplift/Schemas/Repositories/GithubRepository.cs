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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Extensions;
using Uplift.GitHubModule;

namespace Uplift.Schemas
{
	public partial class GithubRepository : Repository
	{
		private GitHubRelease[] releases;

		public override TemporaryDirectory DownloadPackage(Upset package)
		{
			string sourceName = Regex.Replace(package.MetaInformation.dirName, ".Upset.xml$", ".unitypackage", RegexOptions.IgnoreCase);

			releases = GetPackagesReleases();
			GitHubRelease release = releases.FirstOrDefault(rel => rel.assets.Any(asset => asset.name.Contains(sourceName)));

			if (release == null)
				throw new ArgumentException(string.Format("Package {0} is not present in this repository", package.PackageName));

			GitHubAsset packageAsset = release.assets.First(asset => asset.name.Contains(sourceName));
			TemporaryDirectory td = new TemporaryDirectory();
			using (StreamReader sr = new StreamReader(GitHub.GetAssetStream(packageAsset, GetToken())))
			{
				UnityPackage unityPackage = new UnityPackage();
				unityPackage.Extract(sr.BaseStream, td.Path);
			}
			return td;
		}

		public override Upset[] ListPackages()
		{
			releases = GetPackagesReleases();

			GitHubAsset[] upsetAssets = releases
				.SelectMany<GitHubRelease, GitHubAsset>(rel => rel.assets.Where(asset => asset.name.EndsWith("Upset.xml")))
				.ToArray();

			string progressBarTitle = "Parsing Upsets from GitHub repository";
			int index = 0;

			List<Upset> upsetList = new List<Upset>();
			EditorUtility.DisplayProgressBar(progressBarTitle, "Please wait a little bit while Uplift parses the Upset in the GitHub repository at " + urlField, 0f);

			string assetPath;
			try
			{
				foreach (GitHubAsset asset in upsetAssets)
				{
					try
					{
						StrictXmlDeserializer<Upset> deserializer = new StrictXmlDeserializer<Upset>();

						EditorUtility.DisplayProgressBar(
							progressBarTitle,
							"Parsing " + asset.name,
							(float)(index++) / upsetAssets.Length
						);

						if (!TryGetCachedItem(asset.name, out assetPath))
						{
							using (StreamReader sr = new StreamReader(GitHub.GetAssetStream(asset, GetToken())))
							using (FileStream fs = new FileStream(assetPath, FileMode.Create))
							{
								sr.BaseStream.CopyTo(fs);
							}
						}

						using (FileStream fs = new FileStream(assetPath, FileMode.Open))
						{
							Upset upset = deserializer.Deserialize(fs);
							upset.MetaInformation.dirName = asset.name;
							upsetList.Add(upset);
						}
					}
					catch (Exception e)
					{
						Debug.LogErrorFormat("An error occured while trying to get asset {0} ({1}, {2}) from GithubRepository", asset.name, asset.htmlURL, asset.apiURL);
						throw e;
					}
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}

			return upsetList.ToArray();
		}

		private GitHubRelease[] GetPackagesReleases()
		{
			if (releases == null)
			{
				IEnumerator e = GitHub.LoadReleases(urlField, GetToken());
				do { Thread.Sleep(1000); } while (e.MoveNext());

				GitHubRelease[] fetchedReleases = (GitHubRelease[])e.Current;

				if (fetchedReleases == null)
					throw new ApplicationException("This github repository does not have releases");

				string[] tagArray = (tagListField == null || tagListField.Length == 0) ?
					new string[] { "packages" } :
					tagListField;

				releases = fetchedReleases.Where(rel => tagArray.Contains(rel.tag)).ToArray();

				// Fail hard if no release could be found
				if (releases.Length == 0)
					throw new ApplicationException("This repository does not have a release correctly tagged");

				// Logs missing tags if only some of them were found
				if (releases.Length < tagArray.Length)
				{
					string missingTags = string.Join(
						", ",
						tagArray.Where(tag => !releases.Any(rel => rel.tag == tag)).ToArray()
					);

					Debug.LogWarningFormat("Could not find a matching release for all of your tags (missing {0})", missingTags);
				}
			}

			return releases;
		}

		private string GetToken()
		{
			UpliftSettings dotUplift = UpliftSettings.FromDefaultFile();
			if (dotUplift.AuthenticationMethods != null)
				foreach (RepositoryAuthentication auth in dotUplift.AuthenticationMethods)
				{
					if (!(auth is RepositoryToken)) continue;
					if (!(auth.Repository == urlField)) continue;

					return (auth as RepositoryToken).Token;
				}

			Debug.LogWarning("Could not find authentication method for repository at " + urlField);
			return null;
		}

		private bool TryGetCachedItem(string fileName, out string path)
		{
			path = Path.Combine(GetCachePath(), fileName);
			return File.Exists(path);
		}

		private string GetCachePath()
		{
			string path = Path.Combine(FileSystemUtil.GetAppDataPath(), "Uplift");
			FileSystemUtil.EnsureDirExists(path);
			return path;
		}

		public override string ToString()
		{
			return "GithubRepository: " + urlField;
		}
	}
}