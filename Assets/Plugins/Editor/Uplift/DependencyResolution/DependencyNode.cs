// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

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
