using System;

namespace LibVLCSharp.Shared
{
    public interface ISource
    {
        void SetWindowHandle(IntPtr handle);
    }
}
