using Uplift;

namespace Uplift.Testing.Helpers
{
    class UpliftManagerExposer : UpliftManager
    {
        public static void ClearAllInstances()
        {
            UpbringExposer.ClearInstance();
            UpfileExposer.ClearInstance();
            instance = null;
        }
    }
}
