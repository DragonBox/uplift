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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using Uplift.Extensions;
using UnityEngine;
using UnityEditor;
using Uplift.Common;
using Version = Uplift.Common.Version;
using System.Text.RegularExpressions;

namespace Uplift.Schemas {

    public partial class FileRepository
    {
        public override string ToString()
        {
            return "FileRepository " + this.Path;
        }

        public override TemporaryDirectory DownloadPackage(Upset package) {
            TemporaryDirectory td = new TemporaryDirectory();
            
            string sourcePath = System.IO.Path.Combine(Path, package.MetaInformation.dirName);

            if (Directory.Exists(sourcePath))
            {
                Uplift.Common.FileSystemUtil.CopyDirectoryWithMeta(sourcePath, td.Path);
            }
            else if (IsUnityPackage(sourcePath))
            {
                UnityPackageOpener opener = new UnityPackageOpener();
                opener.OpenUnityPackage(sourcePath, td.Path, false);
            }
            else
            {
                Debug.LogError(string.Format("Package {0} version {1} found at {2} has an unexpected format and cannot be downloaded ", package.PackageName, package.PackageVersion, sourcePath));
            }

            return td;
        }

        private static bool IsUnityPackage(string Path)
        {
            return File.Exists(Path) && ".unitypackage".Equals(System.IO.Path.GetExtension(Path), StringComparison.CurrentCultureIgnoreCase);
        }

        public override Upset[] ListPackages() {
            List<Upset> upsetList = new List<Upset>();

            AddExplodedDirectories(upsetList);
            AddUnityPackages(upsetList);

            return upsetList.ToArray();
        }

        private void AddExplodedDirectories(List<Upset> upsetList) {
            string[] directories =  Directory.GetDirectories(Path);
            foreach(string directoryPath in directories)
            {
                string directoryName = directoryPath.Split(System.IO.Path.DirectorySeparatorChar).Last();
                try
                {
                    string upsetPath = Uplift.Common.FileSystemUtil.JoinPaths(Path, directoryName, UpsetFile);
                    if (!File.Exists(upsetPath)) continue;

                    StrictXmlDeserializer<Upset> deserializer = new StrictXmlDeserializer<Upset>();

                    using (FileStream file = new FileStream(upsetPath, FileMode.Open))
                    {
                        Upset upset = deserializer.Deserialize(file);
                        if (upset.Configuration != null && upset.Configuration.Length != 0)
                        {
                            foreach (InstallSpecPath spec in upset.Configuration)
                            {
                                spec.Path = Uplift.Common.FileSystemUtil.MakePathOSFriendly(spec.Path);
                            }
                        }
                        upset.MetaInformation.dirName = directoryName;
                        upsetList.Add(upset);
                    }
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogErrorFormat("Could not load package at {0}, ignoring it ({1}):\n{2}", directoryName, e.Message, e.StackTrace);
                }
            }
        }

        private void AddUnityPackages(List<Upset> upsetList) {
            string[] files =  Directory.GetFiles(Path, "*.*");
            Version Unity5 = new Version{ Major = 5 };
            foreach(string FileName in files)
            {
                try
                {
                    if (!IsUnityPackage(FileName))
                    {
                        continue;
                    }
                    // assume unityPackage doesn't contain an upset file for now. In the future we can support it
                    Upset upset = TryLoadUpset(FileName);
                    if (upset == null) continue;
                    // Note: the spec may contain incomplete unity version here (e.g. 5.6). Maybe we should have a ParseThinUnityVersion
                    if (VersionParser.ParseIncompleteVersion(upset.UnityVersion) < Unity5) {
                        throw new NotSupportedException("The package has been packed by a Unity version prior to Unity5, and we do not support this. Contact the package maintainer for updated version.");
                    }
                    upsetList.Add(upset);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogErrorFormat("Could not load package at {0}, ignoring it ({1}):\n{2}", FileName, e.Message, e.StackTrace);
                }
            }
        }

        private static Upset TryLoadUpset(string packagePath)
        {
            string upsetPath = Regex.Replace(packagePath, ".unitypackage$", ".Upset.xml", RegexOptions.IgnoreCase);

            if (File.Exists(upsetPath))
            {
                StrictXmlDeserializer<Upset> deserializer = new StrictXmlDeserializer<Upset>();

                using (FileStream file = new FileStream(upsetPath, FileMode.Open))
                {
                    Upset upset = deserializer.Deserialize(file);
                    if (upset.Configuration != null && upset.Configuration.Length != 0)
                    {
                        foreach (InstallSpecPath spec in upset.Configuration)
                        {
                            spec.Path = Uplift.Common.FileSystemUtil.MakePathOSFriendly(spec.Path);
                        }
                    }
                    upset.MetaInformation.dirName = packagePath.Split(System.IO.Path.DirectorySeparatorChar).Last();

                    return upset;
                }
            }
            else
            {
                Debug.LogWarning("Unity package found at " + packagePath + " has no matching Upset. It should be at " + upsetPath);
            }

            return null;
        }
    }
}
