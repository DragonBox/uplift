using Uplift.Schemas;
using NUnit.Framework;
using Moq;

namespace UpliftTesting.UnitTesting
{
    [TestFixture]
    public class GetterSetterTest
    {
        [Test]
        public void ConfigurationPropertiesTest()
        {
            Configuration config = new Configuration();
            Mock<PathConfiguration> path_dummy = new Mock<PathConfiguration>();
            config.BaseInstallPath = path_dummy.Object;
            config.DocsPath = path_dummy.Object;
            config.ExamplesPath = path_dummy.Object;
            config.MediaPath = path_dummy.Object;
            config.PluginPath = path_dummy.Object;
            config.RepositoryPath = path_dummy.Object;
            Assert.AreEqual(config.BaseInstallPath, path_dummy.Object);
            Assert.AreEqual(config.DocsPath, path_dummy.Object);
            Assert.AreEqual(config.ExamplesPath, path_dummy.Object);
            Assert.AreEqual(config.MediaPath, path_dummy.Object);
            Assert.AreEqual(config.PluginPath, path_dummy.Object);
            Assert.AreEqual(config.RepositoryPath, path_dummy.Object);
        }

        [Test]
        public void DependencyDefinitionPropertiesTest()
        {
            DependencyDefinition dependency = new DependencyDefinition();
            Mock<VersionSpec> version_spec_dummy = new Mock<VersionSpec>();
            string string_dummy = "I have no purpose";
            dependency.Item = version_spec_dummy.Object;
            dependency.Name = string_dummy;
            dependency.Repository = string_dummy;
            dependency.Version = string_dummy;
            Assert.AreEqual(dependency.Item, version_spec_dummy.Object);
            Assert.AreEqual(dependency.Name, string_dummy);
            Assert.AreEqual(dependency.Repository, string_dummy);
            Assert.AreEqual(dependency.Version, string_dummy);
        }
    }
}