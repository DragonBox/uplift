using System.Collections;
using System;

interface IRepositoryHandler {
    Uplift.Schemas.Upset[] ListPackages();
    TemporaryDirectory DownloadPackage(Uplift.Schemas.Upset package);
}