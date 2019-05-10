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

#if UNITY_5_3_OR_NEWER
using System.IO;
using System.Xml.Serialization;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.Testing.Helpers
{
	public class UpfileExposer : Upfile
	{
		public static UpfileExposer TestingInstance()
		{
			if (instance == null)
			{
				InitializeTestingInstance();
			}

			return (instance as UpfileExposer);
		}

		public static void ClearInstance()
		{
			instance = null;
		}

		internal static void InitializeTestingInstance()
		{
			instance = new UpfileExposer();
		}

		internal static void SetInstance(Upfile upfile)
		{
			instance = upfile;
			instance.LoadPackageList();
		}

		internal static Upfile LoadTestXml(string path)
		{
			StrictXmlDeserializer<Upfile> deserializer = new StrictXmlDeserializer<Upfile>();

			using (FileStream fs = new FileStream(path, FileMode.Open))
			{
				Upfile upfile = deserializer.Deserialize(fs);

				upfile.MakePathConfigurationsOSFriendly();

				if (upfile.Repositories != null)
				{
					foreach (Repository repo in upfile.Repositories)
					{
						if (repo is FileRepository)
						{
							(repo as FileRepository).Path = FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
						}
					}
				}

				return upfile;
			}
		}
	}
}
#endif