using System;
using System.Text.RegularExpressions;

namespace Uplift.Schemas
{
    public partial class Upset
    {
        public struct Meta
        {
            public string dirName;
        }

        public Meta MetaInformation;

        public int PackageVersionAsNumber()
        {
            PackageHandler.VersionStruct version =  PackageHandler.ParseVersion(this.PackageVersion);
            return version.Major * 1000000 + version.Minor * 1000 + version.Version;
        }

    }
}