#if UNITY_5_3_OR_NEWER
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly:InternalsVisibleTo("UpliftTesting")]

namespace Uplift
{
    public static class TestingProperties
    {
        public static void SetLogging(bool on)
        {
            Debug.logger.logEnabled = on;
        }
    }
}
#endif