using System;

namespace LibVLCSharp.Shared
{
    internal static class Constants
    {
#if IOS
        internal const string LibraryName = "@rpath/DynamicMobileVLCKit.framework/DynamicMobileVLCKit";
#elif TVOS
        internal const string LibraryName = "@rpath/DynamicTVVLCKit.framework/DynamicTVVLCKit";
#elif MAC
        internal const string LibraryName = "@executable_path/../MonoBundle/lib/libvlc.dylib";
#else
        internal const string LibraryName = "libvlc";
#endif
        internal const string CoreLibraryName = "libvlccore";

        /// <summary>
        /// The name of the folder that contains the per-architecture folders
        /// </summary>
        internal const string LibrariesRepositoryFolderName = "libvlc";

        internal const string Msvcrt = "msvcrt";
        internal const string Libc = "libc";
        internal const string LibSystem = "libSystem";
        internal const string Kernel32 = "kernel32";
        internal const string LibX11 = "libX11";
        internal const string WindowsLibraryExtension = ".dll";
        internal const string MacLibraryExtension = ".dylib";
        internal const string Lib = "lib";
        internal const string LibVLC = "libvlc";
        internal const string VLCPLUGINPATH = "VLC_PLUGIN_PATH";
        internal const string Plugins = "plugins";
    }

    internal static class ArchitectureNames
    {
        internal const string Win64 = "win-x64";
        internal const string Win86 = "win-x86";
        internal const string Winrt64 = "winrt-x64";
        internal const string Winrt86 = "winrt-x86";
        internal const string WinrtArm = "winrt-arm";

        internal const string Lin64 = "linux-x64";
        internal const string LinArm = "linux-arm";

        internal const string MacOS64 = "osx-x64";
    }

    [Flags]
    internal enum ErrorModes : uint
    {
        SYSTEM_DEFAULT = 0x0,
        SEM_FAILCRITICALERRORS = 0x0001,
        SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
        SEM_NOGPFAULTERRORBOX = 0x0002,
        SEM_NOOPENFILEERRORBOX = 0x8000
    }
}

