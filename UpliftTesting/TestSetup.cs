using NUnit.Framework;

namespace UpliftTesting
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        protected void BeforeAllTests()
        {
            Uplift.TestingProperties.SetLogging(false);
        }
    }
}
