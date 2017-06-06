using UnityEditor;
using UnityEngine;

namespace Uplift
{
    [InitializeOnLoad]
    public class Initialize : MonoBehaviour
    {
        static Initialize()
        {
            // Make sure we initialize UpfileHandler
            UpfileHandler.Instance();
        }

    }
}
