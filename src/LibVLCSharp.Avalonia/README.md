# LibVLCSharp.Avalonia

The official [Avalonia](https://github.com/AvaloniaUI/Avalonia) views for [LibVLCSharp](../LibVLCSharp/README.md).

This package contains the views that allows to display a video played with [LibVLCSharp](../LibVLCSharp/README.md)
in an Avalonia app.

This package depends on [LibVLCSharp](../LibVLCSharp/README.md) as well as [Avalonia](https://github.com/AvaloniaUI/Avalonia).

Supported frameworks:

- netstandard2.0

Supported platforms:

- Windows
- MacOS
- Linux

## Airspace limitations

If you encounter UI issues with the Avalonia VideoView in your application, you may be running into what is called _airspace_ limitations.

For context and explanations of the tradeoffs, see this [PR](https://github.com/videolan/libvlcsharp/pull/1). This is about WPF, but the same applies to Avalonia because they are really close in terms of architecture.
Issues related to airspace are tracked on our GitLab with the [airspace](https://code.videolan.org/videolan/LibVLCSharp/issues?scope=all&utf8=%E2%9C%93&state=all&label_name[]=airspace) tag.

## Avalonia control specific stuffs

Due to the Airspace issue, you cannot easily draw things over the video, unless you have a hack like the one that is included in this project.
This hack means that the Avalonia control works a little differently than other platform's.

If you want to place something over the control, you would probably write code like this in other platforms:

```xml
<Grid>
    <vlc:VideoView x:Name="VideoView" />
    <Button Click="Play_Clicked">PLAY</Button>
</Grid>
```

But for Avalonia, you would rather need something like this:

```xml
<Grid>
    <vlc:VideoView x:Name="VideoView">
        <Button Click="Play_Clicked">PLAY</Button>
    </vlc:VideoView>
</Grid>
```

The `VideoView` appears as a container in your XAML (you can set its `Content` property from code too), but it is really a detached window over your video control.

The DataContext of the `VideoView` is propagated to your overlay content. This means you can inherit the `DataContext` environment from the outside of your `VideoView`

*Note : This behavior is specific to the LibVLCSharp Avalonia and WPF implementations
