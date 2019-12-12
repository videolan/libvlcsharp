# How Do I do X?

This document contains a bunch of Q&A from users that asked how to do a specific operation with LibVLC or LibVLCSharp.

As a general rule, it's a good idea to check how to perform a given operation on official VLC app (iOS, Android, Windows, etc.). From there, logs can give you information, and looking at the code as well.

[Back](home.md)

## How do I play a DVD?

Create your media with `dvd:///` as per the [documentation](https://wiki.videolan.org/VLC_command-line_help/).

## How do I cast my media to my Chromecast?

Check out our [Chromecast sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/Chromecast)

## How do I browse and play media on my local network (NAS/UPnP)?

Check out our [LocalNetwork sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/LocalNetwork)

## How do I record a media to the file system?

Check out our [RecordHLS sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/RecordHLS)

## How do I create a mosaic of views for videos?

Check out our [VideoMosaic sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/VideoMosaic)

## How do I navigate a 360 video and change viewpoint?

Check out our [Gestures sample](https://code.videolan.org/mfkl/libvlcsharp-samples/tree/master/Gestures)

## How do I get codec information about a media?

Inspect the Video and Audio tracks objects with `Media.Tracks`.

## How do I get libvlc logs?

Subscribe to the `LibVLC.Log` event.

## How do I play media with RTSP or HLS protocols?

Like any other media. [Here](https://www.videolan.org/vlc/features.html) is a non exhaustive list of protocols we support.

## How do I get individual frames out of a video?

You may want to have a look at the WPF control of Vlc.DotNet which does exactly that: https://github.com/ZeBobo5/Vlc.DotNet/blob/develop/src/Vlc.DotNet.Wpf/VlcVideoSourceProvider.cs

From libvlc, that means libvlc_video_set_callbacks and libvlc_video_set_format_callbacks.

## How do I do transcoding?

Pretty similarly to how you would do it from the CLI. Read https://wiki.videolan.org/Transcode/ and try media options with `Media.AddOption`.

## How do I enable/disable hardware decoding?

```csharp
MediaPlayer.EnableHardwareDecoding = true
```

## How do I play a YouTube video?

Like this, for example:

```csharp
Core.Initialize();

using(var libVLC = new LibVLC())
{
    var media = new Media(libVLC, "https://www.youtube.com/watch?v=dQw4w9WgXcQ", FromType.FromLocation);
    await media.Parse(MediaParseOptions.ParseNetwork);
    using (var mp = new MediaPlayer(media.SubItems.First()))
    {
            var r = mp.Play();
            Console.ReadKey();
    }
}
```

# Subtitles

Full list commands and arguments https://wiki.videolan.org/VLC_command-line_help/

## How do I set subtitles?

```csharp
Core.Initialize();

using(var libVLC = new LibVLC())
{
    var media = new Media(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", FromType.FromLocation);
    using (var mp = new MediaPlayer(media))
    {
            mp.AddSlave(MediaSlaveType.Subtitle, "file:///C:\\Users\\Me\\Desktop\\subs.srt", true);
            var r = mp.Play();
            Console.ReadKey();
    }
}
```

## How do I change the subtitle font size?

```csharp
media.AddOption(":freetype-rel-fontsize=10"); // Usually 10, 13, 16 or 19
```

## How do I change the subtitle color?

```csharp
media.AddOption(":freetype-color=16711680"); // Red
```

## How do I change the subtitle encoding?

```csharp
media.AddOption(":subsdec-encoding=Windows-125");
```

# How do I change aspect ratio?

If you are using the `MediaPlayerElement`, then it is built-in in the UI.

If not, you should play with the `Scale` and `AspectRatio` MediaPlayer properties.

See https://code.videolan.org/videolan/LibVLCSharp/blob/3.x/LibVLCSharp/Shared/MediaPlayerElement/AspectRatioManager.cs#L130

# How do I change the movie play rate?

```csharp
MediaPlayer.SetRate(float rate)
```

# How do I turn deinterlacing on/off?

# How do I take a snapshot of the video?

```csharp
MediaPlayer.TakeSnapshot(uint num, string? filePath, uint width, uint height)
```

# How do I enable loopback playback?
