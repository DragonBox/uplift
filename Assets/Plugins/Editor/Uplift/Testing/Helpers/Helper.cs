using System;
using System.IO;

namespace Uplift.Testing.Helpers
{
    class Helper
    {
        public const string testRunDirectoryName = "TestRunningDirectory";

        public static void InitializeRunDirectory()
        {
            if (Directory.Exists(testRunDirectoryName)) Directory.Delete(testRunDirectoryName, true);
            Directory.CreateDirectory(testRunDirectoryName);
        }

        public static void ClearRunDirectory()
        {
            Directory.Delete(testRunDirectoryName, true);
        }

        public static string GetLocalFilePath(string[] path)
        {
            return Path.GetFullPath(string.Join(Path.DirectorySeparatorChar.ToString(), path));
        }
    }
}
