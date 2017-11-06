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
using System.Collections.Generic;
using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift.Testing.Helpers
{
    public class PackageListTester : PackageList
    {
        private static PackageListTester _testing_instance;

        public static PackageListTester TestingInstance()
        {
            return _testing_instance ?? (_testing_instance = new PackageListTester());
        }

        public void Clear()
        {
            this.Repositories = new Repository[0];
            this.Packages.Clear();
        }

        public void SetRepositories(Repository[] _repositories)
        {
            this.Repositories = _repositories;
        }

        public Repository[] GetRepositories()
        {
            return Repositories;
        }

        public void SetPackages(List<PackageRepo> _packages)
        {
            this.Packages = _packages;
        }

        public List<PackageRepo> GetPackages()
        {
            return Packages;
        }
    }
}
#endif
