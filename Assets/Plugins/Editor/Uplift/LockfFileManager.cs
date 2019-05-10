using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Uplift.Common;
using Uplift.DependencyResolution;
using Uplift.Packages;
using Uplift.Schemas;

namespace Uplift
{
	class LockfileManager
	{
		public static readonly string lockfilePath = "Upfile.lock";

		public struct LockfileSnapshot
		{
			public DependencyDefinition[] upfileDependencies;
			public PackageRepo[] installableDependencies;
		}

		public static void GenerateLockfile(LockfileSnapshot snapshot)
		{
			string result = "# UPFILE DEPENDENCIES\n";
			foreach (DependencyDefinition def in snapshot.upfileDependencies)
			{
				result += string.Format("{0} ({1})\n", def.Name, def.Version);
			}

			result += "\n# SOLVED DEPENDENCIES\n";

			foreach (PackageRepo pr in snapshot.installableDependencies)
			{
				Upset package = pr.Package;
				result += string.Format("{0} ({1})\n", package.PackageName, package.PackageVersion);
				if (package.Dependencies != null && package.Dependencies.Length != 0)
					foreach (DependencyDefinition dependency in package.Dependencies)
					{
						result += string.Format("\t{0} ({1})\n", dependency.Name, dependency.Version);
					}
			}

			using (StreamWriter file = new StreamWriter(lockfilePath, false))
			{
				file.WriteLine(result);
			}

			LockFileTracker.SaveState();
		}

		public static LockfileSnapshot LoadLockfile()
		{
			string pattern = @"([\w\.\-]+)\s\(([\w\.\+!\*]+)\)";
			LockfileSnapshot result = new LockfileSnapshot();

			using (StreamReader file = new StreamReader(lockfilePath))
			{
				string line = file.ReadLine();
				if (line == null || line != "# UPFILE DEPENDENCIES")
					throw new FileLoadException("Cannot load Upfile.lock, it is missing the \'UPFILE DEPENDENCIES\' header");

				List<DependencyDefinition> upfileDependencyList = new List<DependencyDefinition>();

				Match match;
				while (!string.IsNullOrEmpty(line = file.ReadLine()))
				{
					match = Regex.Match(line, pattern);
					if (!match.Success || match.Groups.Count < 3)
						throw new FileLoadException("Cannot load Upfile.lock, the line " + line + " does not match \'package_name (version_requirement)\'");

					DependencyDefinition temp = new DependencyDefinition
					{
						Name = match.Groups[1].Value,
						Version = match.Groups[2].Value
					};
					upfileDependencyList.Add(temp);
				}
				result.upfileDependencies = upfileDependencyList.ToArray();

				if (line == null)
					throw new FileLoadException("Cannot load Upfile.lock, it is incomplete");

				line = file.ReadLine();
				if (line == null || line != "# SOLVED DEPENDENCIES")
					throw new FileLoadException("Cannot load Upfile.lock, it is missing the \'SOLVED DEPENDENCIES\' header");

				List<PackageRepo> installableList = new List<PackageRepo>();
				line = file.ReadLine();
				PackageList packageList = PackageList.Instance();
				while (!string.IsNullOrEmpty(line))
				{
					match = Regex.Match(line, pattern);
					if (!match.Success || match.Groups.Count < 3)
						throw new FileLoadException("Cannot load Upfile.lock, the line " + line + " does not match \'package_name (installed_version)\'");

					PackageRepo temp = packageList.FindPackageAndRepository(new DependencyDefinition
					{
						Name = match.Groups[1].Value,
						Version = match.Groups[2].Value + "!" // Check for exact version
					});

					if (temp.Package != null && temp.Repository != null)
					{
						installableList.Add(temp);
					}
					else
					{
						UnityEngine.Debug.LogError("Could not find a repository while loading lockfile for " + match.Groups[1].Value);
						installableList.Add(new PackageRepo
						{
							Package = new Upset
							{
								PackageName = match.Groups[1].Value,
								PackageVersion = match.Groups[2].Value
							}
						});
					}
					// Read the dependencies
					while ((line = file.ReadLine()) != null && line.StartsWith("\t"))
					{
						match = Regex.Match(line, pattern);
						if (!match.Success || match.Groups.Count < 3)
							throw new FileLoadException("Cannot load Upfile.lock, the line " + line + " does not match \'package_name (version_requirement)\'");
					}
				}
				result.installableDependencies = installableList.ToArray();
			}

			return result;
		}

	}
}