using System.IO;

namespace Uplift.Schemas
{
    public partial class InstalledPackage
    {
        public void Nuke()
        {
            foreach (var spec in Install)
            {
                if(Directory.Exists(spec.Path)) {
                    Directory.Delete(spec.Path, true);
                }

                if(File.Exists(spec.Path)) {
                    File.Delete(spec.Path);
                }
                
            }
        }
    }
}