using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    internal class PlatformHelper
    {
        internal static bool IsWindows
        {
#if NET40
            get => Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        }

        internal static bool IsLinux
        {
#if NET40
            get => Environment.OSVersion.Platform == PlatformID.Unix;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif
        }

        internal static bool IsMac
        {
#if NET40
            get => false; // no easy way to detect Mac platform host at runtime under net471
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
        }

        internal static bool IsX64BitProcess => IntPtr.Size == 8;
    }
}