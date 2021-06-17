# LibVLCSharp.Forms

[![NuGet Stats](https://img.shields.io/nuget/v/LibVLCSharp.Forms.svg)](https://www.nuget.org/packages/LibVLCSharp.Forms)
[![NuGet Stats](https://img.shields.io/nuget/dt/LibVLCSharp.Forms.svg)](https://www.nuget.org/packages/LibVLCSharp.Forms)

This package provides Xamarin.Forms support for LibVLCSharp.

This package also contains the views for the following platforms:

- Android
- iOS
- Mac

[GTK](../LibVLCSharp.Forms.Platforms.GTK/README.md) and [WPF](../LibVLCSharp.Forms.Platforms.WPF/README.md) Forms support are in separate packages due to multi-targeting limitations.

This package has multiple target frameworks, which means it will pick the right features for your project (you will only get the mac view if you are building a mac project).

   BE CAREFUL: This project does not include **LibVLC** itself! You will need to install it separately!
   See the [Installation](../../README.md#installation) documentation for more info.

## MediaPlayerElement

This package includes a Xamarin.Forms MediaPlayerElement component. It currently supports iOS and Android only.

Note: In your iOS project, you must override the `GetSupportedInterfaceOrientations` method in `AppDelegate.cs` to enable the lock screen feature.

```c#
using Foundation;
using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;
using ObjCRuntime;
using UIKit;
// ...

namespace MyAppNameSpace.iOS
{
    
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
       
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            LibVLCSharpFormsRenderer.Init();
            // ...
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, [Transient] UIWindow forWindow)
        {
            return OrientationChangeListener.Subscribe(this);
        }
    }
}

```

See the [sample](../../samples/Forms/LibVLCSharp.Forms.MediaElement) for more info.

## Why should I reference this package in my project?

If you are in this situation, this package is made for you:

- You want to build a Xamarin.Forms application (no matter on which platform) using LibVLCSharp.

For other platforms, see the [main documentation](../../README.md)