using System;
using UnityEngine;

namespace Schemas {
    public partial class FileRepositoryType : IRepositoryHandler {
        public override void InstallPackage(PackageType package) {
            Debug.Log(String.Format("I'm installing package {0}, version: {1}", package.Name, package.Version));
        }
    }
}