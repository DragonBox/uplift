using System;
using UnityEngine;

namespace Schemas {
    public partial class FileRepository : Repository {
        public override void InstallPackage(DependencyDefinition package) {
            Debug.Log(String.Format("I'm installing package {0}, version: {1}", package.PackageName, package.PackageVersion));
        }
    }
}