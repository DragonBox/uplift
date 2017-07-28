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
                if (spec.Type == InstallSpecType.Root)
                {
                    // Removing Root package
                    Directory.Delete(spec.Path, true);
                }
                else
                {
                    var filePath = spec.Path;

                    try
                    {
                        File.Delete(filePath);
                        File.Delete(filePath + ".meta"); // Removing meta files as well.
                    }
                    catch (FileNotFoundException)
                    {
                        Debug.Log("Warning, tracked file not found: " + filePath);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Debug.Log("Warning, tracked directory not found: " + filePath);
                    }


                    string dirName = Path.GetDirectoryName(spec.Path);

                    if (!string.IsNullOrEmpty(dirName))
                    {

                        dirPaths.Add(dirName);
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
                var recursiveDirPaths = FileSystemUtil.RecursivelyDirPaths(dirPaths).Distinct().ToList();

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