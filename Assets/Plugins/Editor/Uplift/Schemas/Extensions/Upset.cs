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

using Uplift.Common;

namespace Uplift.Schemas
{
	public partial class Upset
	{
		public struct Meta
		{
			public string dirName;
		}

		public Meta MetaInformation;

		override public string ToString()
		{
			var result = this.PackageName + "~" + this.PackageVersion;
			if (this.UnityVersion != null)
			{
				result = string.Format("{0} ({1})", result, this.UnityVersion);

			}
			return result;
		}
		public int PackageVersionAsNumber()
		{
			Version version = VersionParser.ParseVersion(PackageVersion);
			int result = version.Major * 1000000;
			if (version.Minor != null) result += ((int)version.Minor) * 1000;
			if (version.Patch != null) result += (int)version.Patch;
			return result;
		}

	}
}