# Best practices

This page will detail a set of best practices when using LibVLC/LibVLCSharp

[Back](home.md)

# Debugging with libvlc logs

To enable better understand what might be going on inside libvlc during playback, verbose logs are helpful. To enable them, use this argument when creating your `LibVLC` object:

```csharp
new LibVLC("--verbose=2");
```

# Dispose of libvlc objects when done

Since LibVLCSharp is a binding over native libvlc, LibVLCSharp types implement `IDisposable` which means the GC does not handle the disposal of these types, you do. Always call `Dispose()` on LibVLCSharp types when you're done using them (or use `using`).

see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/using-objects for more info.

# Check how official VLC apps do it

VLC for iOS and VLC for Android are the biggest libvlc consumer out there. They use libvlc just like anyone using LibVLCSharp uses libvlc to make their app.

That means anything that the official VLC apps can do, so can you with LibVLCSharp. Their code source are open.

- https://code.videolan.org/videolan/vlc-android
- https://code.videolan.org/videolan/vlc-ios
