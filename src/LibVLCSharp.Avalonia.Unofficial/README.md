# LibVLCSharp.Avalonia.Unofficial

**The** **Unofficial Avalonia** **views for** **LibVLCSharp**.

This package contains the views that allows to display a video played with LibVLCSharp in an Avalonia app.

This Unofficial repository fixes the 2 main problems still present in the LibVLCSharp.Avalonia official release: 

- **how do I use VideoView in an Avalonia UserControl** (in official release it works inside a window only)?
- **how do I put a UserControl in a clickable layer on top of the VLC MediaPlayer** (as in official release it's impossible)?

*VideoView.cs* has been modified to handle the fixes and to answer to both questions.

Being an unofficial release, <u>it's still experimental</u> and the VideoView code may need further modification.

The [samples](https://github.com/radiolondra/libvlcsharp/tree/3.x/samples/LibVLCSharp.Avalonia.Unofficial.Samples) folder of libvlcsharp contains the **LibVLCSharp.Avalonia.Unofficial.Samples** to test the fixed problems.
The samples have been successfully tested on Windows 10, Kubuntu 18.04, MacOS 10.13 and on Raspberry pi3 model B with DietPi (META) as OS.
Use them as starting points for your own projects and to learn how the whole thing is working.

#### ToDo
- Add customizable event handlers
- Add the ability to use multiple, configurable layers on top of VideoView, allowing to accomodate a set of new objects with dynamic properties. 