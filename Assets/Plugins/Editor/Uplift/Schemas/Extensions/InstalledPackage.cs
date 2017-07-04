using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace Uplift.Schemas
{
    public partial class InstalledPackage
    {
        public void Nuke()
        {
            foreach (InstallationSpecs spec in Install)
            {
                if (spec.Kind == InstallSpecType.Root)
                {
                    // Removing Root package
                    Directory.Delete(spec.Path, true);
                }
                else
                {
                    string sourceDir = UpfileHandler.Instance().GetDestinationFor(spec.Kind).Location;

                    string filePath = Path.Combine(sourceDir, spec.Path);

                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (FileNotFoundException)
                    {
                        Debug.Log("Warning, tracked file not found: " + filePath);
                    }
                    
                }
                
            }
            
            // TODO: Remove leftover dirs
        }
    }
}