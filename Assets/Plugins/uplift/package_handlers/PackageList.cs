using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackageList {

    // Singleton implementation
    private PackageList() {
        packages = new List<PackageRepo>();
    }

    private static PackageList instance;

    public static PackageList Instance() {
        if(instance == null) { 
            instance = new PackageList();
        }
        return instance;
    }

    // End of Singleton implementation
    protected List<PackageRepo> packages;
    protected Schemas.Repository[] repositories;

    public void LoadPackages(Schemas.Repository[] repositories) {
        if(this.repositories == null) {
            this.repositories = repositories;
        }
        
        foreach(var repo in repositories) {
            this.LoadPackages(repo);
        }
    }

    public void LoadPackages(Schemas.Repository[] repositories, bool refresh = false) {
        if(refresh) {
            packages.Clear();
        }
        LoadPackages(repositories);
    }

    public void RefreshPackages() {
        packages.Clear();
        LoadPackages(repositories);
    }

    public void LoadPackages(Schemas.Repository repository) {
        PackageRepo pr;
        foreach(var package in repository.ListPackages()) {
            pr = new PackageRepo();
            pr.Package = package;
            pr.Repository = repository;

            packages.Add(pr);

        }
        Debug.Log("Loaded packages, count: " + packages.Count.ToString());
    }

    public PackageRepo[] GetAllPackages()
    {
        return packages.ToArray();
    }

    public PackageRepo GetLatestPackage(string packageName) {
        CandidateSelectionStrategy css = new LatestSelectionStrategy();
        PackageRepo[] packages = this.packages.Where(pr => pr.Package.PackageName == packageName).ToArray();

        return css.Filter(packages)[0];
    }
    

}