using System.Collections;
using System;

interface IRepositoryHandler {
    void InstallPackage(Schemas.Upset package);
    void UpdatePackage(Schemas.Upset package);
    void UninstallPackage(Schemas.Upset package);
    void NukePackage(Schemas.Upset package);
    void NukeAllPackages();

    Schemas.Upset[] ListPackages();
}