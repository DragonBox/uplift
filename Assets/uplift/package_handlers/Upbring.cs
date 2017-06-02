using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Schemas
{
    public partial class Upbring
    {

        private static string[] upbringPathDefinition = { "Assets", "upackages", "Upbring.xml" };
        protected static string upbringPath
        {
            get
            {
                return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), upbringPathDefinition);
            }
        }
        public static Upbring FromXml()
        {
            if (!File.Exists(upbringPath))
            {
                Upbring newUpbring = new Schemas.Upbring();
                newUpbring.InstalledPackage = new InstalledPackage[0];
                return newUpbring;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Schemas.Upbring));
            FileStream fs = new FileStream(upbringPath, FileMode.Open);
            Upbring upbringFile = serializer.Deserialize(fs) as Schemas.Upbring;
            fs.Close();
            return upbringFile;
        }

        public void SaveFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Schemas.Upbring));
            FileStream fs = new FileStream(upbringPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            serializer.Serialize(sw, this);
            sw.Close();
            fs.Close();
        }

        public static void RemoveFile()
        {
            File.Delete(upbringPath);
        }
        public void RemovePackage()
        {
            throw new NotImplementedException();
        }

        public InstalledPackage GetInstalledPackage(string packageName)
        {
            InstalledPackage spec = this.InstalledPackage.Where(ip => ip.Name == packageName).First();
            return spec;
        }

        internal void AddPackage(Upset package)
        {
            InstalledPackage newPackage = new InstalledPackage();
            newPackage.Name = package.PackageName;
            newPackage.Version = package.PackageVersion;

            //InstallationSpecs newPackageInstallPlace = new InstallationSpecs();
            //newPackageInstallPlace.Kind = Schemas.KindSpec.Base;
            //newPackageInstallPlace.Path = LocalHandler.GetLocalDirectory(package.PackageName, package.PackageVersion);
            //newPackage.Install = new InstallationSpecs[]{newPackageInstallPlace};

            for (var i = 0; i < this.InstalledPackage.Length; i++)
            {
                InstalledPackage ip = this.InstalledPackage[i];
                if (ip.Name == newPackage.Name)
                {
                    return;
                }
            }

            InstalledPackage[] finalArray = new InstalledPackage[this.InstalledPackage.Length + 1];
            this.InstalledPackage.CopyTo(finalArray, 0);
            finalArray[this.InstalledPackage.Length] = newPackage;

            this.InstalledPackage = finalArray;
        }

        internal void AddLocation(Upset package, KindSpec kind, string path)
        {

            InstalledPackage internalPackage = null;
            for (var i = 0; i < this.InstalledPackage.Length; i++)
            {
                internalPackage = this.InstalledPackage[i];
                if (internalPackage.Name == package.PackageName)
                {
                    break;
                }
            }

            // 0 is better than null :)
            if (internalPackage.Install == null)
            {
                internalPackage.Install = new InstallationSpecs[0];
            }
            // Note: not catching in case of internalPackage not found
            // as it is valid error throwing condition



            // Check if path doesn't exist and return if it does
            foreach (InstallationSpecs spec in internalPackage.Install)
            {
                if (spec.Kind == kind && spec.Path == path)
                {
                    // Already have it, returning
                    return;
                }
            }



            // Create new spec
            InstallationSpecs newSpec = new InstallationSpecs();
            newSpec.Kind = kind;
            newSpec.Path = path;

            InstallationSpecs[] newArray = new InstallationSpecs[internalPackage.Install.Length + 1];
            internalPackage.Install.CopyTo(newArray, 0);

            newArray[newArray.Length - 1] = newSpec;

            internalPackage.Install = newArray;

        }


    }
}