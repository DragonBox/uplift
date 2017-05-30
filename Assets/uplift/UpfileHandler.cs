using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Collections;
using Schemas;

// This should be Singleton
public class UpfileHandler {
    const bool debugMode = true;
    public const string upfilePath = "Upfile.xml";
    Upfile Upfile;

    private static UpfileHandler instance;

    public void Initialize() {
        if(CheckForUpfile()) {
            InternalLoadFile();
            CheckUnityVersion();
        } else {
            Debug.Log("Upfile doesn't exist. Generate new one from menu if needed.");
        }
    }
    
    public static UpfileHandler Instance() {
        if(instance == null) {
            var uph = new UpfileHandler();
            instance = uph;
            uph.Initialize();
            return uph;
        } else {
            return instance;
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

    public string GetPackagesRootPath() {
        return Upfile.PackagesRootPath;
    }

    public void InstallDependencies() {
        //FIXME: We should check for all repositories, not the first one
        //FileRepository rt = (FileRepository) Upfile.Repositories[0];
        PackageHandler pHandler = new PackageHandler();

        foreach(DependencyDefinition packageDefinition in Upfile.Dependencies) {
            PackageHandler.PackageRepo result = pHandler.FindPackageAndRepository(packageDefinition, Upfile.Repositories);
            if(result.repository != null) {
                result.repository.SetContext(Upfile);
                result.repository.InstallPackage(result.package);

                Upbring upbringFile = Upbring.FromXml();

                upbringFile.AddPackage(result.package);
                upbringFile.SaveFile();
            }
            
            
            
        }

        
    }

    internal void ListPackages()
    {
        foreach(Repository repository in Upfile.Repositories) {
            foreach(Schemas.Upset package in repository.ListPackages()) {
                Debug.Log("Package: " + package.PackageName + " Version: " + package.PackageVersion);
            }
        }
    }

    internal void NukePackages()
    {
        Debug.LogWarning("Nuking all packages!");
        foreach(Repository repository in Upfile.Repositories) {
            repository.SetContext(Upfile);
            repository.NukeAllPackages();
        }  
    }



    //FIXME: Prepare proper version checker
    public void CheckUnityVersion() {
        String upfileVersion = Upfile.UnityVersion;
        String unityVersion = Application.unityVersion; 
        if(unityVersion != upfileVersion) {
            Debug.LogError(String.Format("Uplift: Upfile.xml Unity Version ({0}) doesn't match Unity's one  ({1}).", upfileVersion, unityVersion));
        } else {
            Debug.Log("Upfile: Version check successful");
        }
    }
}