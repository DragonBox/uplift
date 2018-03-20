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
using System.Xml;
using UnityEngine;
using Uplift.Common;
using Uplift.Packages;

namespace Uplift.Schemas
{
    public partial class Upfile
    {
        // --- SINGLETON DECLARATION ---
        protected static Upfile instance;

        internal Upfile() { }

        public static Upfile Instance()
        {
            if (instance == null)
            {
                InitializeInstance();
            }

            return instance;
        }

        public static void ResetInstance()
        {
            instance = null;
            InitializeInstance();
        }

        internal static void InitializeInstance()
        {
            instance = null;
            if (!CheckForUpfile())
            {
                Debug.Log("No Upfile in this project");
                return;
            }

            instance = LoadXml();
            instance.CheckUnityVersion();
            instance.LoadPackageList();
        }

        // --- CLASS DECLARATION ---
        public static readonly string upfilePath = "Upfile.xml";
        public string overridePath;

        public static bool CheckForUpfile()
        {
            return File.Exists(upfilePath);
        }

        internal static Upfile LoadXml()
        {
            return LoadXml(upfilePath);
        }

        internal static Upfile LoadXml(string path)
        {
            try
            {
                StrictXmlDeserializer<Upfile> deserializer = new StrictXmlDeserializer<Upfile>();

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    Upfile upfile = deserializer.Deserialize(fs);

                    upfile.MakePathConfigurationsOSFriendly();
                    upfile.LoadOverrides();

                    if (upfile.Repositories != null)
                    {
                        foreach (Repository repo in upfile.Repositories)
                        {
                            if (repo is FileRepository)
                                (repo as FileRepository).Path = Uplift.Common.FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path);
                        }
                    }

                    Debug.Log("Upfile was successfully loaded");
                    return upfile;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Uplift: Could not load Upfile", e);
            }
        }

        public void SaveFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(upfilePath);
            XmlElement root = doc.DocumentElement;

            // Set Unity Version
            root.SelectSingleNode("UnityVersion").InnerText = UnityVersion;

            // Set Repositories
            foreach(Repository repo in Repositories)
                if(!GetRepositoryOverrides().Any(extraRepo =>
                    extraRepo is FileRepository &&
                    Uplift.Common.FileSystemUtil.MakePathOSFriendly((extraRepo as FileRepository).Path) == Uplift.Common.FileSystemUtil.MakePathOSFriendly((repo as FileRepository).Path)
                ))
                    AddOrReplaceRepository(doc, repo);

            foreach(XmlNode node in root.SelectSingleNode("Repositories").SelectNodes("FileRepository"))
            {
                if(!Repositories.Any(repo => (repo as FileRepository).Path == node.Attributes["Path"].Value))
                    root.SelectSingleNode("Repositories").RemoveChild(node);
            }

            // Set Configuration
            SetPathConfiguration(doc,   "BaseInstallPath",  Configuration.BaseInstallPath);
            SetPathConfiguration(doc,   "DocsPath",         Configuration.DocsPath);
            SetPathConfiguration(doc,   "EditorPluginPath", Configuration.EditorPluginPath);
            SetPathConfiguration(doc,   "ExamplesPath",     Configuration.ExamplesPath);
            SetPathConfiguration(doc,   "GizmoPath",        Configuration.GizmoPath);
            SetPathConfiguration(doc,   "MediaPath",        Configuration.MediaPath);
            SetPathConfiguration(doc,   "PluginPath",       Configuration.PluginPath);
            SetPathConfiguration(doc,   "RepositoryPath",   Configuration.RepositoryPath);
			SetPathConfiguration(doc,   "EditorDefaultResourcePath", Configuration.EditorDefaultResourcePath);

            // Set Dependencies
            foreach(DependencyDefinition def in Dependencies)
                AddOrReplaceDependency(doc, def);

            foreach(XmlNode node in root.SelectSingleNode("Dependencies").SelectNodes("Package"))
            {
                if(!Dependencies.Any(def => def.Name == node.Attributes["Name"].Value))
                    root.SelectSingleNode("Dependencies").RemoveChild(node);
            }

