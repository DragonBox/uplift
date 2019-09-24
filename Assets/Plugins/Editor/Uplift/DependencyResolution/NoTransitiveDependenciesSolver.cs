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
using System.Collections.Generic;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
	class NoTransitiveDependenciesSolver : IDependencySolver
	{
		public List<PackageRepo> SolveDependencies(DependencyDefinition[] dependencies)
		{
			List<PackageRepo> packageList = new List<PackageRepo>();

			foreach (DependencyDefinition dd in dependencies)
			{
				Upset package = new Upset();
				package.PackageName = dd.Name;
				package.PackageVersion = dd.Version;
				package.Dependencies = new DependencyDefinition[1]; ;
				package.Dependencies[0] = dd;

				PackageRepo packageRepo = new PackageRepo();
				packageRepo.Package = package;
				packageList.Add(packageRepo);
			}
			return packageList;
		}

		public List<PackageRepo> SolveDependencies(DependencyDefinition[] dependencies, PackageRepo[] StartingPackages)
		{
			throw new NotImplementedException();
		}
	}
}
