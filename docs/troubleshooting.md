# Troubleshooting issues with LibVLC/LibVLCSharp

A good first step is to try to identify whether your issue is originating from the **core LibVLC library** or from the **C# bindings** code. This is not always straightforward to do.

## Debugging with libvlc logs

To better understand what might be going on inside libvlc during playback in your application, verbose logs are helpful. To enable them, use this argument when creating your `LibVLC` object:

```csharp
new LibVLC("--verbose=2");
```

## Try your code on multiple devices and platforms

Some issues are platform specific, some are even device specific, or driver specific. Luckily LibVLCSharp runs on many platforms, so testing on various OSes is one way to try and pinpoint the original issue.

## Reproduce in official VLC apps, desktop and mobile

Enable debug logs in the official VLC apps and compare the logs with those from your application.