using System.IO;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;


[InitializeOnLoad]
public class Initialize : MonoBehaviour {
    

    
	static  Initialize() {
        // Make sure we initialize UpfileHandler
        UpfileHandler.Instance();

	}




}
