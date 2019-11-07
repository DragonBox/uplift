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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using Uplift.Common;
using Uplift.DependencyResolution;
using Uplift.Packages;
using Uplift.Schemas;
using Uplift.SourceControl;

namespace Uplift
{
	class UpliftManager
	{
		protected LogHandler logHandler;
		private UpliftPreferences preferences;

		// --- SINGLETON DECLARATION ---
		protected static UpliftManager instance;

		internal UpliftManager()
		{

		}

		public static UpliftManager Instance()
		{
			if (instance == null)
			{
				InitializeInstance();
			}

			return instance;
		}

		public static void ResetInstances()
		{
			instance = null;
			Upfile.ResetInstance();
			Upbring.ResetInstance();
			InitializeInstance();
		}

		internal static void InitializeInstance()
		{
			instance = new UpliftManager();
			instance.upfile = Upfile.Instance();
			instance.preferences = UpliftPreferences.FromDefaultFile();
		}

		// --- CLASS DECLARATION ---
		protected Upfile upfile;

		public enum InstallStrategy
		{
			ONLY_LOCKFILE,
			INCOMPLETE_LOCKFILE,
			UPDATE_LOCKFILE
		}

		public struct DependencyState
		{
			public DependencyDefinition definition;
			public InstalledPackage installed;
			public PackageRepo bestMatch, latest;
			public bool transitive;
		}

		public void InstallDependencies(InstallStrategy strategy = InstallStrategy.UPDATE_LOCKFILE)
		{
			using (LogHandler logHandler = new LogHandler(
				appendToCurrentLogFile: false,
				showStack: false,
				logType: preferences.Verbose ? LogType.Log : LogType.Warning))
			{
				Debug.Log("Install Dependencies with strategy " + strategy);
				PackageRepo[] targets = GetTargets(GetDependencySolver(), strategy);

				bool updateLockfile = (strategy != InstallStrategy.ONLY_LOCKFILE);
				InstallPackages(targets, updateLockfile);
			}
		}

		public DependencyState[] GetDependenciesState()
		{
			Upbring upbring = Upbring.Instance();
			PackageRepo[] targets = GetTargets(GetDependencySolver(),
											   InstallStrategy.UPDATE_LOCKFILE,
											   updateLockfile: false);

			bool anyInstalled =
						upbring.InstalledPackage != null &&
						upbring.InstalledPackage.Length != 0;

			List<DependencyState> dependenciesState = new List<DependencyState>();
			foreach (DependencyDefinition definition in upfile.Dependencies)
				AppendDependencyState(
					ref dependenciesState,
					definition,
					targets,
					anyInstalled
				);
			return dependenciesState.ToArray();
		}

		private void AppendDependencyState(
			ref List<DependencyState> dependenciesState,
			DependencyDefinition definition,
			PackageRepo[] targets,
			bool anyInstalled,
			bool transitive = false
		)
		{
			Upbring upbring = Upbring.Instance();

			DependencyState state = new DependencyState
			{
				definition = definition,
				latest = PackageList.Instance().GetLatestPackage(definition.Name)
			};

			state.bestMatch = targets.First(pr => pr.Package.PackageName == definition.Name);
			if (anyInstalled && upbring.InstalledPackage.Any(ip => ip.Name == definition.Name))
			{
				state.installed = upbring.InstalledPackage.First(ip => ip.Name == definition.Name);
			}
			state.transitive = transitive;

			dependenciesState.Add(state);
			if (state.bestMatch.Package.Dependencies != null)
				foreach (DependencyDefinition dependency in state.bestMatch.Package.Dependencies)
					AppendDependencyState(
						ref dependenciesState,
						dependency,
						targets,
						anyInstalled,
						true
					);
		}

