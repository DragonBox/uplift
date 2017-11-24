using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Uplift.Common;

namespace Uplift.Schemas
{
    public partial class DotUplift
    {
        public static readonly string defaultName = ".Uplift.xml";

        internal DotUplift() {}

        public static DotUplift FromDefaultFile()
        {
            return FromFile(defaultName);
        }
        
        public static DotUplift FromFile(string name)
        {
            string source = System.IO.Path.Combine(GetHomePath(), name);

            StrictXmlDeserializer<DotUplift> deserializer = new StrictXmlDeserializer<DotUplift>();

            DotUplift result = new DotUplift { Repositories = new Repository[0], AuthenticationMethods = new RepositoryAuthentication[0] };
            using (FileStream fs = new FileStream(source, FileMode.Open))
            {
                try
                {
                    result = deserializer.Deserialize(fs);

                    foreach (Repository repo in result.Repositories)
                    {
                        if (repo is FileRepository)
                            (repo as FileRepository).Path = Uplift.Common.FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
                    }
                }
                catch (InvalidOperationException)
                {
                    Debug.LogError(string.Format("Global Override file at {0} is not well formed", source));
                }

                return result;
            }
        }

        public static string GetHomePath()
        {
            return (Environment.OSVersion.Platform == PlatformID.Unix ||
                               Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }


    }
}