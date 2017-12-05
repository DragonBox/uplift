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

using System.Xml;
using UnityEngine;
using Uplift.Schemas;

namespace Uplift.Common
{
    public class SampleFile
    {
        internal static readonly string upfile = @"<Upfile>
  <UnityVersion>{0}</UnityVersion>   
  <!-- SAMPLE REPOSITORIES BLOCK

  <Repositories>
    <FileRepository Path=""Path/To/Some/File/Repository"" />
  </Repositories>

  -->

  <Repositories>
  </Repositories>
    
  <Configuration>
    <!--
        Important note:

        We're not tracing Location settings for anything unpacked outside of Assets.
        This means, that if you change those in settings while already
        having packages/other stuff installed then you need to cleanup
        previous locations manually at your own discretion.
    -->
    <!--
        Path Flags:

        SkipPackageStructure (true|false):
          Don't create package subdirectories
          (in form of PackageName~1.0.0)
    -->
  
    <!--
        Repository configuration part.

        Most often, files here shouldn't be visible by your project.
        i.e. should be put out of the Assets/ path in your project.
    -->

    <!-- Path where downloaded, raw, packages are unpacked        -->
    <RepositoryPath  Location=""UPackages""                         />
    <!-- Path where documentation are unpacked                    -->
    <DocsPath        Location=""UPackages/Docs""                    />
    <!-- Path where examples are unpacked                         -->
    <ExamplesPath    Location=""UPackages/Examples""                />

    <!--
        Project configuration part.

        Paths set here most often should be visible by your project,
        i.e.be in Assets path.
    -->

    <!-- Path where ""usable"" files are unpacked                   -->
    <BaseInstallPath    Location=""Assets/UPackages""               />
    <!-- Path where media  files are unpacked                     -->
    <MediaPath          Location=""Assets/UPackages""               />

    <!-- 
        NOTE: the following paths have specific functionnality in Unity.
        Please make sure you know what you're doing before modifying them.
        Please see: https://docs.unity3d.com/Manual/SpecialFolders.html
    -->
    <!-- Path where plugin files are unpacked                     -->
    <PluginPath         Location=""Assets/Plugins""         SkipPackageStructure=""true"" />
    <!-- Path where editor plugin files are unpacked              -->
    <EditorPluginPath   Location=""Assets/Plugins/Editor""  SkipPackageStructure=""true"" />
    <!-- Path where gizmos are unpacked                           -->
    <GizmoPath          Location=""Assets/Gizmos""          SkipPackageStructure=""true"" />

  </Configuration>

  <Dependencies>

  </Dependencies>

  <!-- SAMPLE DEPENDENCIES BLOCK

  <Dependencies>
    <Package Name=""SomePackage"" Repository=""Optional.Repository"" Version=""Optional.Version.Spec"" />
    <Package Name=""PackageA"">
      <MajorVersionMin>3</MajorVersionMin>
    </Package>
  </Dependencies>

  -->
</Upfile>";

        internal static readonly string settings = @"<UpliftSettings>
  <!-- SAMPLE REPOSITORIES BLOCK

    <Repositories>
      <FileRepository Path=""Path/To/Some/File/Repository"" />
    </Repositories>

  -->
  <!-- 

    The repositories in your settings file usually are local development repositories or ""manually"" synchronized shared repositories

  -->
  <Repositories>
  </Repositories>

  <!-- SAMPLE REPOSITORIES BLOCK

    <AuthenticationMethods>
      <RepositoryToken Repository=""https://api.github.com/repos/ME/MYREPO/releases"" Token=""mytokenhash"" />
    </AuthenticationMethods>

  -->
  <AuthenticationMethods>
  </AuthenticationMethods>
</UpliftSettings>";

        public static void CreateSampleUpfile()
        {
            XmlDocument sampleFile = new XmlDocument();

            sampleFile.InnerXml = string.Format(upfile, Application.unityVersion);

            sampleFile.Save("Upfile.xml");
        }

        public static void CreateSampleSettingsFile()
        {
            XmlDocument sampleFile = new XmlDocument();

            sampleFile.InnerXml = settings;

            sampleFile.Save(UpliftSettings.GetDefaultLocation());
        }
    }
}