		private PackageRepo[] GetTargets(IDependencySolver solver, InstallStrategy strategy, bool updateLockfile = true)
		{
			Debug.Log("Get Targets");
			DependencyDefinition[] upfileDependencies = upfile.Dependencies;
			PackageRepo[] targets = new PackageRepo[0];
			bool present = File.Exists(LockfileManager.lockfilePath);

			if (strategy == InstallStrategy.UPDATE_LOCKFILE || (strategy == InstallStrategy.INCOMPLETE_LOCKFILE && !present))
			{
				PackageRepo[] installableDependencies = solver.SolveDependencies(upfileDependencies).ToArray();

				if (updateLockfile)
				{
					Debug.Log(">Update Lockfile");
					LockfileManager.GenerateLockfile(new LockfileManager.LockfileSnapshot
					{
						upfileDependencies = upfileDependencies,
						installableDependencies = installableDependencies
					});
				}
				Debug.Log("Targets = installableDependencies");
				targets = installableDependencies;
			}
			else if (strategy == InstallStrategy.INCOMPLETE_LOCKFILE)
			{
				PackageRepo[] installableDependencies = solver.SolveDependencies(upfileDependencies).ToArray();

				// Case where the file does not exist is already covered
				Debug.Log("Load Lockfile Lockfile");
				LockfileManager.LockfileSnapshot snapshot = LockfileManager.LoadLockfile();

				DependencyDefinition[] modifiedDependencies =
					upfileDependencies
					.Where(def => !snapshot.upfileDependencies.Any(
						d => d.Name == def.Name && d.Version == def.Version
						))
					.ToArray();

				if (modifiedDependencies.Length == 0)
				{
					Debug.Log("No modified dependencies");
					Debug.Log("Targets = installableDependencies");
					targets = installableDependencies;
				}
				else
				{
					Debug.Log("Found modified dependencies");
					// Fetch all the PackageRepo for the unmodified packages
					List<PackageRepo> unmodifiableList = new List<PackageRepo>();
					Queue<PackageRepo> scanningQueue = new Queue<PackageRepo>();
					foreach (DependencyDefinition def in snapshot.upfileDependencies.Where(directDepency => !modifiedDependencies.Any(d => d.Name == directDepency.Name)))
						scanningQueue.Enqueue(snapshot.installableDependencies.First(pr => pr.Package.PackageName == def.Name));

					while (scanningQueue.Count != 0)
					{
						PackageRepo top = scanningQueue.Dequeue();
						unmodifiableList.Add(top);
						if (top.Package.Dependencies != null)
							foreach (DependencyDefinition def in top.Package.Dependencies)
								scanningQueue.Enqueue(snapshot.installableDependencies.First(pr => pr.Package.PackageName == def.Name));
					}
					PackageRepo[] unmodifiable =
						snapshot.installableDependencies
						.Where(pr => unmodifiableList.Any(unmodpr =>
							unmodpr.Package.PackageName == pr.Package.PackageName &&
							unmodpr.Package.PackageVersion == pr.Package.PackageVersion
							))
						.ToArray();

					PackageRepo[] installableModified = solver.SolveDependencies(modifiedDependencies, unmodifiable).ToArray();

					targets = new PackageRepo[unmodifiable.Length + installableModified.Length];
					Array.Copy(unmodifiable, targets, unmodifiable.Length);
					Array.Copy(installableModified, 0, targets, unmodifiable.Length, installableModified.Length);
					Debug.Log("Targets = installable dependencies with solved dependencies + unmodifiable dependencies");
				}

				if (updateLockfile)
				{
					Debug.Log("Update lockfile according to targets");
					LockfileManager.GenerateLockfile(new LockfileManager.LockfileSnapshot
					{
						upfileDependencies = upfileDependencies,
						installableDependencies = targets
					});
				}
			}
			else if (strategy == InstallStrategy.ONLY_LOCKFILE)
			{
				if (!present)
					throw new ApplicationException("Uplift cannot install dependencies in strategy ONLY_LOCKFILE if there is no lockfile");

				Debug.Log("targets = installable dependencies in logfile");
				targets = LockfileManager.LoadLockfile().installableDependencies;
			}
			else
			{
				throw new ArgumentException("Unknown install strategy: " + strategy);
			}

			foreach (var item in targets)
			{
				Debug.Log("| " + item.Package.PackageName + " " + item.Package.PackageVersion);
			}
			return targets;
		}

		private PackageRepo[] IdentifyInstallable(DependencyDefinition[] definitions)
		{
			PackageRepo[] result = new PackageRepo[definitions.Length];
			for (int i = 0; i < definitions.Length; i++)
				result[i] = PackageList.Instance().FindPackageAndRepository(definitions[i]);

			return result;
		}

		public IDependencySolver GetDependencySolver()
		{
			Resolver dependencySolver = new Resolver(PackageList.Instance());
			return dependencySolver;
		}

