# Changelog

## [v1.0.0beta18](https://github.com/DragonBox/uplift/tree/v1.0.0beta18) (2021-11-24)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta17...v1.0.0beta18)

**Merged pull requests:**

- Bump activesupport from 6.0.2.1 to 6.1.4.1 [\#113](https://github.com/DragonBox/uplift/pull/113) ([dependabot[bot]](https://github.com/apps/dependabot))
- We need to continue nuking when we find out an asset that was already… [\#112](https://github.com/DragonBox/uplift/pull/112) ([lacostej](https://github.com/lacostej))
- Bump addressable from 2.7.0 to 2.8.0 [\#111](https://github.com/DragonBox/uplift/pull/111) ([dependabot[bot]](https://github.com/apps/dependabot))
- Bump rack from 2.2.2 to 2.2.3 [\#109](https://github.com/DragonBox/uplift/pull/109) ([dependabot[bot]](https://github.com/apps/dependabot))
- Make sure the default ci\_build doesn't use deprecated 5.6 version by default [\#107](https://github.com/DragonBox/uplift/pull/107) ([lacostej](https://github.com/lacostej))

## [v1.0.0beta17](https://github.com/DragonBox/uplift/tree/v1.0.0beta17) (2020-03-09)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta16...v1.0.0beta17)

**Merged pull requests:**

- Preparing release for 1.0.0beta17 [\#106](https://github.com/DragonBox/uplift/pull/106) ([lacostej](https://github.com/lacostej))
- Bump rack from 2.0.6 to 2.2.2 [\#105](https://github.com/DragonBox/uplift/pull/105) ([dependabot[bot]](https://github.com/apps/dependabot))
- Feature/support unity 2017 4 [\#104](https://github.com/DragonBox/uplift/pull/104) ([lacostej](https://github.com/lacostej))
- Bump rake from 12.3.2 to 13.0.1 [\#99](https://github.com/DragonBox/uplift/pull/99) ([dependabot[bot]](https://github.com/apps/dependabot))

## [v1.0.0beta16](https://github.com/DragonBox/uplift/tree/v1.0.0beta16) (2020-03-06)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta15...v1.0.0beta16)

**Implemented enhancements:**

- XML file\(s\) format/versioning/upgrade mechanism [\#4](https://github.com/DragonBox/uplift/issues/4)
- UplfitManager: do not solve dependencies if InstallStrategy.ONLY\_LOCKFILE [\#94](https://github.com/DragonBox/uplift/pull/94) ([niezbop](https://github.com/niezbop))
- Feature/logging/verbose [\#93](https://github.com/DragonBox/uplift/pull/93) ([niezbop](https://github.com/niezbop))

**Fixed bugs:**

- GitIgnorer: do not register .meta files in the .gitignore [\#95](https://github.com/DragonBox/uplift/pull/95) ([niezbop](https://github.com/niezbop))
- Do not update dependencies when installing several dependencies at once [\#92](https://github.com/DragonBox/uplift/pull/92) ([niezbop](https://github.com/niezbop))

**Closed issues:**

- .gitignore contains .meta.meta entries [\#87](https://github.com/DragonBox/uplift/issues/87)
- Support Nuget Based Repository [\#65](https://github.com/DragonBox/uplift/issues/65)

**Merged pull requests:**

- Preparing release for 1.0.0beta16 [\#103](https://github.com/DragonBox/uplift/pull/103) ([niezbop](https://github.com/niezbop))
- Revert to v1.0.0beta14 with backports to fix urgent bug [\#102](https://github.com/DragonBox/uplift/pull/102) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta15](https://github.com/DragonBox/uplift/tree/v1.0.0beta15) (2019-10-30)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta14...v1.0.0beta15)

**Fixed bugs:**

- Fix/RangeVersionRequirement restriction to another one [\#90](https://github.com/DragonBox/uplift/pull/90) ([niezbop](https://github.com/niezbop))

**Merged pull requests:**

- Preparing release for 1.0.0beta15 [\#91](https://github.com/DragonBox/uplift/pull/91) ([niezbop](https://github.com/niezbop))
- Extract lockfile specific logic from UpliftManager [\#89](https://github.com/DragonBox/uplift/pull/89) ([scassard](https://github.com/scassard))
- Feature/new dependency resolution [\#88](https://github.com/DragonBox/uplift/pull/88) ([scassard](https://github.com/scassard))

## [v1.0.0beta14](https://github.com/DragonBox/uplift/tree/v1.0.0beta14) (2019-05-29)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta12...v1.0.0beta14)

**Merged pull requests:**

- Preparing release for 1.0.0beta14 [\#85](https://github.com/DragonBox/uplift/pull/85) ([lacostej](https://github.com/lacostej))
- Debug/add a log handler [\#84](https://github.com/DragonBox/uplift/pull/84) ([scassard](https://github.com/scassard))
- Fix/lockfile/prevent editing of transitive dependency when installing from lockfile [\#83](https://github.com/DragonBox/uplift/pull/83) ([scassard](https://github.com/scassard))
- Add missing usings [\#82](https://github.com/DragonBox/uplift/pull/82) ([scassard](https://github.com/scassard))
- Revert "Fix/lockfile/fix lockfile modifications when installing from lockfile" [\#80](https://github.com/DragonBox/uplift/pull/80) ([niezbop](https://github.com/niezbop))
- Display PackageExport.asset after its creation [\#78](https://github.com/DragonBox/uplift/pull/78) ([scassard](https://github.com/scassard))
- Fix/lockfile/fix lockfile modifications when installing from lockfile [\#77](https://github.com/DragonBox/uplift/pull/77) ([scassard](https://github.com/scassard))
- Fix/export/prevent uplift installing itself [\#74](https://github.com/DragonBox/uplift/pull/74) ([scassard](https://github.com/scassard))
- Build: split Rakefile for testing the release notes upload [\#73](https://github.com/DragonBox/uplift/pull/73) ([lacostej](https://github.com/lacostej))
- Preparing release for 1.0.0beta13 [\#72](https://github.com/DragonBox/uplift/pull/72) ([jenkinswwtk](https://github.com/jenkinswwtk))
- Format files according to wwtk templates [\#71](https://github.com/DragonBox/uplift/pull/71) ([scassard](https://github.com/scassard))
- Correct update package function to prevent error in First method [\#70](https://github.com/DragonBox/uplift/pull/70) ([scassard](https://github.com/scassard))

## [v1.0.0beta12](https://github.com/DragonBox/uplift/tree/v1.0.0beta12) (2019-03-26)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta11...v1.0.0beta12)

**Fixed bugs:**

- LoseVersionRequirement and MinimalVersionRequirement faulty behaviour [\#66](https://github.com/DragonBox/uplift/issues/66)

**Merged pull requests:**

- Add github\_changelog\_generator as a dependency in the Gemfile [\#69](https://github.com/DragonBox/uplift/pull/69) ([niezbop](https://github.com/niezbop))
- Preparing release for 1.0.0beta12 [\#68](https://github.com/DragonBox/uplift/pull/68) ([niezbop](https://github.com/niezbop))
- Introduce Range version requirement [\#67](https://github.com/DragonBox/uplift/pull/67) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta11](https://github.com/DragonBox/uplift/tree/v1.0.0beta11) (2018-12-18)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta10...v1.0.0beta11)

**Implemented enhancements:**

- Preferences: use .uplift/preferences.xml rather than EditorPrefs for storage [\#63](https://github.com/DragonBox/uplift/pull/63) ([niezbop](https://github.com/niezbop))
- Support editor default resources [\#58](https://github.com/DragonBox/uplift/pull/58) ([niezbop](https://github.com/niezbop))

**Closed issues:**

- Git says MIT, docs say All rights reserved [\#61](https://github.com/DragonBox/uplift/issues/61)

**Merged pull requests:**

- Preparing release for 1.0.0beta11 [\#64](https://github.com/DragonBox/uplift/pull/64) ([niezbop](https://github.com/niezbop))
- Menu Items: Make installation from menu clearer [\#60](https://github.com/DragonBox/uplift/pull/60) ([niezbop](https://github.com/niezbop))
- Project: Add a TODO list and add necessary env variables to DEVELOPMENT\_PROCESS [\#59](https://github.com/DragonBox/uplift/pull/59) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta10](https://github.com/DragonBox/uplift/tree/v1.0.0beta10) (2018-03-15)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta9...v1.0.0beta10)

**Implemented enhancements:**

- Log current Uplift version on Initialize [\#54](https://github.com/DragonBox/uplift/pull/54) ([niezbop](https://github.com/niezbop))

**Fixed bugs:**

- Fix: Better optional parameter parsing [\#56](https://github.com/DragonBox/uplift/pull/56) ([niezbop](https://github.com/niezbop))
- Make sure that the preferences are loaded before using them [\#55](https://github.com/DragonBox/uplift/pull/55) ([niezbop](https://github.com/niezbop))

**Merged pull requests:**

- Preparing release for 1.0.0beta10 [\#57](https://github.com/DragonBox/uplift/pull/57) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta9](https://github.com/DragonBox/uplift/tree/v1.0.0beta9) (2018-02-26)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta8...v1.0.0beta9)

**Implemented enhancements:**

- Refactor ExportPackage definition [\#46](https://github.com/DragonBox/uplift/pull/46) ([niezbop](https://github.com/niezbop))

**Merged pull requests:**

- Release 1.0.0beta9 [\#53](https://github.com/DragonBox/uplift/pull/53) ([niezbop](https://github.com/niezbop))
- Use Unity 5.6.5f1 [\#52](https://github.com/DragonBox/uplift/pull/52) ([niezbop](https://github.com/niezbop))
- Allows to specify a proxy for GithubRepository [\#51](https://github.com/DragonBox/uplift/pull/51) ([niezbop](https://github.com/niezbop))
- Cleaner failure when trying to download an asset [\#50](https://github.com/DragonBox/uplift/pull/50) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta8](https://github.com/DragonBox/uplift/tree/v1.0.0beta8) (2017-12-14)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta7...v1.0.0beta8)

**Implemented enhancements:**

- .gitignore generation with line ending [\#31](https://github.com/DragonBox/uplift/issues/31)
- Settings: ensure .upfile exists before attempting to create sample [\#40](https://github.com/DragonBox/uplift/pull/40) ([lacostej](https://github.com/lacostej))
- Change package~name to package-name [\#37](https://github.com/DragonBox/uplift/pull/37) ([niezbop](https://github.com/niezbop))

**Fixed bugs:**

- Upfile dependencies not refreshed before installation [\#38](https://github.com/DragonBox/uplift/issues/38)
- Fix update window offering to update to old versions [\#44](https://github.com/DragonBox/uplift/pull/44) ([niezbop](https://github.com/niezbop))
- Fix lose and minimal version requirement merge [\#42](https://github.com/DragonBox/uplift/pull/42) ([niezbop](https://github.com/niezbop))
- Fix Updater not comparing beta versions correctly [\#36](https://github.com/DragonBox/uplift/pull/36) ([niezbop](https://github.com/niezbop))

**Closed issues:**

- GUI Update Window says outdated, while I have newer. [\#43](https://github.com/DragonBox/uplift/issues/43)
- uplift self-update mechanism should handle optional field in version \(e.g. beta7\) [\#35](https://github.com/DragonBox/uplift/issues/35)

**Merged pull requests:**

- Preparing release for 1.0.0beta8 [\#45](https://github.com/DragonBox/uplift/pull/45) ([niezbop](https://github.com/niezbop))
- Fix upfile not refreshed before install [\#39](https://github.com/DragonBox/uplift/pull/39) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta7](https://github.com/DragonBox/uplift/tree/v1.0.0beta7) (2017-12-07)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta6...v1.0.0beta7)

**Implemented enhancements:**

- Add upset template to export package with [\#33](https://github.com/DragonBox/uplift/pull/33) ([niezbop](https://github.com/niezbop))

**Fixed bugs:**

- Fix lack of support for unknown repositories in Upfile Editor window [\#32](https://github.com/DragonBox/uplift/pull/32) ([niezbop](https://github.com/niezbop))

**Merged pull requests:**

- Preparing release for 1.0.0beta7 [\#34](https://github.com/DragonBox/uplift/pull/34) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta6](https://github.com/DragonBox/uplift/tree/v1.0.0beta6) (2017-12-05)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta5...v1.0.0beta6)

**Implemented enhancements:**

- Remove unnecessary warning [\#28](https://github.com/DragonBox/uplift/pull/28) ([niezbop](https://github.com/niezbop))
- Add very basic caching for Upset fetching in GithubRepository [\#27](https://github.com/DragonBox/uplift/pull/27) ([niezbop](https://github.com/niezbop))
- Implement settings editor window [\#26](https://github.com/DragonBox/uplift/pull/26) ([niezbop](https://github.com/niezbop))

**Merged pull requests:**

- Preparing release for 1.0.0beta6 [\#30](https://github.com/DragonBox/uplift/pull/30) ([niezbop](https://github.com/niezbop))
- Cleanup: remove unused usings [\#29](https://github.com/DragonBox/uplift/pull/29) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta5](https://github.com/DragonBox/uplift/tree/v1.0.0beta5) (2017-11-30)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta4...v1.0.0beta5)

**Fixed bugs:**

- Some .unitypackage are not extracted correctly [\#23](https://github.com/DragonBox/uplift/issues/23)
- Readme version not correctly updated when releasing [\#22](https://github.com/DragonBox/uplift/issues/22)
- Log not aggregated on Unity 2017+ from the .unitypackage [\#20](https://github.com/DragonBox/uplift/issues/20)

**Merged pull requests:**

- Preparing release for 1.0.0beta5 [\#25](https://github.com/DragonBox/uplift/pull/25) ([niezbop](https://github.com/niezbop))
- Compiler: use defines to make sure that .unitypackage is built correctly [\#21](https://github.com/DragonBox/uplift/pull/21) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta4](https://github.com/DragonBox/uplift/tree/v1.0.0beta4) (2017-11-28)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta3...v1.0.0beta4)

**Implemented enhancements:**

- Add GitHub repository [\#18](https://github.com/DragonBox/uplift/pull/18) ([niezbop](https://github.com/niezbop))

**Merged pull requests:**

- Preparing release for 1.0.0beta4 [\#19](https://github.com/DragonBox/uplift/pull/19) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta3](https://github.com/DragonBox/uplift/tree/v1.0.0beta3) (2017-11-16)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta2...v1.0.0beta3)

**Merged pull requests:**

- Preparing release for 1.0.0beta3 [\#17](https://github.com/DragonBox/uplift/pull/17) ([niezbop](https://github.com/niezbop))
- Remove dependency on Sharpcompress [\#16](https://github.com/DragonBox/uplift/pull/16) ([niezbop](https://github.com/niezbop))
- Automate build and release of binaries on Github [\#15](https://github.com/DragonBox/uplift/pull/15) ([niezbop](https://github.com/niezbop))

## [v1.0.0beta2](https://github.com/DragonBox/uplift/tree/v1.0.0beta2) (2017-11-14)

[Full Changelog](https://github.com/DragonBox/uplift/compare/v1.0.0beta1...v1.0.0beta2)

**Implemented enhancements:**

- Self update mechanism [\#3](https://github.com/DragonBox/uplift/issues/3)
- Add self-update mechanism \(fixes \#3\) [\#11](https://github.com/DragonBox/uplift/pull/11) ([niezbop](https://github.com/niezbop))
- Keep track of the Upfile.lock MD5 to detect project changes [\#9](https://github.com/DragonBox/uplift/pull/9) ([niezbop](https://github.com/niezbop))
- Refactor FileRepository [\#8](https://github.com/DragonBox/uplift/pull/8) ([niezbop](https://github.com/niezbop))

**Fixed bugs:**

- VersionRequirement: BoundedVersionRequirement and LoseVersionRequirement merge conflict [\#12](https://github.com/DragonBox/uplift/issues/12)
- update window: consistency issues [\#5](https://github.com/DragonBox/uplift/issues/5)

**Merged pull requests:**

- Preparing release for 1.0.0beta2 [\#14](https://github.com/DragonBox/uplift/pull/14) ([niezbop](https://github.com/niezbop))
- Fix Version requirement lose and bounded conflict [\#13](https://github.com/DragonBox/uplift/pull/13) ([niezbop](https://github.com/niezbop))
- Force gitignore to have Unix EOL [\#10](https://github.com/DragonBox/uplift/pull/10) ([lacostej](https://github.com/lacostej))
- Refactor dependency management logic for consistency and separation from UI [\#7](https://github.com/DragonBox/uplift/pull/7) ([niezbop](https://github.com/niezbop))
- Warn user that update window may cause unexpected breakage [\#6](https://github.com/DragonBox/uplift/pull/6) ([niezbop](https://github.com/niezbop))
- CONTRIBUTING document [\#2](https://github.com/DragonBox/uplift/pull/2) ([lacostej](https://github.com/lacostej))
- Rakefile improvement: portability using ruby code [\#1](https://github.com/DragonBox/uplift/pull/1) ([lacostej](https://github.com/lacostej))



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*
