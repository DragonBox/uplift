# Uplift

Uplift is a __package manager__ for the [Unity](https://unity3d.com/) game engine, built in __C#__, that provides a standard format for distributing Unity3d assets, and is a tool designed to easily manage the installation of assets into your projects.

## What can it do?

 Uplift is designed to make package usage easy and intuitive. It allows you to use packages, whether they are .unitypackage files downloaded from the AssetStore, packages you found on Github or packages that you have on your file system. Uplift brings the concepts of package repositories, package definitions, transitive dependencies and more to the Unity world.

Right now, it can __install__, __update__ and __remove__ packages as you see fit.

## How does it work?

The behaviour of Uplift is quite straight-forward: it relies on a few files to define what packages you want to install.

The most important file when you are a package user is the __Upfile.xml__ file that is situated in the root of your project. You specify every package that you want to use in it, where to fetch them from, and you can then use the new menu item `Uplift` in the Unity Editor to install, update or remove the specified packages.

Other really important file are the __Upset.xml__ which are present in every package that Uplift uses. These files describe what the package contains: its name, its version, and what type of file it uses. While this does not matter to you if you just plan on using packages, it is a critical part of Uplift's working.

## How do I use it?

Whether you create packages or consume packages, the first thing that you want to do is add Uplift to the Unity project you want to work with. You can do that by either downloading the .unitypackage present in the root of this repository or _head to the Unity [AssetStore](TODO/publish/to/the/assetstore) to directly download it to your project_.

### I am a package user

The only thing that you will want to do is specify an __Upfile.xml__ at the root of your project. To make things easier, you can generate a blank one using the menu: `Uplift > Generate Upfile`.

__NOTE__: You can add and remove any kind of information from the Upfile at any point, just make sure you call `Uplift > Refresh Upfile` when you do so your modifications are taken into account!

Once you have created the file, you will have to fill it in. The first thing that you will want to do is to specify where you want to get your packages from, in the `Repositories` section. You can put there a list of repositories from which you will fetch your packages.

__Example:__ if you want to fetch packages from a folder on your computer, you can add the following line to the Repositories:
```xml
<FileRepository Path="Path/To/Wherever/My/Packages/Are" />
```
Replacing the value of the attribute `Path` by an actual path.

Once all of your repositories have been specify, you can head to the `Configuration` section. In it you can specify where will Uplift store and install the files fetched from the `Repositories`.

__Example:__ if you want to unpack the Examples outside of the Assets folder and rather to a folder called 'Examples', you can specify:
```xml
<ExamplesPath Location="Examples" />
```

You can then add all the dependencies of your project in the `Dependencies` section of the Upfile.

__Example:__ if you want to specify that you depend on the version `1.8` of a package called `foo`, you can add the following line to the Dependencies:
```xml
<Package Name="foo" Version="1.8" />
```

And that's it! Now if you go to `Upflift > Install Dependencies`, Uplift will go through the `Dependencies` section, fetch them from the `Repositories` and will install them where you specified in `Configuration`.

### I am a package creator

If you are used to create packages for Unity, creating packages for Uplift should be as easy. Work as you usually do, but right before you finalize your package, you will simply need to create an __Upset.xml__ which will contain all the information related to your package.

The minimal information that Uplift requires are:

* The package Name: Quite straight-forward, it is the name of your package.
```xml
<PackageName>name_of_my_package</PackageName>
```
* The package Version: The current version of your project.
```xml
<PackageVersion>1.2.3</PackageVersion>
```
* The package License*: The License that you chose for your package.
```xml
<PackageLicense>MIT License</PackageLicense>
```

You can add further information about your package in the Upset:
* Requirements regarding Unity: If you support only specific versions of Unity, you can specify it there so your package can't be used on Unity whose versions do not match your requirements.

__Example:__ I created my package for Unity 5+ and I don't plan to support previous versions. I can then add:
```xml
<UnityVersion>
  <MinVersion>5.0.0</MinVersion>
</UnityVersion>
```

* Package Configuration: Similarly to how the Upfile's configuration works, I can specify what files are in my package so the user can unpack it wherever he wants.

__Example:__ I have put all the documentation of my package under a 'Documentation' folder of my package. I can add this line to the `Configuration` section:
```xml
<Spec Type="Docs" Path="Documentation"/>
```

\* __Why is a license required?__ Because it is essential to your package. Lots of Unity projects will have a commercial use, and if they use your package, they use material that you created; package user therefore needs to ensure that using your created package is something he can do lawfully. [More on that](https://unity3d.com/legal/as_terms).

## Roadmap
### What needs to be done

* Upset support for .unitypackage files, right now version and package name are assumed from the file name, but this needs to be implemented.

* More repositories: we only support file repositories at the moment, and we should be able to support other sources such as (not limited to): Git, Dropbox and Google Drive.

* Transitive dependencies resolution: if a package A depends on a package B, we need to make sure that package B is installed before installing package A, with matching versions.

* Better UI implementation

### What features are planned

* Package exporter/Upset editor: in order to streamline the Uplift package creation, we are planning to create a package creator which would basically act as an overlay for the export package feature of Unity, creating and integrating the Upset file at package creation.

* Support for package setup on installation: when one installs a package, having to setup a few things by hand is common, and it would be interesting in that regard to be able to automatize package setup thanks to a few XML lines in the dependency creation in Upfile.
