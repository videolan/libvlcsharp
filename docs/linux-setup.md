# Getting started on LibVLCSharp.Gtk for Linux

[Back](home.md)

_This procedure was tested on ubuntu 18.10. If you have another distribution, make sure that all requirements are available, and adapt the following commands_

1. Install **[Mono](https://www.mono-project.com/download/stable/#download-lin)**. Make sure you can invoke `msbuild`.
2. Install **libvlc**: 

For ubuntu:
> `apt-get install libvlc-dev`. 

*`libvlc.so` and `libvlccore.so` will be located at `/usr/lib`.*

You may need:
> `apt-get install vlc`

Though note this might pull libvlc 2.x depending on the repository and distro you pull it from.

3. Install **gtk-sharp** (or monodevelop which uses it)

For ubuntu:
> `apt-get install gtk-sharp2`

Should you want to load the libvlc libraries from another location than `/usr/lib`, you'd need to set `LD_LIBRARY_PATH`.