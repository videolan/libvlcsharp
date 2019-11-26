# LibVLC Documentation

[Back](home.md)

- [LibVLC options](#libvlc-options)
- [LibVLC versions and differences](#libvlc-versions-and-differences)

LibVLCSharp is based on LibVLC, so most of the LibVLC API documentation is very relevant to your experience using LibVLCSharp.

You can find it here [LibVLC Doc](https://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html). It currently tracks VLC master (4.0). The search box at the top right allows you to search for LibVLC C API functions and get doc on it. You can also check out documentation from the LibVLC header files directly from the [VLC source code](https://code.videolan.org/videolan/vlc/tree/master/include/vlc).

## LibVLC Options

Some advanced customizations and features are only accessible through LibVLC options. Those take the form of strings that you give to the LibVLC library through the `LibVLC` constructor and/or through `media.AddOption`.

> Note: If using the LibVLC constructor, you need to use prepend your option with `--`, like `--verbose=2`.
> If using the `media.AddOption` API, you need to prepend with `:`, such as `:no-video`.

> Note: Some options only work with the LibVLC constructor and some options only work with the `media.AddOption` API.

The list of such LibVLC string options can be found here https://wiki.videolan.org/VLC_command-line_help/

## LibVLC versions and current status

LibVLC 2.x not supported by VideoLAN, security issues, does not work with LibVLCSharp.

LibVLC 3.x current stable version.

LibVLC 4.x current nightly version.

## Relation to LibVLCSharp versioning

Check out our [versioning docs](../versioning.md).