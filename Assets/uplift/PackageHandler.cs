using System;
using Schemas;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
class PackageHandler
{
    public struct PackageRepo
    {
        public Schemas.Upset package;
        public Schemas.Repository repository;
    }

    internal PackageRepo FindPackageAndRepository(DependencyDefinition packageDefinition, Repository[] repositories)
    {
        PackageRepo result = new PackageRepo();


        foreach (var repo in repositories)
        {
            Upset[] repoPackages = repo.ListPackages();

            foreach (var package in repoPackages)
            {
                
                if (package.PackageName != packageDefinition.Name)
                {
                    continue;
                }

                //FIXME: Better version comparision
                if (package.PackageVersion != packageDefinition.Version)
                {
                    continue;
                }


                //FIXME: Try to find better candidates later on

                result.repository = repo;
                result.package = package;
                return result;

            }
        }

        Debug.LogWarning("Package: " + packageDefinition.Name + " not found");
        return result;


    }
}