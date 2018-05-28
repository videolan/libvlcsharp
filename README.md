<h3 align="center">
    <img src="Assets/banner.png"/>
</h3>

# LibVLCSharp

LibVLCSharp are .NET bindings for `libvlc`, the multimedia framework powering the VLC applications.

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

## Installation

You need to 2 packages to get started.

The first is `libvlc`, which is the actual VLC engine written mostly in C/C++ compiled for your target platform. You can find information about it and how to download it on NuGet [here](https://github.com/mfkl/libvlc-nuget).

The second package you need is LibVLCSharp, the .NET wrapper that consumes `libvlc` and allows you to interact with native code from C#/F#. Get it from NuGet: https://www.nuget.org/packages/LibVLCSharp.

## Getting started

Feel free to check out the sample projects for [iOS](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp.iOS.Sample/ViewController.cs) and [Android](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp.Android.Sample/MainActivity.cs) to get started. 

Basically, you need to instantiate a `VideoView` and add it to your View. It handles the required `libvlc` initialization for you on each platform, and offers a `MediaPlayer` .NET object on which you can call `Play`, `Pause`, set a new media or listen for `libvlc` events.

For usage of the API, you should check out the `libvlc` [C API documentation](https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html) which this wrapper follows closely.

### Quick overview

- `VideoView.cs`: Custom view which holds an `Instance` and a `MediaPlayer`.
- [`Instance.cs`](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp/Shared/Instance.cs): Main object pointing to a native `libvlc` instance in native code. Most other objects are related to this (in case you have multiple players in one app, for example). Accessible from `VideoView`.
- [`MediaPlayer.cs`](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp/Shared/MediaPlayer.cs): Manages playback, offers event listeners and more.
- [`MediaDiscoverer.cs`](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp/Shared/MediaDiscoverer.cs): This object should be used to find media on NAS and any SMB/UPnP-enabled device on your local network.
- [`RendererDiscoverer.cs`](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp/Shared/RendererDiscoverer.cs): Use this to find and use a Chromecast or other distant renderers.
- [`Media.cs`](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp/Shared/Media.cs): Class representing and providing information about a media such as a video or audio file or stream.
- [`Dialog.cs`](https://github.com/mfkl/LibVLCSharp/blob/master/LibVLCSharp/Shared/Dialog.cs): Dialogs can be raised from the `libvlc` engine in some cases. Register callbacks with this object.

#### Currently supported platforms (with working sample):
- Xamarin.Android
- Xamarin.iOS
- Xamarin.Forms

#### Roadmap:
- macOS
- Linux (GTK)
- UWP
- Tizen
- Unity

All contributions are welcome.
