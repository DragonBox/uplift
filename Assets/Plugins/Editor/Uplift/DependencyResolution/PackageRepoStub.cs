using System;
using System.Collections.Generic;
using System.Text;
using Uplift.Common;
using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
	class PackageRepoStub
	{
		Dictionary<string, Upset[]> packageRepo;

		public PackageRepoStub(Dictionary<string, Upset[]> packageRepo)
		{
			this.packageRepo = packageRepo;
		}

		public List<string> GetVersions(string requirementName)
		{
			List<string> listVersions = new List<string>();
			foreach (Upset pkg in packageRepo[requirementName])
			{
				listVersions.Add(pkg.PackageVersion);
			}
			return listVersions;
		}

		public List<Upset> GetPackages(string requirementName)
		{
			List<Upset> listPackages = new List<Upset>();
			//Check if package is in repo
			if (packageRepo[requirementName] != null)
			{
				foreach (Upset pkg in packageRepo[requirementName])
				{
					listPackages.Add(pkg);
				}
			}
			return listPackages;
		}

		override public string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Packages in repo : ");
			foreach (string pkgName in packageRepo.Keys)
			{
				foreach (Upset pkg in packageRepo[pkgName])
				{
					sb.AppendLine("* " + pkg.PackageName + " " + pkg.PackageVersion);
					sb.Append("  |_ dep : ");
					if (pkg.Dependencies != null)
					{
						foreach (DependencyDefinition dep in pkg.Dependencies)
						{
							if (dep != null)
							{
								sb.Append(dep.Name + "_" + dep.Version + "  ");
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