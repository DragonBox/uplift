using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Uplift.Packages;

namespace Uplift.Schemas
{
    public partial class Upbring
    {
        // --- SINGLETON DECLARATION ---
        protected static Upbring instance;

        internal Upbring() { }

        public static Upbring Instance()
        {
            if (instance == null)
            {
                InitializeInstance();
            }

            return instance;
        }

        internal static void InitializeInstance()
        {
            instance = LoadXml();
        }

        // --- CLASS DECLARATION ---
        protected static readonly string upbringFileName = "Upbring.xml";
        protected static string UpbringPath
        {
            get
            {
                string repoPath = Upfile.Instance().GetPackagesRootPath();
                return Path.Combine(repoPath, upbringFileName);
            }
        }

        public static bool CheckForUpbring()
        {
            return File.Exists(UpbringPath);
        }

        internal static Upbring LoadXml()
        {
            if (!File.Exists(UpbringPath))
            {
                Upbring newUpbring = new Upbring {InstalledPackage = new InstalledPackage[0]};
                return newUpbring;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Upbring));
            using(FileStream fs = new FileStream(UpbringPath, FileMode.Open)) {
                return serializer.Deserialize(fs) as Upbring;
            }
        }

        public void SaveFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Upbring));
            using(FileStream fs = new FileStream(UpbringPath, FileMode.Create)) {
                using(StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8)) {
                    serializer.Serialize(sw, this);
                }
            }
        }

        public static void RemoveFile()
        {
            File.Delete(UpbringPath);
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
            if(InstalledPackage == null)
            {
                InstalledPackage = new InstalledPackage[0];
            }

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

        internal void AddLocation(Upset package, InstallSpecType kind, string path)
        {
            InstalledPackage internalPackage;
            if (!SetupInternalPackage(package, out internalPackage)) return;
            // Note: not catching in case of internalPackage not found
            // as it is valid error throwing condition

            // Check if path doesn't exist and return if it does
            if (internalPackage.Install.Any(spec => spec is InstallSpecPath && spec.Type == kind && (spec as InstallSpecPath).Path == path))
            {
                return;
            }

            // Create new spec
            InstallSpec newSpec = new InstallSpecPath {Type = kind, Path = path};
            

            InstallSpec[] newArray = new InstallSpec[internalPackage.Install.Length + 1];
            internalPackage.Install.CopyTo(newArray, 0);

            newArray[newArray.Length - 1] = newSpec;

            internalPackage.Install = newArray;
        }

        internal void AddGUID(Upset package, InstallSpecType kind, string guid)
        {
            InstalledPackage internalPackage;
            if (!SetupInternalPackage(package, out internalPackage)) return;
            // Note: not catching in case of internalPackage not found
            // as it is valid error throwing condition

            // Check if guid doesn't exist and return if it does
            if (internalPackage.Install.Any(spec => spec is InstallSpecGUID && spec.Type == kind && (spec as InstallSpecGUID).Guid == guid))
            {
                return;
            }

            // Create new spec
            InstallSpec newSpec = new InstallSpecGUID { Type = kind, Guid = guid };


            InstallSpec[] newArray = new InstallSpec[internalPackage.Install.Length + 1];
            internalPackage.Install.CopyTo(newArray, 0);

            newArray[newArray.Length - 1] = newSpec;

            internalPackage.Install = newArray;
        }

        private bool SetupInternalPackage(Upset package, out InstalledPackage internalPackage)
        {
            internalPackage = null;

            foreach (InstalledPackage t in InstalledPackage)
            {
                if (t.Name == package.PackageName)
                {
                    internalPackage = t;
                    break;
                }
            }

            // No package has been found
            if (internalPackage == null)
            {
                return false;
            }

            // 0 is better than null :)
            if (internalPackage.Install == null)
            {
                internalPackage.Install = new InstallSpec[0];
            }

            return true;
        }
    }
}