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

### Documentation

For usage of the API, you should check out the `libvlc` [C API documentation](https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html) which this wrapper follows closely.