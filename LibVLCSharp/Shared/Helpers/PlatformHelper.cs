using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    public class PlatformHelper
    {
        public static bool IsWindows
        {
#if NET40
            get => Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        }

        public static bool IsLinux
        {
#if NET40
            get => Environment.OSVersion.Platform == PlatformID.Unix;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif
        }

        public static bool IsMac
        {
#if NET40
            get => false; // no easy way to detect Mac platform host at runtime under net471
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
        }

        public static bool IsX64BitProcess => IntPtr.Size == 8;
    }
}