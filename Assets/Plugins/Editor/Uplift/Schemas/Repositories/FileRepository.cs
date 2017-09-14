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
using System.Text.RegularExpressions;

namespace Uplift.Schemas {

    public partial class FileRepository {

        private const string formatPattern = "{0}{1}{2}";

        public override string ToString()
        {
            return "FileRepository " + this.Path;
        }

        public override TemporaryDirectory DownloadPackage(Upset package) {
            TemporaryDirectory td = new TemporaryDirectory();

            string sourcePath = string.Format(formatPattern, Path, System.IO.Path.DirectorySeparatorChar, package.MetaInformation.dirName);

            if (Directory.Exists(sourcePath))
            {

                Uplift.Common.FileSystemUtil.CopyDirectoryWithMeta(sourcePath, td.Path);
                
            }
            else if (IsUnityPackage(sourcePath))
            {
                using (MemoryStream TarArchiveMS = new MemoryStream())
                {
                    using (FileStream originalFileStream = new FileStream(sourcePath, FileMode.Open))
                    {
                        using (GZipStream decompressionStream =
                            new GZipStream(originalFileStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(TarArchiveMS);
                            TarArchiveMS.Position = 0;
                        }
                    }
                    TarArchive reader = TarArchive.Open(TarArchiveMS);

                    string assetPath = null;
                    MemoryStream assetMS = null;
                    MemoryStream metaMS = null;
                    foreach (TarArchiveEntry entry in reader.Entries)
                    {
                        if (entry.IsDirectory) continue;

                        if (entry.Key.EndsWith("asset"))
                        {
                            if (assetMS != null)
                                throw new InvalidOperationException("Unexpected state: assetMS not null");
                            
                            assetMS = new MemoryStream();
                            entry.WriteTo(assetMS);
                            assetMS.Position = 0;
                            continue;
                        }
                        if (entry.Key.EndsWith("metaData"))
                        {
                            throw new NotSupportedException("The package has been packed by a Unity version prior to Unity5, and we do not support this. Contact the package maintainer for updated version.");
                        }
                        if (entry.Key.EndsWith("meta"))
                        {
                            metaMS = new MemoryStream();
                            entry.WriteTo(metaMS);
                            metaMS.Position = 0;
                            continue;
                        }
                        if (entry.Key.EndsWith("pathname"))
                        {
                            MemoryStream MSM = new MemoryStream();
                            entry.WriteTo(MSM);
                            MSM.Position = 0;
                            using (StreamReader SR = new StreamReader(MSM))
                            {
                                assetPath = SR.ReadToEnd().Split('\n')[0];
                            }
                        }
                        if (assetPath != null)
                        {
                            string AssetPath = System.IO.Path.Combine(td.Path, assetPath.Replace('/', System.IO.Path.DirectorySeparatorChar));
                            if (assetMS == null)
                            {
                                // asset is a directory
                                if (!Directory.Exists(AssetPath))
                                {
                                    Directory.CreateDirectory(AssetPath);
                                }
                                if (metaMS != null)
                                {
                                    string MetaPath = AssetPath + ".meta";
                                    using (FileStream FS = new FileStream(MetaPath, FileMode.Create))
                                    {
                                        metaMS.CopyTo(FS);
                                    }
                                    metaMS.Dispose();
                                    metaMS = null;
                                } else {
                                    // asset is a broken directory - missing meta
                                    Debug.LogError("Directory at path " + assetPath + " doesn't have its meta.");
                                }
                                assetPath = null;
                                continue;
                            }
                            var AssetPathDir = new FileInfo(AssetPath).Directory.FullName;
                            if (!Directory.Exists(AssetPathDir))
                            {
                                Directory.CreateDirectory(AssetPathDir);
                            }
                            using (FileStream FS = new FileStream(AssetPath, FileMode.Create))
                            {
                                assetMS.CopyTo(FS);
                            }
                            assetMS.Dispose();
                            assetMS = null;
                            if (metaMS != null)
                            {
                                string MetaPath = AssetPath + ".meta";
                                using (FileStream FS = new FileStream(MetaPath, FileMode.Create))
                                {
                                    metaMS.CopyTo(FS);
                                }
                                metaMS.Dispose();
                                metaMS = null;
                            }
                            assetPath = null;
                        }
                    }
                }
            } else
            {
                Debug.LogError(string.Format("Package {0} version {1} has an unexpected format and cannot be downloaded ", package.PackageName, package.PackageVersion));
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
                    // Don't look at me. System.IO.Path.Combine(string, string, string) doesn't work in Unity :(
                    char SC = System.IO.Path.DirectorySeparatorChar;
                    string upsetPath = Path + SC + directoryName + SC + UpsetFile;

                    if (!File.Exists(upsetPath)) continue;

                    XmlSerializer serializer = new XmlSerializer(typeof(Upset));

                    using (FileStream file = new FileStream(upsetPath, FileMode.Open))
                    {
                        Upset upset = serializer.Deserialize(file) as Upset;
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
            VersionStruct Unity5 = new VersionStruct{ Major = 5 };
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
					// Note: we allow not full unity version here. Maybe we should have a thin Unity version
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
            string upsetPath = Regex.Replace(packagePath, ".unitypackage", ".Upset.xml", RegexOptions.IgnoreCase);

            if (File.Exists(upsetPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Upset));

                using (FileStream file = new FileStream(upsetPath, FileMode.Open))
                {
                    Upset upset = serializer.Deserialize(file) as Upset;
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
