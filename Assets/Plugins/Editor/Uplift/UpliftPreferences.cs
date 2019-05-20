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
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;

namespace Uplift.Schemas
{
	public partial class UpliftPreferences
	{
		public static readonly string folderName = ".uplift";
		public static readonly string defaultFileName = "preferences.xml";

		public static UpliftPreferences FromDefaultFile()
		{
			UpliftPreferences result;
			//if (TryLoadFromFile(GetProjectLocation(), out result))
			//	return result;
			if (TryLoadFromFile(GetGlobalLocation(), out result))
				return result;

			return new UpliftPreferences();
		}

		public static string GetGlobalLocation()
		{
			string sourceDir = System.IO.Path.Combine(GetHomePath(), folderName);
			return System.IO.Path.Combine(sourceDir, defaultFileName);
		}

		public static string GetProjectLocation()
		{
			throw new NotImplementedException();
		}

		public static bool TryLoadFromFile(string source, out UpliftPreferences result)
		{
			result = new UpliftPreferences();

			if (!File.Exists(source))
			{
				Debug.Log("No preferences file at " + source);
				return false;
			}

			XmlSerializer deserializer = new XmlSerializer(typeof(UpliftPreferences));

			using (FileStream fs = new FileStream(source, FileMode.Open))
			{
				try
				{
					result = deserializer.Deserialize(fs) as UpliftPreferences;
				}
				catch (InvalidOperationException)
				{
					Debug.LogError(string.Format("Global Override file at {0} is not well formed", source));
					return false;
				}

				return true;
			}

		}

		public static string GetHomePath()
		{
			return (Environment.OSVersion.Platform == PlatformID.Unix ||
					Environment.OSVersion.Platform == PlatformID.MacOSX) ?
				Environment.GetEnvironmentVariable("HOME") :
				Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		}

		public string GetProxiedUrl(string url)
		{
			if (!UseGithubProxy || string.IsNullOrEmpty(GithubProxyUrl))
				return url;

			Debug.Log("Proxying github api with " + GithubProxyUrl);
			return url.Replace("https://api.github.com", GithubProxyUrl);
		}
	}
}