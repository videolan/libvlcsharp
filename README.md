<h3 align="center">
    <img src="Assets/banner.png"/>
</h3>

# LibVLCSharp

LibVLCSharp are .NET/Mono bindings for `libvlc`, the multimedia framework powering the VLC applications.

_The official repository URL for this repo is https://code.videolan.org/videolan/LibVLCSharp._

- [Goal](#goal)
- [Features](#features)
- [Supported platforms](#supported-platforms)
- [Installation](#installation)
- [Getting started](#getting-started)
- [Quick API overview](#quick-api-overview)
- [Roadmap](#roadmap)
- [Contribute](#contribute)
- [Communication](#communication)
    - [Forum](#forum)
    - [Issues](#issues)
    - [IRC](#irc)
- [Code of Conduct](#code-of-conduct)
- [License](#license)

## Goal

LibVLCSharp's goal is to support all .NET runtimes (Xamarin/Mono, .NET Core and .NET Framework) on most operating systems by targeting .NET Standard 2.0.

We also aim to provide you with a custom video control integrated with the OS native UI toolkit. That means integration with UWP, Cocoa (Xamarin.Mac), GTK# and game engines with Mono support (Unity, Unreal, Godot). For a current status, see [Supported platforms](#supported-platforms) and [Roadmap](#roadmap).

`libvlc` is a complete, opensource and crossplatform multimedia framework written in C. On the other hand, Xamarin allows true crossplatform .NET code on all platforms and provides an efficient way to build crossplatform UIs with Xamarin.Forms.

LibVLCSharp is designed to be the connecting layer in between `libvlc` and Xamarin.

Using LibVLCSharp means you can take advantage of all `libvlc` features from shared managed code (C#/F#), in a true crossplatform way. You may use the features described below on all supported platforms by LibVLCSharp (Android, iOS, WPF, XForms for now, more coming soon).

## Features

Check out [libvlc-nuget](https://github.com/mfkl/libvlc-nuget) to get a basic understanding of how `libvlc` works, what it can offer and how to install it with NuGet. 

Some of the features include:

- Network browsing for distant filesystems (SMB, FTP, SFTP, NFS...).
- HDMI passthrough for Audio HD codecs, like E-AC3, TrueHD or DTS-HD.
- Stream to distant renderers, like Chromecast.
- 360 video and 3D audio playback with viewpoint change.
- Support for Ambisonics audio and more than 8 audio channels.
- Subtitles size modification live.
- Hardware decoding and display on all platforms.
- DVD playback and menu navigation.

Most things you can achieve with the regular VLC desktop app, you can also achieve using `libvlc`.

## Supported platforms

- Xamarin.Android
- Xamarin.iOS
- Xamarin.Mac
- Xamarin.Forms
- Windows (WPF)

## Installation

You need to install 2 packages to get started.

The first is `libvlc`, which is the actual VLC engine written mostly in C/C++ and compiled for your target platform. You can find information about it and how to download it on NuGet [here](https://github.com/mfkl/libvlc-nuget).

The second package you need is LibVLCSharp, the .NET wrapper that consumes `libvlc` and allows you to interact with native code from C#/F#. 

```cmd
dotnet add package LibVLCSharp
```

https://www.nuget.org/packages/LibVLCSharp

If you plan to use LibVLCSharp with Xamarin.Forms, you need to install LibVLCSharp.Forms instead (which references LibVLCSharp).

```cmd
dotnet add package LibVLCSharp.Forms
```

https://www.nuget.org/packages/LibVLCSharp.Forms/

## Getting started

Feel free to check out the native sample projects for [iOS](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp.iOS.Sample/ViewController.cs) and [Android](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp.Android.Sample/MainActivity.cs) to get started. 

Basically, you need to instantiate a `VideoView` and add it to your View. It handles the required `libvlc` initialization for you on each platform, and offers a `MediaPlayer` .NET object on which you can call `Play`, `Pause`, set a new media or listen for `libvlc` events.

For usage of the API, you should check out the `libvlc` [C API documentation](https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html) which this wrapper follows closely.

Regarding LibVLCSharp.Forms, check out the sample for [Forms](https://github.com/videolan/libvlcsharp/tree/master/LibVLCSharp.Forms.Sample) to get started.
Notably, make sure to call `LibVLCSharpFormsRenderer.Init()` in your platform specific project [*before*](https://forums.xamarin.com/discussion/comment/57605/#Comment_57605) `Xamarin.Forms.Forms.Init` is called.

## Quick API overview

- `VideoView.cs`: Custom view which holds a `LibVLC` object and a `MediaPlayer` object.
- [`LibVLC.cs`](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp/Shared/LibVLC.cs): Main object pointing to a native `libvlc` instance in native code. Accessible from `VideoView`.
- [`MediaPlayer.cs`](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp/Shared/MediaPlayer.cs): Manages playback, offers event listeners and more. Accessible from `VideoView`.
- [`MediaDiscoverer.cs`](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp/Shared/MediaDiscoverer.cs): This object should be used to find media on NAS and any SMB/UPnP-enabled device on your local network.
- [`RendererDiscoverer.cs`](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp/Shared/RendererDiscoverer.cs): Use this to find and use a Chromecast or other distant renderers.
- [`Media.cs`](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp/Shared/Media.cs): Class representing and providing information about a media such as a video or audio file or stream.
- [`Dialog.cs`](https://github.com/videolan/libvlcsharp/blob/master/LibVLCSharp/Shared/Dialog.cs): Dialogs can be raised from the `libvlc` engine in some cases. Register callbacks with this object.

## Roadmap

- macOS (using GTK)
- Linux (using GTK)
- Windows 10 (using UWP, GTK)
- Game engines (Unity, Unreal, Godot)

If you have a request or question regarding the roadmap, feel free to open an [issue](https://code.videolan.org/videolan/LibVLCSharp/issues) or [PR](https://github.com/videolan/libvlcsharp/pulls).

## Contribute

### Pull request

Pull requests are more than welcome! If you do submit one, please make sure to use a descriptive title and description.

### Gitlab issues

You can look through issues we currently have on the [VideoLAN Gitlab](https://code.videolan.org/videolan/LibVLCSharp).

## Communication

### Forum

If you have any question or if you're not sure it's actually an issue, please visit our [forum](https://forum.videolan.org/).

### Issues

You have encountered an issue and wish to report it to the VLC dev team?

You can create one on our [Gitlab](https://code.videolan.org/videolan/LibVLCSharp/issues) or on our [bug tracker](https://trac.videolan.org/vlc/).

Before creating an issue or ticket, please double check for duplicates!

### IRC

Want to quickly get in touch with us for a question, or even just to talk?

You will always find someone from the VLC team on IRC, __#videolan__ channel on the freenode network.

If you don't have an IRC client, you can always use the [freenode webchat](https://webchat.freenode.net/).

## Code of Conduct

Please read and follow the [VideoLAN CoC](https://wiki.videolan.org/Code_of_Conduct/) license.

## License

LibVLCSharp is under the LGPLv2.1.