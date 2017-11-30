// --- BEGIN LICENSE BLOCK ---
/* 
 * Original work Copyright (c) 2017 Unity Technologies 
 * Modified work Copyright 2017-present WeWantToKnow AS
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
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Uplift.Extensions;

namespace Uplift.Common
{
    public class UnityPackage
    {
        /// <summary>
        /// Extracts a package at archivePath, and unpacks it at the same location
        /// </summary>
        public void Extract(string archivePath)
        {
            Extract(
                archivePath,
                Path.GetDirectoryName(archivePath)
            );
        }
        
        /// <summary>
        /// Extracts a package from archivePath, and unpacks it at destinationPath
        /// </summary>
        public void Extract(string archivePath, string destinationPath)
        {
            using (FileStream compressedFileStream = File.OpenRead(archivePath))
            {
                Extract(compressedFileStream, destinationPath);
            }
        }

        // From https://github.com/Unity-Technologies/dotpackage-assistant/blob/master/DotPackageAssistant.cs

        /// <summary>
        /// Extracts a package from the input stream, and unpacks it at destinationPath
        /// </summary>
        public void Extract(Stream stream, string destinationPath)
        {
            GZipStream gzipFileStream = new GZipStream(stream, CompressionMode.Decompress);
            BinaryReader reader = new BinaryReader(gzipFileStream);
            int readBufferSize = 1024 * 1024 * 10;
            int tarBlockSize = 512;
            byte[] readBuffer = new byte[readBufferSize];
            Regex hashPattern = new Regex(@"^([a-f\d]{20,})\/");

            byte[] rawAsset = null;
            byte[] rawMeta = null;
            string path = null;

            while (true)
            {
                byte[] headerBuffer = reader.ReadBytes(tarBlockSize);   //We want the header, but the header is padded to a blocksize
                if (headerBuffer.All(x => x == 0))
                {
                    //Reached end of stream
                    break;
                }
                GCHandle handle = GCHandle.Alloc(headerBuffer, GCHandleType.Pinned);
                TarHeader header;
                header = (TarHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TarHeader));
                handle.Free();

                string filename;
                unsafe
                {
                    filename = Marshal.PtrToStringAnsi((IntPtr)header.filename, 100);
                }
                filename = filename.Trim();
                filename = filename.TrimEnd(new char[] { (char)0 });

                string ustar;
                unsafe
                {
                    ustar = Marshal.PtrToStringAnsi((IntPtr)header.ustar, 6);
                }
                string prefix = string.Empty;
                if (ustar.Equals("ustar"))
                {
                    unsafe
                    {
                        prefix = Marshal.PtrToStringAnsi((IntPtr)header.prefix, 155);
                    }
                }
                prefix = prefix.Trim();
                prefix = prefix.TrimEnd(new char[] { (char)0 });

                string fullname = prefix + filename;
				if(fullname.StartsWith("./")) fullname = fullname.Substring(2);
                Match hashMatch = hashPattern.Match(fullname);

                bool extractPathName = false;
                bool extractRawMeta = false;
                bool extractRawAsset = false;

                if (hashMatch.Success)
                {
                    if (fullname.EndsWith("/asset.meta"))
                    {
                        extractRawMeta = true;
                    }
                    if (fullname.EndsWith("/asset"))
                    {
                        extractRawAsset = true;
                    }
                    if (fullname.EndsWith("/pathname"))
                    {
                        extractPathName = true;
                    }
                }

                string rawFilesize;
                unsafe
                {
                    rawFilesize = Marshal.PtrToStringAnsi((IntPtr)header.filesize, 12);
                }
                string filesize = rawFilesize.Trim();
                filesize = filesize.TrimEnd(new char[] { (char)0 });

                //Convert the octal string to a decimal number
                try
                {
                    int filesizeInt = Convert.ToInt32(filesize, 8);
                    int toRead = filesizeInt;
                    int toWrite = filesizeInt;
                    int modulus = filesizeInt % tarBlockSize;
                    if (modulus > 0)
                        toRead += (tarBlockSize - modulus);    //Read the file and assume it's also 512 byte padded
                    while (toRead > 0)
                    {
                        int readThisTime = Math.Min(readBufferSize, toRead);
                        int writeThisTime = Math.Min(readBufferSize, toWrite);
                        readBuffer = reader.ReadBytes(readThisTime);
                        if (extractPathName)
                        {
                            if (toRead > readThisTime)
                                throw new Exception("Assumed a pathname would fit in a single read!");
                            string pathnameFileContents = Encoding.UTF8.GetString(readBuffer, 0, filesizeInt);
                            path = FormatPath(pathnameFileContents.Split(new char[] { '\n' })[0]);
                        }
                        else if(extractRawMeta)
                        {
                            if(rawMeta == null) rawMeta = new byte[0];
                            int rawLength = rawMeta.Length;
                            Array.Resize<byte>(ref rawMeta, rawLength + writeThisTime);
                            Array.Copy(readBuffer, 0, rawMeta, rawLength, writeThisTime);
                        }
                        else if(extractRawAsset)
                        {
                            if(rawAsset == null) rawAsset = new byte[0];
                            int rawLength = rawAsset.Length;
                            Array.Resize<byte>(ref rawAsset, rawLength + writeThisTime);
                            Array.Copy(readBuffer, 0, rawAsset, rawLength, writeThisTime);
                        }
                        toRead -= readThisTime;
                        toWrite -= writeThisTime;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(String.Format("Caught Exception converting octal string to int: {0}", ex.Message));
                    foreach (byte fsChar in filesize)
                    {
                        Debug.Log(fsChar);
                    }
                    throw;
                }

                // Path has been read, write to file system
                if(path != null)
                {
                    string target = Path.Combine(destinationPath, path);
                    Uplift.Common.FileSystemUtil.EnsureParentExists(target);

                    // Asset or not? (ie directory)
                    if(rawAsset == null)
                    {
                        Directory.CreateDirectory(target);
                    }
                    else
                    {
                        using(var tw = new StreamWriter(target, false, new UTF8Encoding(false)))
                        {
                            tw.BaseStream.Write(rawAsset, 0, rawAsset.Length);
                        }
                        rawAsset = null;
                    }

                    // Create meta
                    if(rawMeta != null)
                    {
                        using(var tw = new StreamWriter(target + ".meta", false, new UTF8Encoding(false)))
                        {
                            tw.BaseStream.Write(rawMeta, 0, rawMeta.Length);
                        }
                        rawMeta = null;
                    }

                    path = null;
                }
            }
        }

        private string FormatPath(string path)
        {
            string outPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).TrimStart(new char[] { '/' });
            if (outPath.StartsWith("./"))
                outPath = outPath.Substring(2);
            return outPath;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct TarHeader
        {
            public fixed byte filename[100];
            public fixed byte filemode[8];
            public fixed byte uid[8];
            public fixed byte gid[8];
            public fixed byte filesize[12]; //Octal
            public fixed byte modtime[12];
            public fixed byte checksum[8];
            public byte linkIndicator;      //OR Type flag in UStar format
            public fixed byte linkname[100];
            //USTAR fields
            public fixed byte ustar[6];
            public fixed byte ustarVersion[2];
            public fixed byte uname[32];
            public fixed byte ugroupname[32];
            public fixed byte major[8];
            public fixed byte minor[8];
            public fixed byte prefix[155];
        };
    }
}