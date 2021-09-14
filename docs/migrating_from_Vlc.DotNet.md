# Migrating from Vlc.DotNet

[Back](home.md)

- [Context](#context)
- [Platform support](#platform-support)
- [API differences](#api-differences)
- [LibVLC nuget vs VLC local installation](#LibVLC-nuget-vs-VLC-local-installation)
- [WPF control intricacies](#wpf-control-intricacies)
- [More documentation links](#documentation)

Feel free to join us on [![Join the chat at https://discord.gg/3h3K3JF](https://img.shields.io/discord/716939396464508958?label=discord)](https://discord.gg/3h3K3JF)

## Context

VideoLAN never created an official .NET libvlc binding so some dotnet folks created several over the years, some more complete and maintained than others. One of the main one is [Vlc.DotNet](https://github.com/ZeBobo5/Vlc.DotNet).

The first commit by [ZeBobo5](https://github.com/ZeBobo5) of [Vlc.DotNet](https://github.com/ZeBobo5/Vlc.DotNet) dates back to march 2009, though it was not the first version.

Later, [Jérémy](https://github.com/jeremyVignelles) became the de facto maintainer of Vlc.DotNet as ZeBobo5 had less time to spend on the project.

Mid 2018, VideoLAN started looking into .NET libvlc bindings. It was decided to create a new project, [LibVLCSharp](https://code.videolan.org/videolan/LibVLCSharp), instead of starting from Vlc.DotNet because we wanted to have a similar public API than [libvlcpp](https://code.videolan.org/videolan/libvlcpp) and design it to be crossplatform from scratch.

## Platform support

### Vlc.DotNet

Vlc.DotNet is designed to work on Windows and Windows only. 

> It provides support down to .NET Framework 3.5 (released in 2007) up to the latest versions of .NET Framework, as well as .NET Core 3+. It offers .NET Standard 1.3 builds as well.

Vlc.DotNet provides View integrations for WPF as well as WinForms.

### LibVLCSharp

LibVLCSharp is designed to be fully crossplatform. Focus was initially mainly on iOS and Android, but desktop integration has become a primary focus as well.

> 
    Xamarin.Android
    Xamarin.iOS
    Xamarin.tvOS
    Xamarin.Mac (Cocoa)
    Windows Classic (WPF, WinForms, GTK)
    Windows Universal (UWP for Desktop, mobile and Xbox)
    Linux including desktop, server and Raspberry Pi (GTK)
    Xamarin.Forms
    Uno Platform (UWP, Android, iOS)
    .NET Standard 1.1 and 2.0
    .NET Framework 4.0 and later
    .NET Core (including ASP.NET Core)
    Unity3D
        Windows Classic

## API differences

Both being based off of the same libvlc APIs, the public C# APIs are quite similar. When in doubt, always inspect the bindings code to see which libvlc C function the C# API binds to.

At the time of writing, both Vlc.DotNet and LibVLCSharp support libvlc 3.x, but only LibVLCSharp has started implementing libvlc 4 features.

For loading LibVLC, unlike with Vlc.DotNet, it is done automatically. If you prefer to do it yourself, you may just call `Core.Initialize()`. The path will be found automatically, provided you have installed the LibVLC Windows nuget (see section below), or you can provide it as a string parameter to `Initialize()`.

Short example of API differences

### Vlc.DotNet

```csharp
using System;
using System.IO;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            var options = new string[]
            {
                // VLC options can be given here. Please refer to the VLC command line documentation.
            };

            var mediaPlayer = new Vlc.DotNet.Core.VlcMediaPlayer(libDirectory);

            var mediaOptions = new string[]
            {
                ":sout=#file{dst="+Path.Combine(Environment.CurrentDirectory, "output.mov")+"}",
                ":sout-keep"
            };

            mediaPlayer.SetMedia(new Uri("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_h264.mov"), mediaOptions);

            bool playFinished = false;
            mediaPlayer.PositionChanged += (sender, e) =>
            {
                Console.Write("\r" + Math.Floor(e.NewPosition * 100) + "%");
            };

            mediaPlayer.EncounteredError += (sender, e) =>
            {
                Console.Error.Write("An error occurred");
                playFinished = true;
            };

            mediaPlayer.EndReached += (sender, e) => {
                playFinished = true;
            };

            mediaPlayer.Play();

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }
    }
}
```

### LibVLCSharp

```csharp
using System;
using System.IO;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var libvlc = new LibVLC();
            using var mediaPlayer = new MediaPlayer(libvlc);

            var mediaOptions = new string[]
            {
                ":sout=#file{dst="+Path.Combine(Environment.CurrentDirectory, "output.mov")+"}",
                ":sout-keep"
            };

            mediaPlayer.Media = new Media(new Uri("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_h264.mov"), mediaOptions);

            bool playFinished = false;
            mediaPlayer.PositionChanged += (sender, e) =>
            {
                Console.Write("\r" + Math.Floor(e.Position * 100) + "%");
            };

            mediaPlayer.EncounteredError += (sender, e) =>
            {
                Console.Error.Write("An error occurred");
                playFinished = true;
            };

            mediaPlayer.EndReached += (sender, e) => {
                playFinished = true;
            };

            mediaPlayer.Play();

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }
    }
}
```

## LibVLC nuget vs VLC local installation

To use any libvlc bindings, you need to first load libvlc in your app and to do so, you need to know the location of the library.

In the past, it was commonly done to use the local VLC desktop app installation directory from your .NET app. It does contain all the necessary files and DLLs, after all.

However it is not really a good idea for several reasons. One, you may want to update your VLC app and not the libvlc build used by your .NET app. Making inter-dependent is not a good idea and there is no reason to. Another reason not to do that is you might want full control over your deployment (i.e. which libvlc build your client uses, and how to update).

For those reasons and more, we created the [VideoLAN NuGet account](https://www.nuget.org/profiles/videolan). NuGet is the package manager for dotnet. Now you can just install any LibVLC version with just a single command. For LibVLC Windows version 3.0.10, type:
```
dotnet add package VideoLAN.LibVLC.Windows --version 3.0.10
```

LibVLCSharp and the WPF/WinForms integrations are also distributed through the VideoLAN NuGet account.

## WPF control intricacies

Proper WPF support for libvlc is not straightforward. That is mainly because of WPF shortcomings. Support for WPF can be implemented in several ways, each having their own upsides and downsides. Vlc.DotNet and LibVLCSharp took different paths regarding this matter.

The short version is that the Vlc.DotNet implementation provides better integration with the WPF ecosystem as it is a _true_ WPF control (transparency, animations) at the cost of poorer performance because of CPU copies in the implementation.

LibVLCSharp enjoys greater performance, thanks to using the HWND API but suffers from the infamous airspace issue. Workarounds have been made to limit these shortcomings and the result is quite useable.

For more context, previous discussions on the LibVLCSharp project can be found [here](https://code.videolan.org/videolan/LibVLCSharp/-/issues?scope=all&utf8=%E2%9C%93&state=all&label_name[]=airspace).

## Documentation

I strongly encourage you to have a look at the available [minimal samples](../samples) and [documentation](home.md). 

For more advanced samples, have a look at [libvlcsharp-samples](https://code.videolan.org/mfkl/libvlcsharp-samples) with apps such as:

- [Chromecast sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/Chromecast): Discover chromecasts on your local network and select one for playback in 100% shared code (Xamarin.Forms, iOS/Android).
- [Record HLS sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/RecordHLS): Simple .NET Core CLI app which shows how to record an HLS stream to the filesystem.
- [RTSP Mosaic sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/VideoMosaic): Cross-platform RTSP player sample with 4 views mosaic (Xamarin.Forms, iOS/Android).
- [PulseMusic UX sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/PulseMusic): A stylish music player UX example using Skia on iOS and Android.
- [Gestures sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/Gestures/Gestures): Cross-platform touch gestures and 360 videos on iOS and Android.
- [LocalNetwork sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/LocalNetwork): Cross-platform local network browsing and playback with network shares (SMB, UPnP) on Android, iOS and WPF.
- [MediaPlayerElement sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/MediaElement): Minimal iOS/Android sample showing how to get started with the crossplatform MediaPlayerElement control from LibVLCSharp.Forms.
- [SkiaSharp Preview Thumbnailer sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/PreviewThumbnailExtractor.Skia): .NET Core sample app showing how to use LibVLC 3 video callbacks and encode the frame with SkiaSharp before saving it to disk.
- [lvst](https://github.com/mfkl/lvst): Watch a movie while it is downloading! lvst is a .NET Core CLI app using MonoTorrent and LibVLCSharp for macOS, Linux and Windows.

Feel free to send a PR if you would like to improve the samples!
