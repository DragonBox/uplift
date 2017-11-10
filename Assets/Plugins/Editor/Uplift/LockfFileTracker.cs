using System;

namespace Uplift
{
    public class LockFileTracker
    {
        private static readonly string envVariable = "UPFILE_LOCK_MD5";

        public static bool HasChanged()
        {
            string currentMD5 = Uplift.Common.FileSystemUtil.GetFileMD5(UpliftManager.lockfilePath);
            string oldMD5 = Environment.GetEnvironmentVariable(envVariable);

            return !string.Equals(currentMD5, oldMD5);
        }

        public static void SaveState()
        {
            Environment.SetEnvironmentVariable(envVariable, Uplift.Common.FileSystemUtil.GetFileMD5(UpliftManager.lockfilePath));
        }
    }
}