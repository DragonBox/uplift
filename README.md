Uplift is a package management tool for Unity3d that provides a standard format for distributing Unity3d assets, a tool designed to easily manage the installation of assets into your projects.

Uplift brings the concepts of package repositories, package definitions, transitive dependencies and more to the Unity world.

Uplift is open source and extensible.

**Problem**:

As your projects grow, you will want to reuse thir party code/assets or share them between projects. To that effect, Unity provides [Asset packages](https://docs.unity3d.com/Manual/AssetPackages.html), a simple yet convenient method, notably used by the Asset Store. Other ways to achieve reuse might include source control specific methods such as git submodules or file system ones (shared folders, links).

While these techniques make it easy for someone to package assets, they leave some burdain to the package integrator. Here are some of the shortcomings you might encounter:

* where to place these assets in your project is left to you. Leaving the assets in the location where the asset packager placed them might cause issues. Moving them around and you have some manual work to do for each new version, for each project you integrate the assets into
* you might need to maintain third party assets (e.g. to support a not yet supported platform, to prevent conflicts between packages, etc)
* transitive dependencies are not managed, i.e. you cannot easily define a package dependency on another one except by describing it in the package documentation.
* updating packages is often manual job, where you have to deal with file name changes, excluding things you might not want, moving files around, etc
* while assets in the AssetStore are under the same license, third party assets come increasingly from outside of the AssetStore. One need to be clearly aware of the licensing these assets before using them.

**Solution**:

Uplift brings several concepts to relieve you of some of this integrator work:
* asset repositories, a place where to store a collection of packages and their updates
* Upset files, the format in which one describes packages and their dependencies
* Upfile, to describe where to look for packages, which packages to include in your project, and how to import them

Also, uplift uses an Upbring file to maintain the state of the integration it performs.