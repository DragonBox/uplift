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
using System.Text;
using Uplift.Common;
using Uplift.Schemas;
using Uplift.Packages;
using System.Linq;

namespace Uplift.DependencyResolution
{
	class PackageListStub : PackageList
	{
		Dictionary<string, Upset[]> dicoPackages;

		public PackageListStub(Dictionary<string, Upset[]> dicoPackages)
		{
			this.dicoPackages = dicoPackages;
			this.Packages = GetAllPackageRepo();
		}

		public List<Upset> GetPackages(string requirementName)
		{
			List<Upset> listPackages = new List<Upset>();
			//Check if package is in repo
			if (dicoPackages[requirementName] != null)
			{
				foreach (Upset pkg in dicoPackages[requirementName])
				{
					listPackages.Add(pkg);
				}
			}
			return listPackages;
		}

		public List<PackageRepo> GetAllPackageRepo()
		{
			List<PackageRepo> listPackageRepo = new List<PackageRepo>();
			List<Upset> listUpset = new List<Upset>();

			foreach (Upset[] listValues in dicoPackages.Values)
			{
				listUpset.AddRange(listValues);
			}

			foreach (Upset pkg in listUpset)
			{
				PackageRepo correspondingRepo = new PackageRepo();
				correspondingRepo.Package = pkg;
				correspondingRepo.Repository = new GithubRepository();

				listPackageRepo.Add(correspondingRepo);
			}
			return listPackageRepo;
		}


		override public string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Packages in repo : ");
			foreach (string pkgName in dicoPackages.Keys)
			{
				foreach (Upset pkg in dicoPackages[pkgName])
				{
					sb.Append("* ");
					sb.Append(pkg.PackageName);
					sb.Append(" ");
					sb.AppendLine(pkg.PackageVersion);
					sb.Append("  |_ dep : ");
					if (pkg.Dependencies != null)
					{
						foreach (DependencyDefinition dep in pkg.Dependencies)
						{
							if (dep != null)
							{
								sb.Append(dep.Name);
								sb.Append("_");
								sb.Append(dep.Version);
								sb.Append("  ");
							}
						}
					}
					else
					{
						sb.Append("None");
					}
					sb.AppendLine();
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}