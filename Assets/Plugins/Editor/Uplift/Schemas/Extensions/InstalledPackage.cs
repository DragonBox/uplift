using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Uplift.Schemas
{
    public partial class InstalledPackage
    {
        public void Nuke()
        {
            var dirPaths = new List<string>();
            
            foreach (InstallationSpecs spec in Install)
            {
                if (spec.Kind == InstallSpecType.Root)
                {
                    // Removing Root package
                    Directory.Delete(spec.Path, true);
                }
                else
                {
                    var sourceDir = UpfileHandler.Instance().GetDestinationFor(spec.Kind).Location;

                    var filePath = Path.Combine(sourceDir, spec.Path);

                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (FileNotFoundException)
                    {
                        Debug.Log("Warning, tracked file not found: " + filePath);
                    }


                    string dirName = Path.GetDirectoryName(spec.Path);

                    if (!string.IsNullOrEmpty(dirName))
                    {

                        dirPaths.Add(Path.Combine(sourceDir, dirName));
                    }
                    
                    

                }
                
            }

            foreach (var p in dirPaths.Distinct())
            {
                if(string.IsNullOrEmpty(p))
                {
                    continue;
                }
                
                try
                {
                    Debug.Log("Removing " + p);
                    Directory.Delete(p, true);
                }
                catch (DirectoryNotFoundException)
                {
                    // Few places where this might happen:
                    // First place
                }
            }
        }
    }
}