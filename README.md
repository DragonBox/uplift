# UPLIFT

[![License](https://img.shields.io/badge/license-MIT-green.svg?style=flat)](https://github.com/DragonBox/uplift/blob/master/LICENSE)
![Version](https://img.shields.io/badge/version-1.0.0beta1-blue.svg)

Uplift is a __package manager__ for the [Unity](https://unity3d.com/) game engine, built in __C#__, that provides a standard format for distributing Unity3d assets, and is designed to easily manage the installation of assets into your projects.

---

## What can it do?

Uplift is designed to make package usage easy and intuitive. It allows you to use packages, whether they are .unitypackage files downloaded from the AssetStore, packages you found on Github or packages that you have on your file system. Uplift brings the concepts of package repositories, package definitions, transitive dependencies and more to the Unity world.

Right now, it can __install__, __update__ and __remove__ packages as you see fit.

## How does it work?

The behaviour of Uplift is quite straight-forward: it relies on a few files to define what packages you want to install.

The most important file when you are a package user is the __Upfile.xml__ file that is situated in the root of your project. You specify every package that you want to use in it, where to fetch them from, and you can then use the new menu item `Uplift` in the Unity Editor to install, update or remove the specified packages.

Other really important file are the __Upset.xml__ which are present in every package that Uplift uses. These files describe what the package contains: its name, its version, and what type of file it uses. While this does not matter to you if you just plan on using packages, it is a critical part of Uplift's working.

## How do I use it?

We created a dedicated [Documentation](https://dragonbox.github.io/uplift_site/index.html) for you to understand how to use Uplift. It is more than just how tos as it requires you to adopt a certain philosophy regarding how to use packages in your project, and how to modularize it.

## How do bring Uplift into my project?

### Import the binaries

You can download the binaries (.dll or .unitypackage) from the [releases](https://github.com/DragonBox/uplift/releases) and put them directly into you project. Note that if you use the .dll, you will need to import a dependency of Uplift, [SharpCompress](https://github.com/adamhathcock/sharpcompress) into your project. This is not required for the .unitypackage version as the package already contains the version.

### Build it yourself

If you do not want to import foreign DLLs into your project, you can clone this repository and build it yourself. As this is rather a good practice, we have set up tools to easily build the Uplift DLL. You can build it:

* from the Editor: Go to `Tools > Uplift > Build > BuildDll`, and get the dll produced under the target directory at the project.

* from the command line:
    * if you use [u3d](https://github.com/DragonBox/u3d), you can do:

    ```shell
    u3d -- -logFile build.log -batchmode -quit -executeMethod BuildTool.DllCompiler.BuildPackage
    ```

    * if you don't, you first need to find the path to your Unity.exe for the current version of Uplift, and run

    ```shell
    PATH-TO-YOUR-UNITY.EXE -- -logFile build.log -batchmode -quit -executeMethod BuildTool.DllCompiler.BuildPackage
    ```

    * if you use Rake, we wrapped all the build process:

    ```shell
    cd DevelopmentTools; rake build; cd ..
    ```

## If you want to contribute

First of all, take a look at the [Code of conduct](https://github.com/DragonBox/uplift/blob/master.old/CODE_OF_CONDUCT.md).

If you add a feature, please try to test it as thoroughly as possible!
