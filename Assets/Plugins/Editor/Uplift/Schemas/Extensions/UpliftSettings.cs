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
using FileSystemUtil = Uplift.Common.FileSystemUtil;

namespace Uplift.Schemas
{
    public partial class UpliftSettings
    {
        public static readonly string folderName = ".uplift";
        public static readonly string defaultFileName = "settings.xml";

        internal UpliftSettings() {}

        public static UpliftSettings FromDefaultFile()
        {
            string sourceDir = System.IO.Path.Combine(GetHomePath(), folderName);
            string source = System.IO.Path.Combine(sourceDir, defaultFileName);
            
            return FromFile(source);
        }
        
        public static UpliftSettings FromFile(string source)
        {
            UpliftSettings result = new UpliftSettings { Repositories = new Repository[0], AuthenticationMethods = new RepositoryToken[0] };

            if(!File.Exists(source))
            {
                Debug.Log("No local settings file detected at " + source);
                return result;
            }

            StrictXmlDeserializer<UpliftSettings> deserializer = new StrictXmlDeserializer<UpliftSettings>();

            using (FileStream fs = new FileStream(source, FileMode.Open))
            {
                try
                {
                    result = deserializer.Deserialize(fs);

                    foreach (Repository repo in result.Repositories)
                    {
                        if (repo is FileRepository)
                            (repo as FileRepository).Path = FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
                    }
                }
                catch (InvalidOperationException)
                {
                    Debug.LogError(string.Format("Global Override file at {0} is not well formed", source));
                }

                return result;
            }
        }

        public static string GetHomePath()
        {
            return (Environment.OSVersion.Platform == PlatformID.Unix ||
                               Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }


    }
}