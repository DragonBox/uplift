Potential tasks, objectives and improvements
============================================

# Better dependency solving algorithm

The dependency solving algorithm is a NP-complete issue, but our current solving algorithm is not able to solve such a problem. It relies on the assumption that package dependencies do not evolve much in time, which is not a valid assumption, and this should be tackled ASAP.

# Direct git dependency

We should implement a way to support direct git dependencies such as Ruby's Bundler does with:

```ruby
gem 'some_gem', git: 'https://some.domain.example/some_gem/'
```

It would allow for a much smoother package development cycle without having to release many "development" versions before proving stability.

Note that this feature can be rather rich (support for several packages in the same repo, specifying a reference, a branch or a tag...) and could/should be implemented incrementally.

# Better repositories

Currently the Repository interface is a bit too heavy, and does not scale well at all because the repository has to list all of its packages. A better implementation would be along the lines of the following:

```csharp
public interface IRepository
{
    bool HasPackage(PackageSpecification specification);
    TemporaryDirectory GetPackage(PackageSpecification specification);
}
```

Which would scale much better, and would allow for a more flexible repository system.

# Overall drop of the UpliftManager

The UpliftManager is huge bloated singleton which has most of the feature going through it. It should be re-designed in smaller, better defined responsibilities.

# Better Update window(s)

Both update windows (Update Uplift/Update dependencies) are quite frankly ugly and could use a redesign. The Update Dependencies windows especially.

# [IDEA] Taking advantage of the Unity asset processing

Unity knows when file are modified under `Assets/`, so moving the `Upfile.xml` directly at the root of the project could mean that we can take advantage of its asset processing to detect changes to the Upfile and act accordingly (either automatically install dependencies or notify the user that a dependency changed).
 