		private void SolveVersionConflict(ref DependencyNode existing, DependencyNode compared)
		{
			IVersionRequirement restricted;
			try
			{
				restricted = existing.Requirement.RestrictTo(compared.Requirement);
			}
			catch (IncompatibleRequirementException e)
			{
				UnityEngine.Debug.LogError("Unsolvable version conflict in the dependency graph");
				throw new IncompatibleRequirementException("Some dependencies " + existing.Name + " are not compatible.\n", e);
			}

			existing.Requirement = restricted;
		}

		public void InstallPackages(PackageRepo[] targets, bool updateLockfile = true)
		{
			Debug.Log("Install Packages");
			using (LogAggregator LA = LogAggregator.InUnity(
				"Successfully installed dependencies ({0} actions were done)",
				"Successfully installed dependencies ({0} actions were done) but warnings were raised",
				"Some errors occured while installing dependencies",
				targets.Length
			))
			{
				// Remove installed dependencies that are no longer in the dependency tree
				foreach (InstalledPackage ip in Upbring.Instance().InstalledPackage)
				{
					if (targets.Any(tar => tar.Package.PackageName == ip.Name)) continue;

					UnityEngine.Debug.Log("Removing unused dependency on " + ip.Name);
					NukePackage(ip.Name);
				}

				foreach (PackageRepo pr in targets)
				{
					if (pr.Repository != null)
					{
						if (Upbring.Instance().InstalledPackage.Any(ip => ip.Name == pr.Package.PackageName))
						{
							Debug.Log("update " + pr.Package.PackageName);
							UpdatePackage(pr, updateDependencies: false, updateLockfile: updateLockfile);
						}
						else
						{
							DependencyDefinition def = upfile.Dependencies.Any(d => d.Name == pr.Package.PackageName) ?
								upfile.Dependencies.First(d => d.Name == pr.Package.PackageName) :
								new DependencyDefinition() { Name = pr.Package.PackageName, Version = pr.Package.PackageVersion };

							using (TemporaryDirectory td = pr.Repository.DownloadPackage(pr.Package))
							{
								Debug.Log("install " + pr.Package.PackageName);
								InstallPackage(pr.Package, td, def, updateLockfile);
							}
						}
					}
					else
					{
						UnityEngine.Debug.LogError("No repository for package " + pr.Package.PackageName);
					}
				}
			}
			UnityHacks.BuildSettingsEnforcer.EnforceAssetSave();
		}

		public void NukeAllPackages()
		{
			Upbring upbring = Upbring.Instance();
			using (LogAggregator LA = LogAggregator.InUnity(
				"{0} packages were successfully nuked",
				"{0} packages were successfully nuked but warnings were raised",
				"Some errors occured while nuking {0} packages",
				upbring.InstalledPackage.Length
			))
			{
				foreach (InstalledPackage package in upbring.InstalledPackage)
				{
					package.Nuke();
					upbring.RemovePackage(package);
				}

				//TODO: Remove file when Upbring properly removes everything
				Upbring.RemoveFile();
			}
		}

		public string GetPackageDirectory(Upset package)
		{
			return package.PackageName + "~" + package.PackageVersion;
		}

		public string GetRepositoryInstallPath(Upset package)
		{
			return Path.Combine(upfile.GetPackagesRootPath(), GetPackageDirectory(package));
		}

