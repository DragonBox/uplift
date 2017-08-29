using System.Collections.Generic;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    public class DependencyNode
    {
        protected string version;
        protected string repository;
        protected string name;
        protected List<DependencyNode> dependencies;
        protected int index;
        protected int lowlink;

        public DependencyNode() { }
        public DependencyNode(string name, string version, string repository) : this(name, version, repository, null) { }
        public DependencyNode(string name, string version, string repository, List<DependencyNode> dependencies)
        {
            this.name = name;
            this.version = version;
            this.repository = repository;
            this.dependencies = dependencies;

            index = -1;
            lowlink = -1;
        }

        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
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

