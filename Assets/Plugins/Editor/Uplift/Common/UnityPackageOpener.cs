using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using UnityEngine;
using Uplift.Extensions;

namespace Uplift.Common
{
    public class UnityPackageOpener
    {
        public void OpenUnityPackage(string archivePath, bool deleteAfterOpening = false)
        {
            OpenUnityPackage(
                archivePath,
                Path.GetDirectoryName(archivePath),
                deleteAfterOpening
            );
        }
        public void OpenUnityPackage(string archivePath, string destinationPath, bool deleteAfterOpening = false)
        {
            using (MemoryStream TarArchiveMS = new MemoryStream())
            {
                using (FileStream originalFileStream = new FileStream(archivePath, FileMode.Open))
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
                        string AssetPath = System.IO.Path.Combine(destinationPath, assetPath.Replace('/', System.IO.Path.DirectorySeparatorChar));
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

            if(deleteAfterOpening)
            {
                File.Delete(archivePath);
            }
        }
    }
}