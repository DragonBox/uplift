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
            VersionStruct version = VersionParser.ParseIncompleteVersion(PackageVersion);
            return 0;
        }
    }
}