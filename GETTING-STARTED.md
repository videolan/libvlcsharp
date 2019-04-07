# Requirements

Make sure you have the proper SDK installed for your target platform. The **Xamarin** workloads, the .NET desktop components, the GTK# one, etc. depending on your needs.

# Getting started

Clone https://code.videolan.org/mfkl/libvlcsharp-samples to get started and explore the samples.

The `MinimalPlayback` one provides a simple approach to getting video on the screen quickly.

The steps are:
1. Instantiate a `VideoView` and add it to your main View. 
2. Call `Core.Initialize()` to load the `libvlc` native libraries, depending on your host platform.
3. The `VideoView` offers a `MediaPlayer` object (with data-binding support) which you should create and set on the `VideoView`. The `MediaPlayer` allows you to control playback with APIs such as `Play`, `Pause`, set a new media or listen for playback events.
4. In case you are using `LibVLCSharp.Forms`, make sure to call `LibVLCSharpFormsRenderer.Init()` in your platform specific project [*before*](https://forums.xamarin.com/discussion/comment/57605/#Comment_57605) `Xamarin.Forms.Forms.Init` is called. See the [Forms sample](https://github.com/videolan/libvlcsharp/tree/master/Samples/Forms).

# Platform-specific caveats

When building LibVLCSharp, we tried really hard to provide a unified experience to all developpers.
Despite our efforts, there are some cases where we had no choice but to make some hacks.

Sometimes, the hacks are hidden into LibVLCSharp's code, but sometimes, you'll need to write extra code.

## WPF
### The airspace issue
*This issue also occurs in LVS.Forms.WPF, but no mitigation technique has been implemented yet.*

tl;dr : If you need to put content over your video, place it inside the control, like this:
```xml
    <lvs:VideoView>
        <Grid>
            <TextBlock>This content will be rendered over the video</TextBlock>
        </Grid>
    </lvs:VideoView>
```
It will be rendered in a separate transparent window which is rendered over the video view.

Detailed explanation:
To display a Video in WPF, you don't have many solutions:
- Use `D3DImage` : This seems like a good idea to use DirectX to accelerate the rendering on screen. However, libvlc doesn't expose a way to use DirectX9 from an application, and even if it did, it wouldn't work. D3DImage tells you when it's ready for rendering, but libvlc has its own clock and wants to "push" content to the view.
- Use an `InteropBitmap`, as [Vlc.DotNet.Wpf](https://github.com/ZeBobo5/Vlc.DotNet/). With this, you have a real WPF `ImageSource` that you can use to make nice things in WPF (Blur effects, transformations...). However, copying a decoded image from libvlc to the .net buffer, and displaying it many times per second is really slow and uses too much CPU, especially for 4K videos...
- Use a `WindowsFormsHost`, which creates a Win32 window. The nice thing is that libvlc can use a Win32 window directly and use DirectX to draw the video on it directly, using the hardware acceleration.

We decided to implement the WPF control with the third solution. However, integrating a `WindowsFormsHost` inside a WPF application creates a window over the WPF content, meaning that the WPF content you write will never be above the WinForms content, here, the video.

This is a problem for many people because they want to display the play/pause controls over the video. This is why LibVLCSharp.WPF comes with its own wrapper to help users to mitigate this issue.

The idea is simple: since WinForms is a window over the WPF content, let's create another window over the WinForms window.
The content you place inside the `VideoView` is used for the window, like this:

```xml
    <lvs:VideoView>
        <Grid>
            <TextBlock>This content will be rendered over the video</TextBlock>
        </Grid>
    </lvs:VideoView>
```

# Documentation

For usage of the API, you should check out the `libvlc` [C API documentation](https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html) which this wrapper follows closely.