		//FIXME: This is super unsafe right now, as we can copy down into the FS.
		// This should be contained using kinds of destinations.
		private void InstallPackage(Upset package, TemporaryDirectory td, DependencyDefinition dependencyDefinition, bool updateLockfile)
		{
			Debug.Log("Installing package " + package.PackageName + " " + package.PackageVersion);

			if (dependencyDefinition == null)
			{
				throw new ArgumentNullException("Failed to install package " + package.PackageName + ". Dependency Definition is null.");
			}

			GitIgnorer VCSHandler = new GitIgnorer();

			using (LogAggregator LA = LogAggregator.InUnity(
				"Package {0} was successfully installed",
				"Package {0} was successfully installed but raised warnings",
				"An error occured while installing package {0}",
				package.PackageName
			))
			{
				Upbring upbring = Upbring.Instance();

				// Note: Full package is ALWAYS copied to the upackages directory right now
				string localPackagePath = GetRepositoryInstallPath(package);
				upbring.AddPackage(package);
				if (!Directory.Exists(localPackagePath))
					Directory.CreateDirectory(localPackagePath);

				Uplift.Common.FileSystemUtil.CopyDirectory(td.Path, localPackagePath);
				upbring.AddLocation(package, InstallSpecType.Root, localPackagePath);

				VCSHandler.HandleDirectory(upfile.GetPackagesRootPath());

				InstallSpecPath[] specArray;
				if (package.Configuration == null)
				{
					// If there is no Configuration present we assume
					// that the whole package is wrapped in "InstallSpecType.Base"
					InstallSpecPath wrapSpec = new InstallSpecPath
					{
						Path = "",
						Type = InstallSpecType.Base
					};

					specArray = new[] { wrapSpec };
				}
				else
				{
					specArray = package.Configuration;
				}

				foreach (InstallSpecPath spec in specArray)
				{
					if (dependencyDefinition.SkipInstall != null && dependencyDefinition.SkipInstall.Any(skip => skip.Type == spec.Type)) continue;

					var sourcePath = Uplift.Common.FileSystemUtil.JoinPaths(td.Path, spec.Path);

					PathConfiguration PH = upfile.GetDestinationFor(spec);
					if (dependencyDefinition.OverrideDestination != null && dependencyDefinition.OverrideDestination.Any(over => over.Type == spec.Type))
					{
						PH.Location = Uplift.Common.FileSystemUtil.MakePathOSFriendly(dependencyDefinition.OverrideDestination.First(over => over.Type == spec.Type).Location);
					}

					var packageStructurePrefix =
						PH.SkipPackageStructure ? "" : GetPackageDirectory(package);

					var destination = Path.Combine(PH.Location, packageStructurePrefix);

					// Working with single file
					if (File.Exists(sourcePath))
					{
						// Working with singular file
						if (!Directory.Exists(destination))
						{
							Directory.CreateDirectory(destination);
							VCSHandler.HandleFile(destination);
						}
						if (Directory.Exists(destination))
						{ // we are copying a file into a directory
							destination = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourcePath));
						}
						File.Copy(sourcePath, destination);
						Uplift.Common.FileSystemUtil.TryCopyMeta(sourcePath, destination);

						if (destination.StartsWith("Assets"))
						{
							TryUpringAddGUID(upbring, sourcePath, package, spec.Type, destination);
						}
						else
						{
							upbring.AddLocation(package, spec.Type, destination);
						}

					}

					// Working with directory
					if (Directory.Exists(sourcePath))
					{
						// Working with directory
						Uplift.Common.FileSystemUtil.CopyDirectoryWithMeta(sourcePath, destination);
						if (!PH.SkipPackageStructure)
							VCSHandler.HandleDirectory(destination);

						bool useGuid = destination.StartsWith("Assets");
						foreach (var file in Uplift.Common.FileSystemUtil.RecursivelyListFiles(sourcePath, true))
						{
							if (useGuid)
								TryUpringAddGUID(upbring, file, package, spec.Type, destination);
							else
								upbring.AddLocation(package, spec.Type, Path.Combine(destination, file));

							if (PH.SkipPackageStructure)
								VCSHandler.HandleFile(Path.Combine(destination, file));
						}
					}
				}

				upbring.SaveFile();

				if (updateLockfile)
				{
					Debug.Log("Updating Lockfile entry for " + package.PackageName);
					LockfileManager.LockfileSnapshot snapshot = LockfileManager.LoadLockfile();
					int index;
					bool found = false;
					for (index = 0; index < snapshot.installableDependencies.Length; index++)
					{
						if (snapshot.installableDependencies[index].Package.PackageName == package.PackageName)
						{
							found = true;
							break;
						}
					}
					if (found)
					{
						snapshot.installableDependencies[index].Package = package;
					}
					else
					{
						Array.Resize<PackageRepo>(ref snapshot.installableDependencies, snapshot.installableDependencies.Length + 1);
						snapshot.installableDependencies[snapshot.installableDependencies.Length] = new PackageRepo { Package = package };
					}
					LockfileManager.GenerateLockfile(snapshot);
				}

