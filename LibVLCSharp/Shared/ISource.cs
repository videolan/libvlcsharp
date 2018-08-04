using System;

namespace LibVLCSharp.Shared
{
    public interface ISource
    {
#if WINDOWS
            IntPtr Hwnd { set; }
#elif ANDROID
            void SetAndroidContext(IntPtr handle);
#elif COCOA
            IntPtr NsObject { set; }
#endif
    }
}
