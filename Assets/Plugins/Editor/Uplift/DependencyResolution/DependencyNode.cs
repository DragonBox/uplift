using System.Collections.Generic;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    public class DependencyNode
    {
        protected IVersionRequirement requirement;
        protected string repository;
        protected string name;
        protected List<DependencyNode> dependencies;
        protected int index;
        protected int lowlink;
        public SkipInstallSpec[] skips;
        public OverrideDestinationSpec[] overrides;

        public DependencyNode() { }
        public DependencyNode(DependencyDefinition definition) : this(
                definition.Name,
                definition.Version,
                definition.Repository,
                definition.SkipInstall,
                definition.OverrideDestination,
                null
                ) { }
        public DependencyNode(string name, string version, string repository) : this(name, version, repository, null, null, null) { }
        public DependencyNode(string name, string version, string repository, SkipInstallSpec[] skips, OverrideDestinationSpec[] overrides, List<DependencyNode> dependencies)
        {
            this.name = name;
            this.requirement = VersionParser.ParseRequirement(version);
            this.repository = repository;
            this.dependencies = dependencies;
            this.skips = skips;
            this.overrides = overrides;

            index = -1;
            lowlink = -1;
        }

        public IVersionRequirement Requirement
        {
            get
            {
                return requirement;
            }
            set
            {
                requirement = value;
            }
        }

        public string Repository
        {
            get
            {
                return repository;
            }
            set
            {
                repository = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public List<DependencyNode> Dependencies
        {
            get
            {
                if (dependencies == null)
                    dependencies = new List<DependencyNode>();

                return dependencies;
            }
            set
            {
                dependencies = value;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        public int Lowlink
        {
            get
            {
                return lowlink;
            }
            set
            {
                lowlink = value;
            }
        }
    }
}

