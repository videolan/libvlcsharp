# Async Support

[Back](home.md)

## LibVLC Async concept

The LibVLC native library have a set of functions that are largely _asynchronous_.

In an interop scenario, this means that when calling the `Play` function for example, control may return to the caller (e.g. your code calling into LibVLCSharp) from the native function _before_ the playback is actually _started_. How so?

Well, most mediaplayer functions, like `Play`, are merely commands that kick off an asynchronous operation. There is no guarantee that the requested state is satisfied by the time the function returns (or even at all, sometimes).

## LibVLCSharp Async

Using LibVLCSharp APIs which hold the `Async` suffix means LibVLCSharp will make .NET wait asynchronously for the requested change in LibVLC to fully take effect before returning control to the caller (e.g. your code).

This permits using C# language features such as `async/await` transparently when calling into the LibVLC APIs.

## Sync vs Async signature: which dhould you use?

- For most operations, you could use the **synchronous** version if it simplifies your code. Using the API without `Async` in LibVLCSharp does not mean it is **blocking**, it means you issue a command and your code continues running whether or not the command has been satified. In most cases, the command will be satisfied so quickly you will not need/care to listen for the confirmation.
- An exception could be stopping RTSP streams, which may take a while for example. For this specific use case, and only if you are interested in knowning when the playback pipeline is effectively `Stopped`, I would use the `StopAsync` version of the `Stop` version.