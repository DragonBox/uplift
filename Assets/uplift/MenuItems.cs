using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MenuItems : MonoBehaviour {


	static MenuItems() {
		
	}

	[MenuItem("Uplift/Refresh", false, -15)]
	static void RefreshUpfile() {
		UpfileHandler.Instance().Initialize();
	}


	[MenuItem("Uplift/Generate Upfile", true, 0)]
	static bool CheckForUpfile() {
		return !UpfileHandler.Instance().CheckForUpfile();
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
		UpfileHandler.Instance().InstallDependencies();
		UnityEditor.AssetDatabase.Refresh();
	}


	[MenuItem("Uplift/Show Update Window", false,30)]
	static void ShowUpdateWindow() {
		UpdateUtility uw = new UpdateUtility();
		uw.ShowUtility();
		//EditorWindow.GetWindow(typeof(UpdateUtility));
	}

	[MenuItem("Uplift/Debug/List Packages", false, 50)]
	static void ListPackages() {
		UpfileHandler.Instance().ListPackages();
	}

		[MenuItem("Uplift/Debug/Nuke All Packages", false, 50)]
	static void NukePackages() {
		UpfileHandler.Instance().NukePackages();
		UnityEditor.AssetDatabase.Refresh();
	}




}
