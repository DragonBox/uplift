using System.Collections.Generic;
using Uplift.Common;

namespace Uplift.Schemas
{
    public partial class Upfile
    {
        public void MakePathConfigurationsOSFriendly()
        {
            foreach(PathConfiguration path in PathConfigurations())
            {
                path.Location = FileSystemUtil.MakePathOSFriendly(path.Location);
            }
        }

        public IEnumerable<PathConfiguration> PathConfigurations()
        {
            if (Configuration == null) yield break;
            yield return Configuration.BaseInstallPath;
            yield return Configuration.DocsPath;
            yield return Configuration.EditorPluginPath;
            yield return Configuration.ExamplesPath;
            yield return Configuration.MediaPath;
            yield return Configuration.PluginPath;
            yield return Configuration.RepositoryPath;
        }
    }
}