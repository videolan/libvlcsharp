# LibVLCSharp Thumbnailer CLI sample

This sample generates ARGB thumbnails for a local video at one or more timestamps and prints the center pixel color for each frame.

The Windows native dependency is `VideoLAN.LibVLC.Windows` `4.0.0-alpha-20260624`, the latest build available from the VideoLAN preview feed when this sample was added.

```powershell
dotnet run --project samples/LibVLCSharp.Thumbnailer.Cli.Sample -- C:\Videos\sample.mp4
dotnet run --project samples/LibVLCSharp.Thumbnailer.Cli.Sample -- C:\Videos\sample.mp4 0 2000 5000 8000
```

When no timestamps are provided, the sample uses `0`, `2000`, `5000`, and `8000` milliseconds.
