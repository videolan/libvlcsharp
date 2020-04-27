# LibVLCSharp.GTK

[![NuGet Stats](https://img.shields.io/nuget/v/LibVLCSharp.GTK.svg)](https://www.nuget.org/packages/LibVLCSharp.GTK)
[![NuGet Stats](https://img.shields.io/nuget/dt/LibVLCSharp.GTK.svg)](https://www.nuget.org/packages/LibVLCSharp.GTK)

The official [GTK#](https://github.com/mono/gtk-sharp) views for [LibVLCSharp](../LibVLCSharp/README.md).

This package contains the views that allows to display a video played with [LibVLCSharp](../LibVLCSharp/README.md)
in a GTK# app (the mono/gtk-sharp one, [see below](#note-on-gtk-sharp)).

[LibVLCSharp.Forms.Platforms.GTK](../LibVLCSharp.Forms.Platforms.GTK) depends on this package.

This package depends on [LibVLCSharp](../LibVLCSharp/README.md).

Supported frameworks:

- net47 (on mono)

Supported platforms:

- Windows
- Linux

NOTE: This package does not currently support macOS! See [this issue](https://code.videolan.org/videolan/LibVLCSharp/issues/92)

WARNING: To create a GTK# program and run the samples, you will need mono and GTK# installed on the machine you build AND
on the machine you run.

## <a name="note-on-gtk-sharp"></a> A note on mono/gtk-sharp vs GtkSharp/GtkSharp

Let's face it, mono/gtk-sharp has a lot of drawbacks:

- It is based on GTK 2 (GTK 3 was released in 2011... but no stable version of GTK# for GTK3 as of now)
- Requires mono and GTK# installed on both the build and the target machine. (aka no nuget package)

On the other hand, [GtkSharp/GtkSharp](https://github.com/GtkSharp/GtkSharp) fixes this issues by providing a
.NET Standard 2.0 package for GTK 3.0 and NuGet packages for the GTK# libraries.

Unfortunately, Xamarin.Forms.GTK is only implemented using the former, which means that in order to support Xamarin.Forms.GTK, our views needs to support mono/gtk-sharp, hence this package.

## Why should I reference this package in my project?

If you want to create a video application using GTK# and C# with mono, this package is made for you.

You can also create a true cross-platform application with Xamarin.Forms, and use the GTK# backend to target linux and windows.
In that case, you would need the [LibVLCSharp.Forms.Platforms.GTK](../LibVLCSharp.Forms.Platforms.GTK) package instead,
which internally references this one.

For other platforms, see the [main documentation](../../README.md)