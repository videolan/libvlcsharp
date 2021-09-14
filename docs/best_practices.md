# Best practices

This page will detail a set of best practices when using LibVLC/LibVLCSharp

[Back](home.md)

## Only create **one** LibVLC instance at all times

It is recommended by VLC core developers to only create a single instance of type `LibVLC` during your application's lifecycle.
You may create as many `MediaPlayer` objects as you want from a single `LibVLC` object.

## Dispose of libvlc objects when done

Since LibVLCSharp is a binding over native libvlc, LibVLCSharp types implement `IDisposable` which means the GC does not handle the disposal of these types, you do. Always call `Dispose()` on LibVLCSharp types when you're done using them (or use `using`).

see https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/using-objects for more info.

## Do not call LibVLC from a LibVLC event without switching thread first

Doing this
```csharp
mediaPlayer.EndReached += (sender, args) => mediaPlayer.Play(nextMedia);
```

Might freeze your app. 

If you need to call back into LibVLCSharp from an event, you need to switch thread. This is an example of how to do it:

```csharp
mediaPlayer.EndReached += (sender, args) => ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Play(nextMedia);
```

## Generate a plugin cache (LibVLC.Windows only for now)

If you want `Core.Initialize()` or `new LibVLC()` (if you don't call `Core.Initialize` yourself) to be faster, you could generate a plugins cache (if it is not already there and up to date).

Just run your app once with:

```csharp
new LibVLC("--reset-plugins-cache");
```

This will generate a new `plugins.dat` file in your libvlc plugins folder. LibVLC will use this file to get information on the available plugins in advance, reducing dramatically the loading process.

This file should be updated everytime LibVLC is updated (not LibVLCSharp). We will likely ship it in the LibVLC.Windows NuGet in the future, but you can already generate it yourself by simply using the above code once.

## Check how official VLC apps do it

VLC for iOS and VLC for Android are the biggest libvlc consumer out there. They use libvlc just like anyone using LibVLCSharp uses libvlc to make their app.

That means anything that the official VLC apps can do, so can you with LibVLCSharp. Their code source are open.

- https://code.videolan.org/videolan/vlc-android
- https://code.videolan.org/videolan/vlc-ios