				td.Dispose();
				UnityHacks.BuildSettingsEnforcer.EnforceAssetSave();
			}
		}

		private void CheckGUIDConflicts(string sourceDirectory, Upset package)
		{
			foreach (string file in Uplift.Common.FileSystemUtil.RecursivelyListFiles(sourceDirectory))
			{
				if (!file.EndsWith(".meta")) continue;
				string guid = LoadGUID(file);
				string guidPath = AssetDatabase.GUIDToAssetPath(guid);
				if (!string.IsNullOrEmpty(guidPath))
				{
					if (File.Exists(guidPath) || Directory.Exists(guidPath))
					{
						// the guid is cached and the associated file/directory exists
						Directory.Delete(sourceDirectory, true);
						throw new ApplicationException(
							string.Format(
								"The guid {0} is already used and tracks {1}. Uplift was trying to import a file with meta at {2} for package {3}. Uplift cannot install this package, please clean your project before trying again.",
								guid,
								guidPath,
								file,
								package.PackageName
							)
						);
					}
					// else, the guid is cached but there are no longer anything linked with it
				}
			}
		}

		private void TryUpringAddGUID(Upbring upbring, string file, Upset package, InstallSpecType type, string destination)
		{
			if (file.EndsWith(".meta")) return;
			string metaPath = Path.Combine(destination, file + ".meta");
			if (!File.Exists(metaPath))
			{
				upbring.AddLocation(package, type, Path.Combine(destination, file));
				return;
			}
			string guid = LoadGUID(metaPath);
			upbring.AddGUID(package, type, guid);
		}

		private string LoadGUID(string path)
		{
			const string guidMatcherRegexp = @"guid: (?<guid>\w+)";
			using (StreamReader sr = new StreamReader(path))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					Match matchObject = Regex.Match(line, guidMatcherRegexp);
					if (matchObject.Success) return matchObject.Groups["guid"].ToString();
				}
			}

			throw new InvalidDataException(string.Format("File {0} does not contain guid information", path));
		}

		private void UpdatePackage(Upset package, TemporaryDirectory td, bool updateLockfile)
		{
			NukePackage(package.PackageName);

			// First or default returns the first DependencyDefinition which satistfies dep.Name == package.PackageName
			// If no elements meets this condition a Default value for DependencyDefinition is returned which, for our implementation, is null. 
			DependencyDefinition definition = Upfile.Instance().Dependencies.FirstOrDefault(dep => dep.Name == package.PackageName);

			if (definition == null)
			{
				definition = new DependencyDefinition() { Name = package.PackageName, Version = package.PackageVersion };
			}

			InstallPackage(package, td, definition, updateLockfile);
		}

		public void UpdatePackage(PackageRepo newer, bool updateDependencies = true, bool updateLockfile = true)
		{
			InstalledPackage installed = Upbring.Instance().InstalledPackage.First(ip => ip.Name == newer.Package.PackageName);

			// If latest version is greater than the one installed, update to it
			if (string.Equals(newer.Package.PackageVersion, installed.Version))
			{
				UnityEngine.Debug.Log(string.Format("Required version of {0} is already installed ({1})", installed.Name, installed.Version));
				return;
			}
			else
			{
				using (TemporaryDirectory td = newer.Repository.DownloadPackage(newer.Package))
				{
					Debug.Log("Required version is not installed, updating package");
					UpdatePackage(newer.Package, td, updateLockfile);
				}
			}

			if (updateDependencies)
			{
				List<PackageRepo> packageRepo = GetDependencySolver().SolveDependencies(upfile.Dependencies);
				foreach (PackageRepo dependencyPR in packageRepo)
				{

					if (Upbring.Instance().InstalledPackage.Any(ip => ip.Name == dependencyPR.Package.PackageName))
					{
						UpdatePackage(dependencyPR,
									  updateDependencies: false,
									  updateLockfile: updateLockfile
									 );
					}
					else
					{
						using (TemporaryDirectory td = dependencyPR.Repository.DownloadPackage(dependencyPR.Package))
						{
							InstallPackage(dependencyPR.Package, td, null, true);
						}
					}
				}
			}
		}

		// What's the difference between Nuke and Uninstall?
		// Nuke doesn't care for dependencies (if present)
		public void NukePackage(string packageName)
		{
			Debug.Log("Nuking " + packageName);
			Upbring upbring = Upbring.Instance();
			InstalledPackage package = upbring.GetInstalledPackage(packageName);
			package.Nuke();
			upbring.RemovePackage(package);
			upbring.SaveFile();
			UnityHacks.BuildSettingsEnforcer.EnforceAssetSave();
		}
	}
}
