using LibVLCSharp.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
#if ANDROID
using Java.Interop;
#endif

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// The Core class handles libvlc loading intricacies on various platforms as well as 
    /// the libvlc/libvlcsharp version match check.
    /// </summary>
    public static class Core
    {
        struct Native
        {
#if UWP
            [DllImport(Constants.Kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadPackagedLibrary(string dllToLoad, uint reserved = 0);
#elif NETFRAMEWORK || NETSTANDARD
            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport(Constants.LibSystem, EntryPoint = "dlopen")]
            internal static extern IntPtr Dlopen(string libraryPath, int mode = 1);

            /// <summary>
            /// Initializes the X threading system
            /// </summary>
            /// <remarks>Linux X11 only</remarks>
            /// <returns>non-zero on success, zero on failure</returns>
            [DllImport(Constants.LibX11, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int XInitThreads();

            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern ErrorModes SetErrorMode(ErrorModes uMode);
#elif ANDROID
            [DllImport(Constants.LibraryName, EntryPoint = "JNI_OnLoad")]
            internal static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default);
#endif
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();
        }


#if NETFRAMEWORK || NETSTANDARD || UWP
        static IntPtr _libvlcHandle;
#endif
#if !UWP && NETFRAMEWORK || NETSTANDARD
        static IntPtr _libvlccoreHandle;
#endif

        /// <summary>
        /// Load the native libvlc library (if necessary, depending on platform)
        /// <para/> Ensure that you installed the VideoLAN.LibVLC.[YourPlatform] package in your target project
        /// <para/> This will throw a <see cref="VLCException"/> if the native libvlc libraries cannot be found or loaded.
        /// <para/> It may also throw a <see cref="VLCException"/> if the LibVLC and LibVLCSharp major versions do not match.
        /// See https://code.videolan.org/videolan/LibVLCSharp/blob/master/VERSIONING.md for more info about the versioning strategy.
        /// </summary>
        /// <param name="libvlcDirectoryPath">The path to the directory that contains libvlc and libvlccore
        /// No need to specify unless running netstandard 1.1, or using custom location for libvlc
        /// <para/> This parameter is NOT supported on Linux, use LD_LIBRARY_PATH instead.
        /// </param>
        public static void Initialize(string libvlcDirectoryPath = null)
        {
#if ANDROID
            if(Android.OS.Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.JellyBeanMr1)
                LoadLibCpp();

            InitializeAndroid();
#elif UWP
            InitializeUWP();
#elif NETFRAMEWORK || NETSTANDARD
            DisableMessageErrorBox();
            InitializeDesktop(libvlcDirectoryPath);
#endif
#if !UWP10_0 && !NETSTANDARD1_1
            EnsureVersionsMatch();
#endif
        }

#if !UWP10_0 && !NETSTANDARD1_1
        /// <summary>
        /// Checks whether the major version of LibVLC and LibVLCSharp match <para/>
        /// Throws an NotSupportedException if the major versions mismatch
        /// </summary>
        static void EnsureVersionsMatch()
        {
            var libvlcMajorVersion = int.Parse(Native.LibVLCVersion().FromUtf8().Split('.').First());
            var libvlcsharpMajorVersion = Assembly.GetExecutingAssembly().GetName().Version.Major;
            if(libvlcMajorVersion != libvlcsharpMajorVersion)
                throw new VLCException($"Version mismatch between LibVLC {libvlcMajorVersion} and LibVLCSharp {libvlcsharpMajorVersion}. " +
                    $"They must share the same major version number");
        }
#endif
#if ANDROID
        static void LoadLibCpp()
        {
            try
            {               
                Java.Lang.JavaSystem.LoadLibrary("c++_shared");
            }
            catch(Java.Lang.UnsatisfiedLinkError exception)
            {
                throw new VLCException($"failed to load libc++_shared {nameof(exception)} {exception.Message}");
            }
        }
        static void InitializeAndroid()
        {
            var initLibvlc = Native.JniOnLoad(JniRuntime.CurrentRuntime.InvocationPointer);
            if(initLibvlc == 0)
                throw new VLCException("failed to initialize libvlc with JniOnLoad " +
                                       $"{nameof(JniRuntime.CurrentRuntime.InvocationPointer)}: {JniRuntime.CurrentRuntime.InvocationPointer}");
        }
#elif UWP
        static void InitializeUWP()
        {
            _libvlcHandle = Native.LoadPackagedLibrary(Constants.LibraryName);
            if (_libvlcHandle == IntPtr.Zero)
            {
                throw new VLCException($"Failed to load {Constants.LibraryName}{Constants.WindowsLibraryExtension}, error {Marshal.GetLastWin32Error()}. Please make sure that this library, {Constants.CoreLibraryName}{Constants.WindowsLibraryExtension} and the plugins are copied to the `AppX` folder. For that, you can reference the `VideoLAN.LibVLC.WindowsRT` NuGet package.");
            }
        }

#elif NETFRAMEWORK || NETSTANDARD
        /// <summary>
        /// Disable error dialogs in case of dll loading failures on older Windows versions.
        /// <para/>
        /// This is mostly to fix Windows XP support (https://code.videolan.org/videolan/LibVLCSharp/issues/173),
        /// though it may happen under other conditions (broken plugins/wrong ABI).
        /// <para/>
        /// As libvlc may load additional plugins later in the lifecycle of the application, 
        /// we should not unset this on exiting <see cref="Initialize(string)"/>
        /// </summary>
        static void DisableMessageErrorBox()
        {
            if (!PlatformHelper.IsWindows)
                return;

            var oldMode = Native.SetErrorMode(ErrorModes.SYSTEM_DEFAULT);
            Native.SetErrorMode(oldMode | ErrorModes.SEM_FAILCRITICALERRORS | ErrorModes.SEM_NOOPENFILEERRORBOX);
        }

        static void InitializeDesktop(string libvlcDirectoryPath = null)
        {
            if(PlatformHelper.IsLinux)
            {
                if (!string.IsNullOrEmpty(libvlcDirectoryPath))
                {
                    throw new InvalidOperationException($"Using {nameof(libvlcDirectoryPath)} is not supported on the Linux platform. " +
                        $"The recommended way is to have the libvlc librairies in /usr/lib. Use LD_LIBRARY_PATH if you need more customization");
                }
                // Initializes X threads before calling VLC. This is required for vlc plugins like the VDPAU hardware acceleration plugin.
                if (Native.XInitThreads() == 0)
                {
#if !NETSTANDARD1_1
                    Trace.WriteLine("XInitThreads failed");
#endif
                }
                return;
            }

            // full path to directory location of libvlc and libvlccore has been provided
            if (!string.IsNullOrEmpty(libvlcDirectoryPath))
            {
                bool loadResult;
                if(PlatformHelper.IsWindows)
                {
                    var libvlccorePath = LibVLCCorePath(libvlcDirectoryPath);
                    loadResult = LoadNativeLibrary(libvlccorePath, out _libvlccoreHandle);
                    if(!loadResult)
                    {
                        Log($"Failed to load required native libraries at {libvlccorePath}");
                        return;
                    }
                }

                var libvlcPath = LibVLCPath(libvlcDirectoryPath);
                loadResult = LoadNativeLibrary(libvlcPath, out _libvlcHandle);
                if(!loadResult)
                    Log($"Failed to load required native libraries at {libvlcPath}");
                return;
            }

#if !NETSTANDARD1_1
            var paths = ComputeLibVLCSearchPaths();

            foreach(var path in paths)
            {
                if (PlatformHelper.IsWindows)
                {
                    LoadNativeLibrary(path.libvlccore, out _libvlccoreHandle);
                }
                var loadResult = LoadNativeLibrary(path.libvlc, out _libvlcHandle);
                if (loadResult) break;
            }

            if (!Loaded)
            {
                throw new VLCException($"Failed to load required native libraries. Search paths include {string.Join("; ", paths.Select(p => $"{p.libvlc},{p.libvlccore}"))}");
            }
#endif
        }

#if !NETSTANDARD1_1
        static List<(string libvlccore, string libvlc)> ComputeLibVLCSearchPaths()
        {
            var paths = new List<(string, string)>();
            string arch;

            if(PlatformHelper.IsMac)
            {
                arch = ArchitectureNames.MacOS64;
            }
            else
            {
                arch = PlatformHelper.IsX64BitProcess ? ArchitectureNames.Win64 : ArchitectureNames.Win86;
            }

            var libvlcDirPath1 = Path.Combine(Path.GetDirectoryName(typeof(LibVLC).Assembly.Location), 
                Constants.LibrariesRepositoryFolderName, arch);

            var libvlccorePath1 = string.Empty;
            if (PlatformHelper.IsWindows)
            {
                libvlccorePath1 = LibVLCCorePath(libvlcDirPath1);
            }
            var libvlcPath1 = LibVLCPath(libvlcDirPath1);
            paths.Add((libvlccorePath1, libvlcPath1));

            var assemblyLocation = Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly()?.Location;

            var libvlcDirPath2 = Path.Combine(Path.GetDirectoryName(assemblyLocation), 
                Constants.LibrariesRepositoryFolderName, arch);

            var libvlccorePath2 = string.Empty;
            if(PlatformHelper.IsWindows)
            {
                libvlccorePath2 = LibVLCCorePath(libvlcDirPath2);
            }

            var libvlcPath2 = LibVLCPath(libvlcDirPath2);
            paths.Add((libvlccorePath2, libvlcPath2));

            var libvlcPath3 = LibVLCPath(Path.GetDirectoryName(typeof(LibVLC).Assembly.Location));

            paths.Add((string.Empty, libvlcPath3));
            return paths;
        }
#endif
        static string LibVLCCorePath(string dir) => Path.Combine(dir, $"{Constants.CoreLibraryName}{LibraryExtension}");

        static string LibVLCPath(string dir) => Path.Combine(dir, $"{Constants.LibraryName}{LibraryExtension}");

        static string LibraryExtension => PlatformHelper.IsWindows ? Constants.WindowsLibraryExtension : Constants.MacLibraryExtension;

        static bool Loaded => _libvlcHandle != IntPtr.Zero;

        static void Log(string message)
        {
#if !NETSTANDARD1_1
            Trace.WriteLine(message);
#else
            Debug.WriteLine(message);
#endif
        }

        static bool LoadNativeLibrary(string nativeLibraryPath, out IntPtr handle)
        {
            handle = IntPtr.Zero;
            Log($"Loading {nativeLibraryPath}");

#if !NETSTANDARD1_1
            if (!File.Exists(nativeLibraryPath))
            {
                Log($"Cannot find {nativeLibraryPath}");
                return false;
            }
#endif
            if(PlatformHelper.IsMac)
            {
                handle = Native.Dlopen(nativeLibraryPath);
            }
            else
            {
                handle = Native.LoadLibrary(nativeLibraryPath);
            }

            return handle != IntPtr.Zero;
        }
#endif // NETFRAMEWORK || NETSTANDARD
    }

    internal static class Constants
    {
#if IOS
        internal const string LibraryName = "@rpath/DynamicMobileVLCKit.framework/DynamicMobileVLCKit";
#elif UNITY_ANDROID
        /// <summary>
        /// The vlc-unity C++ plugin which handles rendering (opengl/d3d) libvlc callbacks
        /// </summary>
        internal const string UnityPlugin = "VlcUnityWrapper";
        internal const string LibraryName = "libvlcjni";
#elif TVOS
        internal const string LibraryName = "@rpath/DynamicTVVLCKit.framework/DynamicTVVLCKit";
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
