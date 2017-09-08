using System.Xml.Serialization;
using Uplift.Common;

namespace Uplift.Schemas
{
    public partial class DependencyDefinition
    {
        [XmlIgnore]
        public IVersionRequirement Requirement
        {
            get
            {
                return VersionParser.ParseRequirement(Version);
            }
        }
    }
}
