#if UNITY_5_3_OR_NEWER
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

        public static string GetLocalFilePath(params string[] path)
        {
            return Path.GetFullPath(string.Join(Path.DirectorySeparatorChar.ToString(), path));
        }

        public static T SetCurrentDirectory<T>(string NewCurrentDirectory, Func<T> block)
        {
            string currentPath = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(NewCurrentDirectory);
                return block();
            }
            finally
            {
                Directory.SetCurrentDirectory(currentPath);
            }
        }

        public static string PathCombine(params string[] values) {
            return string.Join (Path.DirectorySeparatorChar.ToString (), values);
        }
    }
}
#endif
