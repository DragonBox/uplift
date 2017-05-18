using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MenuItems : MonoBehaviour {

	static UpfileHandler UpfileHandler;
	static MenuItems() {
		UpfileHandler = Initialize.UpfileHandler;
	}
	[MenuItem("Uplift/Generate Upfile", true, 0)]
	static bool CheckForUpfile() {
		return !UpfileHandler.CheckForUpfile();
	}
	[MenuItem("Uplift/Generate Upfile", false, 0)]
	static void GenerateUpfile() {
		Debug.Log("Hi, I Generate upfile!");

		Schemas.Upfile upfile = new Schemas.Upfile();
		
		upfile.UnityVersion = Application.unityVersion;

		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Schemas.Upfile));
		serializer.Serialize(new FileStream(UpfileHandler.upfilePath, FileMode.CreateNew), upfile);
		Debug.Log("Done");
	}

	[MenuItem("Uplift/Check Dependencies", false, 20)]
	static void CheckDependencies() {
		
	}

	[MenuItem("Uplift/Install Dependencies", false, 20)]
	static void InstallDependencies() {
		UpfileHandler.InstallDependencies();
	}





}
