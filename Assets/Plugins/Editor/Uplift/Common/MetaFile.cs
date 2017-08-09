using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Uplift.Common
{
    class MetaFile
    {
        public string FileFormatVersion { get; set; }
        public string Guid { get; set; }
        public string FolderAsset { get; set; }
        public int TimeCreated { get; set; }
        public string LicenseType { get; set; }

        public static MetaFile FromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            DeserializerBuilder builder = new DeserializerBuilder();
            builder.WithNamingConvention(new CamelCaseNamingConvention());
            Deserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();

            using (StreamReader sr = File.OpenText(path))
            {
                return deserializer.Deserialize<MetaFile>(sr);
            }
        }
    }

    class Importer
    {

    }
}
