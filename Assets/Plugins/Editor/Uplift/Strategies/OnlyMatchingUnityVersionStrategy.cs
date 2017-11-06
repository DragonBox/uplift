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

using Uplift.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Uplift.Strategies {
    internal class OnlyMatchingUnityVersionStrategy : CandidateSelectionStrategy {

        Version unityVersion;

        public OnlyMatchingUnityVersionStrategy(string unityVersion) {
            this.unityVersion = VersionParser.ParseVersion(unityVersion, false);
        }

        public override PackageRepo[] Filter(PackageRepo[] candidates) {
            var result = new List<PackageRepo>();

            foreach(PackageRepo item in candidates) {
                // if lack of UnityVersion - continue gracefully but notify
                if(item.Package.UnityVersion == null) {
                    Debug.LogWarning("Package " + item.Package.ToString() + " doesn't have minimal UnityVersion specified!");
                    result.Add(item);
                    continue;
                }

                Version packageUnityRequirement = VersionParser.ParseVersion(item.Package.UnityVersion, false);

                if(unityVersion >= packageUnityRequirement) {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }
    }
}
