# LibVLCSharp.WPF

[![NuGet Stats](https://img.shields.io/nuget/v/LibVLCSharp.WPF.svg)](https://www.nuget.org/packages/LibVLCSharp.WPF)
[![NuGet Stats](https://img.shields.io/nuget/dt/LibVLCSharp.WPF.svg)](https://www.nuget.org/packages/LibVLCSharp.WPF)

LibVLCSharp.WPF is the WPF integration for LibVLCSharp.

It contains the views that allow to display a video played with [LibVLCSharp](../LibVLCSharp/README.md)
in a WPF app.

[LibVLCSharp.Forms.Platforms.WPF](../LibVLCSharp.Forms.Platforms.WPF) depends on this package.

This package depends on [LibVLCSharp](../LibVLCSharp/README.md).

Supported framework:

- net461+

Supported platform:

- Windows

## Airspace limitations

If you encounter UI issues with the WPF VideoView in your application, you may be running into what is called _airspace_ limitations.

For context and explanations of the tradeoffs, see this [PR](https://github.com/videolan/libvlcsharp/pull/1).
Issues related to airspace are tracked on our GitLab with the [airspace](https://code.videolan.org/videolan/LibVLCSharp/issues?scope=all&utf8=%E2%9C%93&state=all&label_name[]=airspace) tag.

## Why should I reference this package in my project?

If you want to create a video application using WPF and any supported .NET language, this package is made for you.

You can also create a true cross-platform application with Xamarin.Forms, and use the WPF backend.
In that case, you would need the [LibVLCSharp.Forms.Platforms.WPF](../LibVLCSharp.Forms.Platforms.WPF) package instead, which internally references this one.

For other platforms, see the [main documentation](../README.md).
