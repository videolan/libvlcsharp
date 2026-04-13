using System;
using LibVLCSharp.Shared;

// AOT Compatibility test for LibVLCSharp core.
//
// Build-time trim analysis (no native toolchain needed):
//   dotnet build
//
// Full NativeAOT publish (requires MSVC on Windows, clang on Linux/macOS):
//   dotnet publish -r win-x64 -c Release
//   dotnet publish -r linux-x64 -c Release

// Reference all public types so the linker includes them in analysis.
_ = typeof(LibVLC);
_ = typeof(MediaPlayer);
_ = typeof(Media);
_ = typeof(MediaList);
_ = typeof(MediaDiscoverer);
_ = typeof(RendererDiscoverer);
_ = typeof(Equalizer);
_ = typeof(MediaInput);
_ = typeof(StreamMediaInput);

Console.WriteLine("LibVLCSharp AOT compatibility OK");
