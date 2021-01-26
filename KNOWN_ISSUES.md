Known Issues
============

# Package update

After working with Uplift for several months, the package update, which is a vital part of using a package manager on a daily basis, is a bad experience with a lot of issues.

## Update window performance

On larger projects with a lot of direct dependencies and transitive dependencies, the Uplift update window is slow, and reacts poorly, making the package update process for singular packages troublesome.

## Update window availability

The update window relies on the current state of dependencies being stable (no resolution conflicts, all packages including installed ones available online...) to be displayed, which means that it cannot be used to actually solve any issue.

## Lack of conservative update

There is no GUI for conservative updates, while the functionnality (or at least some form of it) is present in the form of the `UpliftManager.InstallStrategy.INCOMPLETE_LOCKFILE` strategy.

This results in updating dependencies from the Upfile.xml not being an option, as would be possible on other package managers.

## Update packages too present

The `Update packages` option in Uplift menu update __all packages as much as possible__. This feature, barely ever needed, is incentivised way too much resulting on a lot of users clicking it either by mistake or by confusion. This in turns leads to broken states that need to be cleaned up.

# Reliability

There are still some reliability issues with the existing installing procedure under very specific situations resulting in packages poorly uninstalled, wrong package version being installed...

The issues are hard to replicate, and the context under which they appear even harder to define.

# Performance consideration when working with Git LFS

Git LFS has reportedly been slowed down quite a lot by large .gitignore files [Issue #2221](https://github.com/git-lfs/git-lfs/issues/2221), [Issue #3668](https://github.com/git-lfs/git-lfs/issues/3668) which Uplift tends to create by relying on it to make sure that packages are not added to git.

It is supposed to be addressed by [latest versions of Git LFS](https://github.com/git-lfs/git-lfs/pull/2233).