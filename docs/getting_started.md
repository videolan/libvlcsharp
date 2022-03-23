# Requirements

Make sure you have the proper SDK installed for your target platform. The **Xamarin** workloads, the .NET desktop components, the GTK# one, etc. depending on your needs.

[Back](home.md)

# Getting started

Clone https://code.videolan.org/mfkl/libvlcsharp-samples to get started and explore the samples.

The `MinimalPlayback` one provides a simple approach to getting video on the screen quickly.

The steps are:
1. Instantiate a `VideoView` and add it to your main View. 
2. (Optional) Call `Core.Initialize()` to load the `libvlc` native libraries. If you are using LibVLCSharp with the provided LibVLC native nugets, you do not need to make this call yourself (LibVLCSharp handles it for you) but you can. Reasons you'd want to do it yourself, could be to separate the libvlc loading process (which may take a bit of time) and the mediaplayback starting process. Also, if you put your libvlc build in a custom location, you may want to call `Core.Initialize(string path)` yourself with the custom path location.
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
### Context Menu & Mouse-Click Events
If you're trying to create a context menu for your `VideoView` or hook to one of the mouse-click events, you may find that it does not work as expected. This is due to quirk with WPF, for those to fire off there **has** to be a definitively defined background which is *not fully* transparent.

So one quick and easy workaround is to add for example a `Grid` inside the `VideoView` as explained above. We'll be using this `Grid` component as a sort of invisible wall to detect click events. You'd implement it like this:

```xml
<vlc:VideoView x:Name="MyVideoView">
	<Grid x:Name="DummyGrid" Background="#02000000" MouseRightButtonUp="DummyGrid_MouseRightButtonUp">
		<Grid.ContextMenu>
			<ContextMenu>
				<MenuItem Header="Hello World"/>
			</ContextMenu>
		</Grid.ContextMenu>
	</Grid>
</vlc:VideoView>
```

Do note how we're defining a background, which would otherwise be set to `null` by default. Now it has a black background but with a *very* low opacity, so it's still basically transparent. Also be aware that setting the opacity to 0%, in other words `#00000000` will *not* work. _The alpha must be higher than 0._

The context menu will now appear as expected when you right-click the video and the `MouseRightButtonUp` event will fire as expected.

## UWP

These issues are issues of libvlc 3, that we hope to get fixed for libvlc 4

### LibVLC additionnal parameters

tl;dr : If you want to play a video with UWP, integrate the `VideoView` in your control, use XAML Behaviours to invoke a command upon receiving the `Initalized` event and then initialize your player with:
```cs
new LibVLC(eventArgs.SwapChainOptions);
```

Detailed explanation:
In UWP here are mandatory options to be given to the LibVLC constructor. These options tells libvlc (and more precisely the DirectX plugin) where to output the video, using a so-called "swap chain".

This initialization step will disappear in libvlc 4, but for now, you will need to have a code similar to [this](../samples/LibVLCSharp.UWP.Sample/MainPage.xaml) and [this](../samples/LibVLCSharp.UWP.Sample/MainViewModel.cs#L63).


### Automatic audio output module selection

tl;dr : You don't have anything to do. This section explains how things work internally

Detailed explanation:
When you create a `new LibVLC()` in UWP, the `--aout=winstore` is automatically added. This means that you can't specify another `--aout` option in that constructor.

# Documentation

For usage of the API, you should check out the `libvlc` [C API documentation](https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html) which this wrapper follows closely.