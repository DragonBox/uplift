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

using UnityEditor;
using UnityEngine;
using Uplift.Common;
using Uplift.Schemas;
using System;

namespace Uplift
{
    [InitializeOnLoad]
    public class Initialize : MonoBehaviour
    {
        private static readonly string env_variable = "UPLIFT_INSTALLATION_DONE";
        static Initialize()
        {
            Debug.Log("Upfile loading...");
            if (!Upfile.CheckForUpfile())
            {
                Debug.Log("No Upfile was found at the root of your project, Uplift created a sample one for you to start working on");
                SampleFile.CreateSampleUpfile();
            }
            
            if(!IsInitialized())
            {
                UpliftManager.Instance().InstallDependencies(strategy: UpliftManager.InstallStrategy.INCOMPLETE_LOCKFILE, refresh: true);
                MarkAsInitialized();
            }
        }

        private static bool IsInitialized()
        {
            return string.Equals(Environment.GetEnvironmentVariable(env_variable), "true");
        }

        private static void MarkAsInitialized()
        {
            Environment.SetEnvironmentVariable(env_variable, "true");
        }
    }
}
