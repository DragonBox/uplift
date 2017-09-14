using System.IO;
using System.Xml.Serialization;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.Testing.Helpers
{
    public class UpfileExposer : Upfile
    {
        public static UpfileExposer TestingInstance()
        {
            if (instance == null)
            {
                InitializeTestingInstance();
            }

            return (instance as UpfileExposer);
        }

        public static void ClearInstance()
        {
            instance = null;
        }

        internal static void InitializeTestingInstance()
        {
            instance = new UpfileExposer();
        }

        internal static void SetInstance(Upfile upfile)
        {
            instance = upfile;
            instance.LoadPackageList();
        }

        internal static Upfile LoadTestXml(string path)
        {
            StrictXmlDeserializer<Upfile> deserializer = new StrictXmlDeserializer<Upfile>();

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                Upfile upfile = deserializer.Deserialize(fs);

                upfile.MakePathConfigurationsOSFriendly();

                if (upfile.Repositories != null)
                {
                    foreach (Repository repo in upfile.Repositories)
                    {
                        if (repo is FileRepository)
                        {
                            (repo as FileRepository).Path = FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
                        }
                    }
                }

                return upfile;
            }
        }

        public override void LoadOverrides()
        {
            return;
        }
    }
}
