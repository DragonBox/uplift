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

        // int has 32 bits
        // Sign  | Major      | Minor   | Build   | Revision
        // x     | xxxxxxxxxx | xxxxxxx | xxxxxxx | xxxxxxx
        // 1 bit | 10 bits    | 7 bits  | 7 bits  | 7 bits
        public int PackageVersionAsNumber()
        {
            VersionParser.VersionStruct version = VersionParser.ParseVersion(PackageVersion);
            return ((version.Major & 2047) << 21) +
                ((version.Minor & 255) << 14) +
                ((version.Build & 255) << 7) +
                (version.Revision & 255);
        }
    }
}