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
            Debug.Log("Upfile loading...");
            Upfile.Instance();
        }
    }
}
