# Downloading with LibVLCSharp

Requires a LibVLC 4 build with the downloader API.

```csharp
using LibVLCSharp;

using var libVLC = new LibVLC();
using var downloader = new MediaDownloader(libVLC);
using var media = new Media(new Uri("https://example.com/video.mp4"));

await downloader.DownloadAsync(media, "video.mp4",
    progress: new Progress<DownloadProgress>(p =>
        Console.WriteLine($"{p.BytesWritten} / {p.TotalBytes} bytes")),
    cancellationToken: cancellationToken);
```

The file overload creates a new file and refuses to overwrite an existing one.
The stream overload writes from the current position and leaves the stream open:

```csharp
await downloader.DownloadAsync(media, outputStream,
    cancellationToken: cancellationToken);
```

Both overloads use bounded, pooled buffers and pause the download when the writer
falls behind. Completion includes all destination writes and the final flush.
Native failures throw `IOException`; cancellation throws `OperationCanceledException`.
Destination exceptions propagate, and partial output is retained on failure or
cancellation. Progress counts bytes written by this download, regardless of the
stream's starting position. Avoid using the destination concurrently until the task
completes. `Progress<T>` dispatches notifications through its captured synchronization
context; those notifications may arrive after the download task completes.

For custom synchronous processing, `Queue` accepts a `DownloadBufferCallback`
receiving a span over the native buffer. No `Marshal.Copy` or unsafe code is needed
in the caller:

```csharp
using var request = downloader.Queue(media, (bytes, position, total) =>
{
    ProcessSynchronously(bytes); // bytes is ReadOnlySpan<byte>.
    return bytes.Length;
});

var status = await request.Completion;
```

The span is valid only during the callback. Copy bytes that must survive it, and
keep callbacks short. Return the number of bytes consumed, `-1` for an error, or
`-2` to cancel. Consuming fewer bytes than supplied automatically pauses the
request; resume with `request.SetPause(false)` from outside the callback.
Call `request.SetPause(true)` or `request.Cancel()` outside callbacks to control an
individual request, or `downloader.CancelAll()` to cancel all active downloads.

Run the CLI sample with:

```text
downloader <URL-or-file> <output-file>
```

Press Ctrl+C to cancel.
