using System.Xml.Serialization;
using Uplift.Common;

namespace Uplift.Schemas
{
    public partial class DependencyDefinition
    {
        [XmlIgnore]
        private IVersionRequirement requirement;
        [XmlIgnore]
        public IVersionRequirement Requirement
        {
            get
            {
                if (requirement == null) requirement = VersionParser.ParseRequirement(this.Version);
                return requirement;
            }
            set
            {
                requirement = value;
            }
        }
    }
}
