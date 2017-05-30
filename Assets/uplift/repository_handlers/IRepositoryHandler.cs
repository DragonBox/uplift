using System.Collections;
using System;

interface IRepositoryHandler {
    Schemas.Upset[] ListPackages();
    void InstallPackage(Schemas.Upset package);
}