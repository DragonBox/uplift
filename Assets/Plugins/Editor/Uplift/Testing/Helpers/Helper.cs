// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

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
