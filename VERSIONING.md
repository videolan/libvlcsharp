# Versioning

_This strategy applies to all LibVLCSharp [nuget packages](https://www.nuget.org/profiles/videolan) (Forms, etc.)._

## Versioning strategy

### We are tied to the libvlc versioning as this binding targets the libvlc public API

### We will follow its major version (i.e. libvlc 3.x -> LibVLCSharp 3.x and so on)
This makes it easier for people to understand and use the wrapper with libvlc, on all platforms.

#### 0.x versions

Initial LibVLCSharp releases were 0.x versions and target libvlc 3. It's getting stable now, though some APIs might still change (notably `VideoView` public API). When we consider it ready, we will move the LibVLCSharp package version to 3.x and 4.x.

## Long Term Support (LTS)

### We will maintain up to 2 major libvlc versions at the same time, using 2 different branches, starting with libvlc 3

So when libvlc 4 gets released, we will maintain both LibVLCSharp 3.x and LibVLCSharp 4.x using the same nuget package id. When libvlc 5 lands, it is likely that we remove support for libvlc 3. We will still accept PRs and contributions from the community for older LibVLCSharp versions.

## Breaking changes

### Since our major version (i.e. [1].0.0) is tied to the libvlc major version, if we need to introduce breaking changes in the wrapper, we will bump the minor version (i.e. 1.[1].0) and reserve the last number (i.e. 1.1.[1]) for improvements and general maintenance (non breaking API changes).