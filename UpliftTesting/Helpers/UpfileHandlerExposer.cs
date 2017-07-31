using System;
using System.IO;
using System.Xml.Serialization;
using Uplift;
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
                Upfile raw = serializer.Deserialize(fs) as Upfile;
                if (raw.Configuration != null)
                {
                    if (raw.Configuration.BaseInstallPath != null) { raw.Configuration.BaseInstallPath.Location = raw.Configuration.BaseInstallPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.DocsPath != null) { raw.Configuration.DocsPath.Location = raw.Configuration.DocsPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.ExamplesPath != null) { raw.Configuration.ExamplesPath.Location = raw.Configuration.ExamplesPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.MediaPath != null) { raw.Configuration.MediaPath.Location = raw.Configuration.MediaPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.PluginPath != null) { raw.Configuration.PluginPath.Location = raw.Configuration.PluginPath.Location.MakePathOSFriendly(); }
                    if (raw.Configuration.RepositoryPath != null) { raw.Configuration.RepositoryPath.Location = raw.Configuration.RepositoryPath.Location.MakePathOSFriendly(); }
                }

                return raw;
            }
        }

        public override void CheckUnityVersion()
        {
            // Do nothing
            // No call to Unity methods this way
        }
    }
}
