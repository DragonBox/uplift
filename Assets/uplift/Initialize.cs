using System.IO;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;


[InitializeOnLoad]
public class Initialize : MonoBehaviour {
    

    public static UpfileHandler UpfileHandler;
    
	static  Initialize() {
        if(UpfileHandler == null) {
            UpfileHandler = new UpfileHandler();
            UpfileHandler.Initialize();
        }

	}




}
