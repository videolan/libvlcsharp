using System;
using System.Diagnostics;
using System.IO;
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
            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern IntPtr LoadPackagedLibrary(string dllToLoad);

            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport(Constants.libSystem)]
            internal static extern IntPtr dlopen(string libraryPath, int mode = 1);
#if ANDROID
            [DllImport(Constants.LibraryName, EntryPoint = "JNI_OnLoad")]
            internal static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default(IntPtr));
#endif

            /// <summary>
            /// Initializes the X threading system
            /// </summary>
            /// <remarks>Linux X11 only</remarks>
            /// <returns>non-zero on success, zero on failure</returns>
            [DllImport(Constants.libX11, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int XInitThreads();
        }

        static IntPtr _libvlccoreHandle;
        static IntPtr _libvlcHandle;

        /// <summary>
        /// Load the native libvlc library (if necessary depending on platform)
        /// </summary>
        /// <param name="appExecutionDirectory">The path to the app execution directory. 
        /// No need to specify unless running netstandard 1.1, or using custom location for libvlc
        /// </param>
        public static void Initialize(string appExecutionDirectory = null)
        {
#if ANDROID
            InitializeAndroid();
#elif !IOS
            InitializeDesktop(appExecutionDirectory);
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
#endif
        //TODO: Add Unload library func using handles
        static void InitializeDesktop(string appExecutionDirectory = null)
        {
            if(appExecutionDirectory == null)
            {
#if NETSTANDARD1_1
                throw new ArgumentNullException(nameof(appExecutionDirectory),
                    $"{nameof(appExecutionDirectory)} cannot be null for netstandard1.1 target. Please provide a path to Initialize.");
#else
                var myPath = typeof(LibVLC).Assembly.Location;
                appExecutionDirectory = Path.GetDirectoryName(myPath);
                if (appExecutionDirectory == null)
                    throw new NullReferenceException(nameof(appExecutionDirectory));
#endif
            }

            if (PlatformHelper.IsWindows)
            {
                var arch = PlatformHelper.IsX64BitProcess ? ArchitectureNames.Win64 : ArchitectureNames.Win86;

                var librariesFolder = Path.Combine(appExecutionDirectory, Constants.LibrariesRepositoryFolderName, arch);

                _libvlccoreHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.CoreLibraryName}.dll");
                
                if(_libvlccoreHandle == IntPtr.Zero)
                {
                    throw new VLCException($"Failed to load required native library {Constants.CoreLibraryName}.dll");
                }

                _libvlcHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.LibraryName}.dll");
                
                if(_libvlcHandle == IntPtr.Zero)
                {
                    throw new VLCException($"Failed to load required native library {Constants.LibraryName}.dll");
                }
            }
            else if (PlatformHelper.IsMac)
            {
                _libvlcHandle = PreloadNativeLibrary(appExecutionDirectory, $"{Constants.LibraryName}.dylib");
                if (_libvlcHandle == IntPtr.Zero)
                {
                    throw new VLCException($"Failed to load required native library {Constants.LibraryName}.dylib");
                }
            }
            else if (PlatformHelper.IsLinux)
            {
                // Initializes X threads before calling VLC. This is required for vlc plugins like the VDPAU hardware acceleration plugin.
                if (Native.XInitThreads() == 0)
                {
#if !NETSTANDARD1_1
                    Trace.WriteLine("XInitThreads failed");
#endif
                }
            }
        }

        //TODO: Add dlopen for UWP, Linux
        static IntPtr PreloadNativeLibrary(string nativeLibrariesPath, string libraryName)
        {
            Debug.WriteLine($"Loading {libraryName}");
            var libraryPath = Path.Combine(nativeLibrariesPath, libraryName);

#if !NETSTANDARD1_1
            if (!File.Exists(libraryPath))
            {
                Debug.WriteLine($"Cannot find {libraryPath}");
                return IntPtr.Zero;
            }
#endif
            return PlatformHelper.IsMac ? Native.dlopen(libraryPath) : Native.LoadLibrary(libraryPath);
        }
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