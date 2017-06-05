using System.Collections;
using System;

interface IRepositoryHandler {
    Schemas.Upset[] ListPackages();
    TemporaryDirectory DownloadPackage(Schemas.Upset package);
}