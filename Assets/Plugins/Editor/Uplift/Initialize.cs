using UnityEditor;
using UnityEngine;
using Uplift.Schemas;

namespace Uplift
{
    [InitializeOnLoad]
    public class Initialize : MonoBehaviour
    {
        static Initialize()
        {
            Upfile.Instance();
        }
    }
}
