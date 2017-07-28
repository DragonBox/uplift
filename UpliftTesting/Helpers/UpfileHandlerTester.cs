using Uplift;
using Uplift.Schemas;

namespace UpliftTesting.Helpers
{
    class UpfileHandlerTester : UpfileHandler
    {
        private static UpfileHandlerTester _testing_instance;

        public void Clear()
        {
            this.Upfile = null;
        }

        public void SetUpfile(Upfile upfile)
        {
            this.Upfile = upfile;
        }

        public Upfile GetUpfile()
        {
            return Upfile;
        }

        public static UpfileHandlerTester TestingInstance()
        {
            return _testing_instance ?? (_testing_instance = new UpfileHandlerTester());
        }

        public new void Initialize()
        {
            base.Initialize();
        }

        public new void CheckUnityVersion()
        {
            // No call to Unity during Testing phase
        }
    }
}
