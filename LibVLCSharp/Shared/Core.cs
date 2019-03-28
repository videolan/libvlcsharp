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
    public static class Core
    {
        struct Native
        {
#if NET || NETSTANDARD
            [DllImport(Constants.Kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadPackagedLibrary(string dllToLoad, uint reserved =  0);

            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport(Constants.libSystem)]
            internal static extern IntPtr dlopen(string libraryPath, int mode = 1);

            /// <summary>
            /// Initializes the X threading system
            /// </summary>
            /// <remarks>Linux X11 only</remarks>
            /// <returns>non-zero on success, zero on failure</returns>
            [DllImport(Constants.libX11, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int XInitThreads();

#elif ANDROID
            [DllImport(Constants.LibraryName, EntryPoint = "JNI_OnLoad")]
            internal static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default(IntPtr));
#endif
        }

#if NET || NETSTANDARD
        static IntPtr _libvlccoreHandle;
        static IntPtr _libvlcHandle;
#endif
        /// <summary>
        /// Load the native libvlc library (if necessary, depending on platform)
        /// <para/> Ensure that you installed the VideoLAN.LibVLC.[YourPlatform] package in your target project
        /// <para/> This will throw a <see cref="VLCException"/> if the native libvlc libraries cannot be found or loaded.
        /// </summary>
        /// <param name="libvlcDirectoryPath">The path to the directory that contains libvlc and libvlccore
        /// No need to specify unless running netstandard 1.1, or using custom location for libvlc
        /// <para/> This parameter is NOT supported on Linux, use LD_LIBRARY_PATH instead.
        /// </param>
        public static void Initialize(string libvlcDirectoryPath = null)
        {
#if ANDROID
            InitializeAndroid();
#elif NET || NETSTANDARD
            InitializeDesktop(libvlcDirectoryPath);
#endif
        }

#if ANDROID
        static void InitializeAndroid()
        {
            var initLibvlc = Native.JniOnLoad(JniRuntime.CurrentRuntime.InvocationPointer);
            if(initLibvlc == 0)
                throw new VLCException("failed to initialize libvlc with JniOnLoad " +
                                       $"{nameof(JniRuntime.CurrentRuntime.InvocationPointer)}: {JniRuntime.CurrentRuntime.InvocationPointer}");
        }
#elif NET || NETSTANDARD
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

            // TODO: Temp HACK to make it work for UAP target
            paths.Add((Constants.CoreLibraryName, Constants.LibraryName));

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

            string libvlccorePath1 = string.Empty;
            if (PlatformHelper.IsWindows)
            {
                libvlccorePath1 = LibVLCCorePath(libvlcDirPath1);
            }
            var libvlcPath1 = LibVLCPath(libvlcDirPath1);
            paths.Add((libvlccorePath1, libvlcPath1));

            var assemblyLocation = Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly()?.Location;
            
            var libvlcDirPath2 = Path.Combine(Path.GetDirectoryName(assemblyLocation), 
                Constants.LibrariesRepositoryFolderName, arch);

            string libvlccorePath2 = string.Empty;
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
                handle = Native.dlopen(nativeLibraryPath);
            }
            else
            {
                handle = Native.LoadPackagedLibrary(nativeLibraryPath); // TODO: Use correct API depending on if UWP or not
            }

            return handle != IntPtr.Zero;
        }
#endif // NET || NETSTANDARD
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
        internal const string libSystem = "libSystem";
        internal const string Kernel32 = "kernel32";
        internal const string libX11 = "libX11";
        internal const string WindowsLibraryExtension = ".dll";
        internal const string MacLibraryExtension = ".dylib";
    }

    internal static class ArchitectureNames
    {
        internal const string Win64 = "win-x64";
        internal const string Win86 = "win-x86";
        internal const string Winrt64 = "winrt-x64";
        internal const string Winrt86 = "winrt-x86";
        
        internal const string Lin64 = "linux-x64";
        internal const string LinArm = "linux-arm";

        internal const string MacOS64 = "osx-x64";
    }
}