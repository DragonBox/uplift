using Uplift.Schemas;
using System.IO;

namespace Uplift.Testing.Helpers
{
    public class UpbringExposer : Upbring
    {
        public static void TryPurgeUpbring()
        {
            if (File.Exists(UpbringPath)) File.Delete(UpbringPath);
        }

        public static void ClearInstance()
        {
            instance = null;
        }
    }
}
