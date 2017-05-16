using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Collections;
using Schemas;

public class UpfileHandler {
    const bool debugMode = true;
    public const string upfilePath = "Upfile.xml";
    Schemas.Upfile Upfile;

     
    public void Log(string str) {
        if(debugMode) {
            Debug.Log(str);
        }
    }

    public void Initialize() {
        if(CheckForUpfile()) {
            Log("File exists");
            InternalLoadFile();
            CheckUnityVersion();
        } else {
            Log("Upfile doesn't exist. Generate new one from menu if needed.");
        }
    }
    
    public bool CheckForUpfile() {
        return File.Exists(upfilePath);
    }

    private void InternalLoadFile() {
        Upfile = LoadFile();
    }

    static public Schemas.Upfile LoadFile() {
        XmlSerializer serializer = new XmlSerializer(typeof(Schemas.Upfile));

        return serializer.Deserialize(new FileStream(upfilePath, FileMode.Open)) as Schemas.Upfile;
    }

    public void InstallDependencies() {
        //FIXME: We should check for all repositories, not the first one
        FileRepositoryType rt = (FileRepositoryType) Upfile.Repositories[0];

        Debug.Log(rt.ToString());
        foreach(Schemas.PackageType package in Upfile.Dependencies) {
            rt.InstallPackage(package);
        }
    }
    //FIXME: Prepare proper version checker
    public void CheckUnityVersion() {
        String upfileVersion = Upfile.Requirements.Unity.Version;
        String unityVersion = Application.unityVersion; 
        if(unityVersion != upfileVersion) {
            Debug.LogError(String.Format("Uplift: Upfile.xml Unity Version ({0}) doesn't match Unity's one  ({1}).", upfileVersion, unityVersion));
        } else {
            Log("Version check successful");
        }
    }
}