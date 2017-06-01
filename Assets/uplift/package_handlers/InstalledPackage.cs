using System.IO;

namespace Schemas
{
    public partial class InstalledPackage
    {
        public void Nuke()
        {
            foreach (var spec in this.Install)
            {
                Directory.Delete(spec.Path, true);
            }
        }
    }
}