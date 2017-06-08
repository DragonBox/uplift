using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Uplift.Schemas
{
    public partial class Upbring
    {

        private static string[] upbringPathDefinition = { "Assets", "Plugins", "Upackages", "Upbring.xml" };
        protected static string upbringPath
        {
            get
            {
                return string.Join(Path.DirectorySeparatorChar.ToString(), upbringPathDefinition);
            }
        }
        public static Upbring FromXml()
        {
            if (!File.Exists(upbringPath))
            {
                Upbring newUpbring = new Upbring {InstalledPackage = new InstalledPackage[0]};
                return newUpbring;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Upbring));
            FileStream fs = new FileStream(upbringPath, FileMode.Open);
            Upbring upbringFile = serializer.Deserialize(fs) as Upbring;
            fs.Close();
            return upbringFile;
        }

        public void SaveFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Upbring));
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
            InstalledPackage spec = InstalledPackage.First(ip => ip.Name == packageName);
            return spec;
        }

        internal void AddPackage(Upset package)
        {
            InstalledPackage newPackage = new InstalledPackage
            {
                Name = package.PackageName,
                Version = package.PackageVersion
            };

            if (InstalledPackage.Any(ip => ip.Name == newPackage.Name))
            {
                return;
            }

            InstalledPackage[] finalArray = new InstalledPackage[InstalledPackage.Length + 1];
            InstalledPackage.CopyTo(finalArray, 0);
            finalArray[InstalledPackage.Length] = newPackage;

            InstalledPackage = finalArray;
        }

        internal void AddLocation(Upset package, KindSpec kind, string path)
        {

            InstalledPackage internalPackage = null;
            
            foreach (InstalledPackage t in InstalledPackage)
            {
                internalPackage = t;
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
            if (internalPackage.Install.Any(spec => spec.Kind == kind && spec.Path == path))
            {
                return;
            }



            // Create new spec
            InstallationSpecs newSpec = new InstallationSpecs {Kind = kind,Path = path};
            

            InstallationSpecs[] newArray = new InstallationSpecs[internalPackage.Install.Length + 1];
            internalPackage.Install.CopyTo(newArray, 0);

            newArray[newArray.Length - 1] = newSpec;

            internalPackage.Install = newArray;

        }


    }
}