using System.IO;

namespace Schemas
{
    public partial class InstalledPackage
    {
        public void Nuke()
        {
            foreach (var spec in this.Install)
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