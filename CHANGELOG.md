# Change Log

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



\* *This Change Log was automatically generated by [github_changelog_generator](https://github.com/skywinder/Github-Changelog-Generator)*