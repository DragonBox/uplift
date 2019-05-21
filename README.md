# UPLIFT

[![License](https://img.shields.io/badge/license-MIT-green.svg?style=flat)](https://github.com/DragonBox/uplift/blob/master/LICENSE)
![Version](https://img.shields.io/badge/version-1.0.0beta13-blue.svg)

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

First of all, take a look at the [Code of conduct](https://github.com/DragonBox/uplift/blob/master/CODE_OF_CONDUCT.md) and our [Contributor agreement](https://github.com/DragonBox/uplift/blob/master/CONTRIBUTING.md)

If you add a feature, please try to test it as thoroughly as possible!

## A word about PackMan

> Package Manager: Exposed the API for enabling internal components to be updated more frequently than the Editor. This is the first step in implementing the Unity Package Manager. We are taking an incremental approach to integrate the system into the Unity ecosystem. It will grow with more features over time. For this first release, we have avoided exposing user-facing features.

From the [Unity 2017.2 release notes](https://unity3d.com/fr/unity/whats-new/unity-2017.2.0).

Unity is currently developing their own package manager, and you may be interested in our stance on the subject of the future of Uplift after PackMan (or whatever name it will have) release.

__NOTE:__ The information we have about Unity's package manager is limited and could be not accurate. Do not take our word for granted when it comes to the features and limitations of their product. It is not yet released so it could still be subject to changes and the communication around it from Unity has been limited so we may be wrong.

The first thing that has to be said is that we do not think that Uplift can be objectively better than Unity's own made package manager. They have more time and resource than we do and have access to the core of their engine. Therefore, in the long run Uplift may not be the main package manager of Unity.
Nonetheless, we believe that Uplift has strengths that could motivate one to use it, despite the upcoming Unity product:

- First thing first, __Uplift is completly open source__. When it comes down to tools that can have a heavy impact on your project, we believe that having access to what is exactly happening is a good thing. You can fork it and use your own version, you can contribute to this one if you think that its behaviour should be enhanced; it is __YOUR__ tool.

- __Uplift is available right now__. The official Unity feature is not yet finished and we don't know exactly when it will be fully released, but we believe that it will not be before the end of 2018. You can start using Uplift right now and have all the benefits of a package manager without having to wait for several months.

- __Uplift has been designed to work with Unity 5.3 and upwards__ (and we partially support Unity 5.0 to 5.3). It is our understanding that Unity's package manager will only support the versions of Unity 2017 and upwards, so any project that you are not willing to migrate to 2017+ could not use it, while Uplift will work for these projects.

- Finally, __Uplift is not in complete opposition with PackMan__. Using Uplift requires you to do a big paradigm shift towards the use of a package manager. It encourages modularity, versioning and other concepts that are separate from the actual implementation of the package manager. If you start using Uplift right now and modularize some of you projects, your work will not be lost if you decide to shift to Unity's package manager when it is released. Most of it will actually be done, as a big part of using package managers requires you to adopt a specific mindset and switching from one another should not be too much of a hassle, and should remain an implementation detail.

In conclusion to this matter, we believe that Uplift and Unity's Package Manager do not completly overlap. Whether it is in scope or in implementation, we are confident that both of these products can find use without major conflict with the other.