            doc.Save(upfilePath);   
        }

        private void AddOrReplaceRepository(XmlDocument document, Repository repository)
        {
            XmlNode main = document.DocumentElement.SelectSingleNode("Repositories");
            // FIXME: support other repositories when necessary
            XmlNodeList repositoryList = main.SelectNodes("FileRepository");
            foreach(XmlNode node in repositoryList)
            if(node.Attributes["Path"].Value == (repository as FileRepository).Path)
                {
                    main.RemoveChild(node);
                    break;
                }

            XmlElement repo = document.CreateElement("FileRepository");
            repo.SetAttribute("Path", Uplift.Common.FileSystemUtil.MakePathUnix((repository as FileRepository).Path));
            
            main.AppendChild(repo);
        }

        private void SetPathConfiguration(XmlDocument document, string nodeName, PathConfiguration pc)
        {
            XmlNode original = document.DocumentElement.SelectSingleNode("Configuration").SelectSingleNode(nodeName);
            XmlNode temp = original;

            temp.Attributes["Location"].Value = Uplift.Common.FileSystemUtil.MakePathUnix(pc.Location);
            if(pc.SkipPackageStructureSpecified)
            {
                if(temp.Attributes["SkipPackageStructure"] == null)
                    temp.Attributes.Append(document.CreateAttribute("SkipPackageStructure"));

                temp.Attributes["SkipPackageStructure"].Value = pc.SkipPackageStructure.ToString().ToLower();
            }
            else
                temp.Attributes.RemoveNamedItem("SkipPackageStructure");

            document.DocumentElement.SelectSingleNode("Configuration").ReplaceChild(original, temp);
        }

        private void AddOrReplaceDependency(XmlDocument document, DependencyDefinition def)
        {
            XmlNode main = document.DocumentElement.SelectSingleNode("Dependencies");
            XmlNodeList dependencyList = main.SelectNodes("Package");
            foreach(XmlNode node in dependencyList)
                if(node.Attributes["Name"].Value == def.Name)
                {
                    main.RemoveChild(node);
                    break;
                }
            
            XmlElement dependency = document.CreateElement("Package");
            dependency.SetAttribute("Name", def.Name);
            dependency.SetAttribute("Version", def.Version);
            if(def.OverrideDestination != null)
            {
                XmlElement overrideNode = document.CreateElement("OverrideDestination");
                foreach(OverrideDestinationSpec spec in def.OverrideDestination)
                {
                    XmlElement over = document.CreateElement("Override");
                    over.SetAttribute("Type", spec.Type.ToString());
                    over.SetAttribute("Location", spec.Location);
                    overrideNode.AppendChild(over);
                }
                dependency.AppendChild(overrideNode);
            }
            if(def.SkipInstall != null)
            {
                XmlElement skipNode = document.CreateElement("SkipInstall");
                foreach(SkipInstallSpec spec in def.SkipInstall)
                {
                    XmlElement skip = document.CreateElement("Skip");
                    skip.SetAttribute("Type", spec.Type.ToString());
                    skipNode.AppendChild(skip);
                }
                dependency.AppendChild(skipNode);
            }
            main.AppendChild(dependency);
        }

        public virtual void LoadOverrides()
        {
            Repository[] overrides = GetRepositoryOverrides();
            if (Repositories == null)
            {
                Repositories = overrides;
            }
            else if(overrides != null)
            {
                int repositoriesSize = Repositories.Length + overrides.Length;

                Repository[] newRepositoryArray = new Repository[repositoriesSize];
                Array.Copy(Repositories, newRepositoryArray, Repositories.Length);
                Array.Copy(overrides, 0, newRepositoryArray, Repositories.Length, overrides.Length);

                Repositories = newRepositoryArray;
            }
        }

        internal Repository[] GetRepositoryOverrides()
        {
            Repository[] result = new Repository[0];
            try
            {
                result = string.IsNullOrEmpty(overridePath) ?
                    UpliftSettings.FromDefaultFile().Repositories :
                    UpliftSettings.FromFile(overridePath).Repositories;
            }
            catch (Exception e)
            {
                Debug.LogError("Could not load repositories overrides from .Uplift file\n" + e);
            }

            return result;
        }

        public string GetPackagesRootPath()
        {
            return Configuration.RepositoryPath.Location;
        }

        public void LoadPackageList()
        {
            PackageList pList = PackageList.Instance();
            pList.LoadPackages(Repositories, true);
        }

        //FIXME: Prepare proper version checker
        public virtual void CheckUnityVersion()
        {
            string environmentVersion = Application.unityVersion;
            if(VersionParser.ParseUnityVersion(environmentVersion) < VersionParser.ParseUnityVersion(UnityVersion))
            {
                Debug.LogError(string.Format("Upfile.xml Unity Version ({0}) targets a higher version of Unity than you are currently using ({1})",
                    UnityVersion, environmentVersion));
            }
        }

        public void MakePathConfigurationsOSFriendly()
        {
            foreach(PathConfiguration path in PathConfigurations())
            {
                if (!(path == null))
                    path.Location = Uplift.Common.FileSystemUtil.MakePathOSFriendly(path.Location);
            }
        }

        public IEnumerable<PathConfiguration> PathConfigurations()
        {
            if (Configuration == null) yield break;
            yield return Configuration.BaseInstallPath;
            yield return Configuration.DocsPath;
            yield return Configuration.EditorPluginPath;
            yield return Configuration.ExamplesPath;
            yield return Configuration.GizmoPath;
            yield return Configuration.MediaPath;
            yield return Configuration.PluginPath;
            yield return Configuration.RepositoryPath;
			yield return Configuration.EditorDefaultResourcePath;
        }

        public PathConfiguration GetDestinationFor(InstallSpec spec)
        {
            PathConfiguration PH;

            var specType = spec.Type;

            switch (specType)
            {
                case (InstallSpecType.Base):
                    PH = Configuration.BaseInstallPath;
                    break;

                case (InstallSpecType.Docs):
                    PH = Configuration.DocsPath;
                    break;

                case (InstallSpecType.EditorPlugin):
                    PH = Configuration.EditorPluginPath;
                    break;

				case (InstallSpecType.EditorDefaultResource):
					PH = Configuration.EditorDefaultResourcePath;
					break;

                case (InstallSpecType.Examples):
                    PH = Configuration.ExamplesPath;
                    break;

                case (InstallSpecType.Gizmo):
                    PH = Configuration.GizmoPath;
                    break;

                case (InstallSpecType.Media):
                    PH = Configuration.MediaPath;
                    break;

                case (InstallSpecType.Plugin):
                    PH = Configuration.PluginPath;

                    // Platform as string
                    string platformAsString;

                    switch (spec.Platform)
                    {
                        case (PlatformType.All): // It means, that we just need to point to "Plugins" folder.
                            platformAsString = "";
                            break;
                        case (PlatformType.iOS):
                            platformAsString = "ios";
                            break;
                        default:
                            platformAsString = "UNKNOWN";
                            break;
                    }
                    PH.Location = Path.Combine(PH.Location, platformAsString);
                    break;

                default:
                    PH = Configuration.BaseInstallPath;
                    break;
            }

            return PH;
        }

        public IEnumerable<Upset> ListPackages()
        {
            if (Repositories == null) yield break;
            foreach(Repository repository in Repositories)
            {
                foreach(Upset package in repository.ListPackages())
                {
                    yield return package;
                }
            }
        }
    }
}
