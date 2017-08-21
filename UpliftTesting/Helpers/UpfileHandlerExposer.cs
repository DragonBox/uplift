using System;
using System.IO;
using System.Xml.Serialization;
using Uplift;
using Uplift.Common;
using Uplift.Extensions;
using Uplift.Schemas;

namespace UpliftTesting.Helpers
{
    class UpfileHandlerExposer : UpfileHandler
    {
        public static new UpfileHandler Instance()
        {
            if (instance != null) return instance;

            UpfileHandler uph = new UpfileHandlerExposer();
            instance = uph;
            uph.Initialize();
            return uph;
        }

        public string test_upfile_path = "";

        public static void ResetSingleton()
        {
            instance = null;
        }

        public void Clear()
        {
            this.Upfile = null;
        }

        public void SetUpfile(Upfile upfile)
        {
            this.Upfile = upfile;
        }

        public Upfile GetUpfile()
        {
            return Upfile;
        }

        public override bool CheckForUpfile()
        {
            return File.Exists(test_upfile_path);
        }

        public override Upfile LoadFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Upfile));

            using (FileStream fs = new FileStream(test_upfile_path, FileMode.Open))
            {
                Upfile upfile = serializer.Deserialize(fs) as Upfile;

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

        public override void CheckUnityVersion()
        {
            // Do nothing
            // No call to Unity methods this way
        }
    }
}
