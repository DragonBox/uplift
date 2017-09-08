using Uplift.Common;

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
            VersionStruct version =  VersionParser.ParseVersion(PackageVersion);
            int result = version.Major * 1000000;
            if (version.Minor != null) result += ((int)version.Minor) * 1000;
            if (version.Patch != null) result += (int)version.Patch;
            return result;
        }

    }
}