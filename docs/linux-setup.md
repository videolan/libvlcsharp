# Getting started on LibVLCSharp for Linux

[Back](home.md)

_This procedure was tested on ubuntu 20.10. If you have another distribution, make sure that all requirements are available, and adapt the following commands_

1. Install the [.net core SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux)
2. Install **libvlc**: 

For ubuntu:
> `sudo apt install libvlc-dev`. 

*`libvlc.so` and `libvlccore.so` will be located at `/usr/lib`.*
Should you want to load the libvlc libraries from another location than `/usr/lib`, you'd need to set `LD_LIBRARY_PATH`.

You may need:
> `sudo apt install vlc`

## For [gtk-sharp](https://github.com/mono/gtk-sharp)

Install **gtk-sharp** (or monodevelop which uses it)

For ubuntu:
> `sudo apt install gtk-sharp2`

## For other platforms

If your application doesn't find `libX11.so`, you may need to install the `libx11-dev` package :
> `sudo apt install libx11-dev`