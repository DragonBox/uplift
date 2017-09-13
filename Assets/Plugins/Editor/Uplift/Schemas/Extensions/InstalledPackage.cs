using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Uplift.Common;

namespace Uplift.Schemas
{
    public partial class InstalledPackage
    {
        public void Nuke()
        {
            var dirPaths = new List<string>();
            
            foreach (InstallSpec spec in Install)
            {
                if(spec is InstallSpecPath)
                {
                    InstallSpecPath specPath = spec as InstallSpecPath;
                    var friendlyPath = Uplift.Common.FileSystemUtil.MakePathWindowsFriendly(specPath.Path);
                    if (specPath.Type == InstallSpecType.Root)
                    {
                        // Removing Root package
                        Directory.Delete(friendlyPath, true);
                    }
                    else
                    {
                        try
                        {
                            File.Delete(friendlyPath);
                            File.Delete(friendlyPath + ".meta"); // Removing meta files as well.
                        }
                        catch (FileNotFoundException)
                        {
                            Debug.Log("Warning, tracked file not found: " + friendlyPath);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Debug.Log("Warning, tracked directory not found: " + friendlyPath);
                        }


                        string dirName = Path.GetDirectoryName(friendlyPath);

                        if (!string.IsNullOrEmpty(dirName))
                        {
                            dirPaths.Add(dirName);
                        }
                    }
                }
                else if(spec is InstallSpecGUID)
                {
                    InstallSpecGUID specGuid = spec as InstallSpecGUID;
                    string guidPath = AssetDatabase.GUIDToAssetPath(specGuid.Guid);
                    if (specGuid.Type == InstallSpecType.Root)
                    {
                        // Removing Root package
                        Directory.Delete(guidPath, true);
                    }
                    else
                    {
                        try
                        {
                            File.Delete(guidPath);
                            File.Delete(guidPath + ".meta"); // Removing meta files as well.
                        }
                        catch (FileNotFoundException)
                        {
                            Debug.Log("Warning, tracked file not found: " + guidPath);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Debug.Log("Warning, tracked directory not found: " + guidPath);
                        }

                        string dirName = Path.GetDirectoryName(guidPath);

                        if (!string.IsNullOrEmpty(dirName))
                        {
                            dirPaths.Add(dirName);
                        }
                    }
                }          
            }

            // An itchy bit.
            // Paths can nest. Ordering of paths is not guaranteed.
            // So we'll loop over removing dirs to the point, where no action has been made.
            
            var actionsDone = 1;
            var loopCounter = 1;

            while (actionsDone != 0)
            {
                if (loopCounter > 5)
                {
                    Debug.LogWarning(
                        "Warning: Nuke Dependency Loop has done more than 5 rounds. This might or might not be error, depending on setup"
                        );           
                }
                
                actionsDone = 0;
                loopCounter++;

                // We're recursively listing the directories we're keeping so that we can extract empty directories.
                var recursiveDirPaths = Uplift.Common.FileSystemUtil.RecursivelyDirPaths(dirPaths).Distinct().ToList();

                foreach (var p in recursiveDirPaths)
                {
                    
                    if (string.IsNullOrEmpty(p))
                    {
                        continue;
                    }

                    if (!Directory.Exists(p))
                    {
                        continue; 
                    }
                    
                    var filesInDirectory = Directory.GetFiles(p).Length;
                    var directoriesInDirectory = Directory.GetDirectories(p).Length;

                    if (filesInDirectory + directoriesInDirectory > 0)
                    {
                        continue;
                    }

                    try
                    {
                        Directory.Delete(p, true);
                        actionsDone += 1;
                        File.Delete(p + ".meta"); // .meta file for directory might exist, remove it as well
                    }
                    catch (DirectoryNotFoundException)
                    {
                        
                    }
                }
            }
        }
    }
}