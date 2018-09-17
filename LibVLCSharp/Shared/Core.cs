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
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr LoadPackagedLibrary(string dllToLoad);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string dllToLoad);
#if ANDROID
            [DllImport(Constants.LibraryName, EntryPoint = "JNI_OnLoad")]
            internal static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default(IntPtr));
#endif
        }

        static IntPtr _libvlccoreHandle;
        static IntPtr _libvlcHandle;

        public static void Initialize()
        {
#if DESKTOP
            InitializeDesktop();
#elif ANDROID
            InitializeAndroid();
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
#elif DESKTOP
        //TODO: Add Unload library func using handles
        static void InitializeDesktop()
        {
            var myPath = typeof(LibVLC).Assembly.Location;
            var appExecutionDirectory = Path.GetDirectoryName(myPath);
            if (appExecutionDirectory == null)
                throw new NullReferenceException(nameof(appExecutionDirectory));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var arch = Environment.Is64BitProcess ? ArchitectureNames.Win64 : ArchitectureNames.Win86;

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
        }

        //TODO: check if Store app
        static IntPtr PreloadNativeLibrary(string nativeLibrariesPath, string libraryName)
        {
            Debug.WriteLine($"Loading {libraryName}");
            var libraryPath = Path.Combine(nativeLibrariesPath, libraryName);
            if(!File.Exists(libraryPath))
            {
                Debug.WriteLine($"Cannot find {libraryPath}");
                return IntPtr.Zero;
            }

            return Native.LoadLibrary(libraryPath);// TODO: cross-platform load
        }
#endif
    }

    internal static class Constants
    {
#if IOS
        internal const string LibraryName = "@rpath/DynamicMobileVLCKit.framework/DynamicMobileVLCKit";
#elif MAC
        internal const string LibraryName = "@rpath/VLCKit.framework/VLCKit";
#else
        internal const string LibraryName = "libvlc";    
#endif

        internal const string CoreLibraryName = "libvlccore";

        /// <summary>
        /// The name of the folder that contains the per-architecture folders
        /// </summary>
        internal const string LibrariesRepositoryFolderName = "libvlc";
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