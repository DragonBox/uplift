using System;
using System.IO;
using Uplift.Common;

namespace Uplift.Extensions
{
    public static class StringExtension
    {
        public static string MakePathOSFriendly(this string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            return Uplift.Common.FileSystemUtil.JoinPaths(path.Split('/', '\\'));
        }
    }
}
