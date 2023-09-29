# LibVLCSharp.WPF

[![NuGet Stats](https://img.shields.io/nuget/v/LibVLCSharp.WPF.svg)](https://www.nuget.org/packages/LibVLCSharp.WPF)
[![NuGet Stats](https://img.shields.io/nuget/dt/LibVLCSharp.WPF.svg)](https://www.nuget.org/packages/LibVLCSharp.WPF)

LibVLCSharp.WPF is the WPF integration for LibVLCSharp.

It contains the views that allow to display a video played with [LibVLCSharp](../LibVLCSharp/README.md)
in a WPF app.

[LibVLCSharp.Forms.Platforms.WPF](../LibVLCSharp.Forms.Platforms.WPF) depends on this package.

This package depends on [LibVLCSharp](../LibVLCSharp/README.md).

Supported frameworks:

- net461+
- netcoreapp3.0

Supported platform:

- Windows

## Airspace limitations

If you encounter UI issues with the WPF VideoView in your application, you may be running into what is called _airspace_ limitations.

For context and explanations of the tradeoffs, see this [PR](https://github.com/videolan/libvlcsharp/pull/1).
Issues related to airspace are tracked on our GitLab with the [airspace](https://code.videolan.org/videolan/LibVLCSharp/issues?scope=all&utf8=%E2%9C%93&state=all&label_name[]=airspace) tag.

## WPF control specific stuffs

Due to the Airspace issue, you cannot easily draw things over the video in WPF, unless you have a hack like the one that is included in this project.
This hack means that the WPF control works a little differently than other platform's.

If you want to place something over the control, you would probably write code like this in other platforms:

```xml
<Grid>
    <vlc:VideoView x:Name="VideoView" />
    <Button Click="Play_Clicked">PLAY</Button>
</Grid>
```

But for WPF, you would rather need something like this:

```xml
<Grid>
    <vlc:VideoView x:Name="VideoView">
        <Button Click="Play_Clicked">PLAY</Button>
    </vlc:VideoView>
</Grid>
```

The `VideoView` appears as a container in your XAML (you can set its `Content` property from code too), but it is really a detached window over your video control.

The DataContext of the `VideoView` is propagated to your overlay content. This means you can inherit the `DataContext` environment from the outside of your `VideoView`

*Note : This behavior is specific to the LibVLCSharp WPF implementation and is not (yet?) available to LibVLCSharp.Forms.Platforms.WPF*

## WPF transforms support and limitations

Applying layout and render transforms to the `VideoView` or one of its ancestors is partially supported by LibVLCSharp.WPF: 

Translate transforms and uniform scale transforms are fully supported and correctly applied to the video and the overlay content. Using a `Viewbox` with a uniform stretch is also fully supported, as it is analogous to a uniform scale transform.

Non-uniform scale transforms and negative scale factors (mirroring) have a limited support. That also applies to using a `Viewbox` with a non-uniform stretch.

Other transforms (rotate, skew, ...) are currently not supported.

## Why should I reference this package in my project?

If you want to create a video application using WPF and any supported .NET language, this package is made for you.

You can also create a true cross-platform application with Xamarin.Forms, and use the WPF backend.
In that case, you would need the [LibVLCSharp.Forms.Platforms.WPF](../LibVLCSharp.Forms.Platforms.WPF) package instead, which internally references this one.

For other platforms, see the [main documentation](../../README.md).
