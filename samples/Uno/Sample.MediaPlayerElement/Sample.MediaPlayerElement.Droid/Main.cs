using System;
using Android.App;
using Android.Runtime;
using Windows.UI.Xaml;

namespace Sample.MediaPlayerElement.Droid
{
    [Application(
        Label = "@string/ApplicationName",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/AppTheme"
    )]
    public class Application : NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(new App(), javaReference, transfer)
        {
        }
    }
}
