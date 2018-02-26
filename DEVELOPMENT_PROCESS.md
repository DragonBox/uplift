# Development process

This document is to guide anyone allowed to write to the main Uplift repository to assist in releasing

## RELEASING A NEWER VERSION

- __Bumping__ the version:

```shell
rake bump
```

The first step is to describe which version you are going to release. You will be asked to input the newer version.

- __Preparing__ the release:

```shell
rake pre_release
```

Next you need to prepare the next release: make sure that the git is clean, generate the changelog and create a pull request for the release.

_NOTE:_ You should approve and merge the PR before going to the next step

- Actually __releasing__ the project:

```shell
rake release
```

This creates a draft for a release, with a title, a changelog and builds and attach the `.unitypackages` files for the target versions to the release.

_NOTES:_

. The release is only drafted and you need to accept it before it is available to the public

. Because we build on several targets, git may not be clean as Unity could have updated some scripts when switching API (this is for example the case between `5.6.5f1` and `2017.1.2f1`). You will probably need to `git checkout` some modifications!
