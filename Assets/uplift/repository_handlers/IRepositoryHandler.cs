using System.Collections;
using System;

interface IRepositoryHandler {
    void InstallPackage(Schemas.DependencyDefinition package);
    void UpdatePackage(Schemas.DependencyDefinition package);
    void UninstallPackage(Schemas.DependencyDefinition package);
    void NukePackage(Schemas.DependencyDefinition package);
    void NukeAllPackages();

    Schemas.DependencyDefinition[] ListPackages();
}