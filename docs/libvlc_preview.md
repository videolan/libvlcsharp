# LibVLC Preview

[Back](home.md)

LibVLC preview builds are development builds. They can be unstable, but allow you to enjoy new features before the stable release.

**It is advised NOT TO use these builds in production.**

- [Installation](#installation)
- [Source code](#source-code)
- [Stable/Preview differences](#stable-and-preview-differences)
  - [LibVLC](#libvlc)
  - [LibVLCSharp](#libvlcsharp)

# Installation

LibVLCSharp 3 and 4 preview and LibVLC 4 nightly builds are distributed using https://feedz.io/ for now. To pull pre-release versions into your project, use the following `NuGet.config` file:
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="videolan-preview" value="https://f.feedz.io/videolan/preview/nuget/index.json" />
  </packageSources>
</configuration>
```

# Source code

LibVLC: Development happens in the main git repository https://code.videolan.org/videolan/vlc

LibVLCSharp: master branch of https://code.videolan.org/videolan/LibVLCSharp

# Stable and Preview Differences

## LibVLC

Differences between LibVLC stable (3.x) and LibVLC preview (4.x)

release notes https://code.videolan.org/videolan/vlc/-/blob/master/README

## LibVLCSharp

Differences between LibVLCSharp stable (3.x) and LibVLCSharp preview (4.x)

_TODO_
API diffs,
platform support diffs (new/dropped)
- TFM diffs (new/dropped): `net40` was dropped in favor of `net45` in libvlcsharp 4.
- namespace changes: `Shared` was removed.
- unsafe usage -> perf diff, ongoing

https://code.videolan.org/videolan/LibVLCSharp/-/issues?scope=all&utf8=%E2%9C%93&state=opened&milestone_title=LibVLCSharp%204