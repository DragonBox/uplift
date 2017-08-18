using System;
using System.IO;

namespace Uplit.Testing.Helpers
{
    class Helper
    {
        public static string GetLocalFilePath(string[] path)
        {
            return string.Join(Path.DirectorySeparatorChar.ToString(), path);
        }
    }
}
