using NUnit.Framework;
using System.IO;
using Uplift.Schemas;

namespace UpliftTesting.UnitTesting
{
    [TestFixture]
    class InstalledPackagesTest
    {
        [Test]
        public void NukeRootInstallTest()
        {
            InstalledPackage ip = new InstalledPackage();
            string installed_path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(installed_path);

            ip.Install = new InstallSpec[]
            {
                new InstallSpec()
                {
                    Type = InstallSpecType.Root,
                    Path = installed_path
                }
            };

            ip.Nuke();

            Assert.IsFalse(Directory.Exists(installed_path));
            // Make sure that the directory is deleted even if the test fails
            if(Directory.Exists(installed_path)) { Directory.Delete(installed_path); }            
        }
    }
}
