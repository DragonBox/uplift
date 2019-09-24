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