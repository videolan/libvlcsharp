# LibVLCSharp

[![NuGet Stats](https://img.shields.io/nuget/v/LibVLCSharp.svg)](https://www.nuget.org/packages/LibVLCSharp)
[![NuGet Stats](https://img.shields.io/nuget/dt/LibVLCSharp.svg)](https://www.nuget.org/packages/LibVLCSharp)

The official .NET wrapper around LibVLC.

This package contains the core features of LibVLCSharp (libvlc loading and initialization, .NET-friendly classes to ease the use of libvlc...).
All other `LibVLCSharp.*` packages depend on this one.

This package also contains the views for the following platforms:

- Android
- iOS
- Mac
- tvOS
- UWP

This package has multiple target frameworks, which means it will pick the right features for your project (you will only get the mac view if you are building a mac project).

> BE CAREFUL: This project does not include **LibVLC** itself! You will need to install it separately!
See the [Installation](../../README.md#installation) documentation for more info.

## Why should I reference this package in my project?

If you are in one of these situation, this package is made for you.

- You want to build a console application that leverages the power of VLC for transcoding/streaming/recording/playing audio... without displaying the video anywhere
- You want to build a Xamarin.iOS/Android/Mac/tvOS/UWP app (not Xamarin.Forms, for that, see [LibVLCSharp.Forms](../LibVLCSharp.Forms/README.md) )

For other platforms, see the [main documentation](../../README.md)