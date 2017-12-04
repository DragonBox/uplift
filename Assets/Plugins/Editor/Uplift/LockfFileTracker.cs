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

using System;

namespace Uplift
{
    public class LockFileTracker
    {
        private static readonly string envVariable = "UPFILE_LOCK_MD5";

        public static bool HasChanged()
        {
            if(!System.IO.File.Exists(UpliftManager.lockfilePath)) return false;
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