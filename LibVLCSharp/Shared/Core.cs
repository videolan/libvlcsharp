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

            /// <summary>
            /// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
            /// </summary>
            /// <param name="dllToLoad">The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file). The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file. If the string specifies a full path, the function searches only that path for the module. If the string specifies a relative path or a module name without a path, the function uses a standard search strategy to find the module; for more information, see the Remarks. If the function cannot find the module, the function fails. When specifying a path, be sure to use backslashes (\), not forward slashes (/). For more information about paths, see Naming a File or Directory. If the string specifies a module name without a path and the file name extension is omitted, the function appends the default library extension .dll to the module name. To prevent the function from appending .dll to the module name, include a trailing point character (.) in the module name string.</param>
            /// <returns>If the function succeeds, the return value is a handle to the module. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string dllToLoad);

            /// <summary>
            /// Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
            /// </summary>
            /// <param name="hModule">A handle to the DLL module that contains the function or variable.</param>
            /// <param name="lpProcName">he function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
            /// <returns>If the function succeeds, the return value is the address of the exported function or variable. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
            [DllImport(Constants.Kernel32, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            /// <summary>
            /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count. When the reference count reaches zero, the module is unloaded from the address space of the calling process and the handle is no longer valid.
            /// </summary>
            /// <param name="hModule">A handle to the loaded library module.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call the GetLastError function.</returns>
            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern bool FreeLibrary(IntPtr hModule);


            [DllImport(Constants.libSystem)]
            internal static extern IntPtr dlopen(string libraryPath, int mode = 1);
#if ANDROID
            [DllImport(Constants.LibraryName, EntryPoint = "JNI_OnLoad")]
            internal static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default(IntPtr));
#endif
        }

        struct LinuxNative
        {
            internal const int RTLD_LAZY = 1;
            [DllImport(Constants.Libdl, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr dlopen(string lpFileName, int flags);

            [DllImport(Constants.Libdl, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr dlsym(IntPtr handle, string symbol);

            [DllImport(Constants.Libdl, CallingConvention = CallingConvention.Cdecl)]
            internal static extern bool dlclose(IntPtr handle);

            [DllImport(Constants.Libdl, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            internal static extern string dlerror();
        }

        /// <summary>
        /// The handle of the libvlccore library
        /// </summary>
        internal static IntPtr LibVLCCoreHandle { get; private set; }

        /// <summary>
        /// The handle of the libvlc library
        /// </summary>
        internal static IntPtr LibVLCHandle { get; private set; }

        /// <summary>
        /// The handle of the libvlcLogInterop library. https://github.com/jeremyVignelles/libvlcLogInterop
        /// </summary>
        internal static IntPtr LibVLCLogInteropHandle { get; private set; }

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

            var arch = GetArchitectureName();

            if (IsWindows)
            {
                var librariesFolder = Path.Combine(appExecutionDirectory, Constants.LibrariesRepositoryFolderName, arch);

                LibVLCCoreHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.CoreLibraryName}.dll");
                
                if(LibVLCCoreHandle == IntPtr.Zero)
                {
                    throw new VLCException($"Failed to load required native library {Constants.CoreLibraryName}.dll");
                }

                LibVLCHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.LibraryName}.dll");
                
                if(LibVLCHandle == IntPtr.Zero)
                {
                    throw new VLCException($"Failed to load required native library {Constants.LibraryName}.dll");
                }


                LibVLCLogInteropHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.LibVLCLogInteropName}.dll");
            }
            else if (IsLinux)
            {
                var librariesFolder = Path.Combine(appExecutionDirectory, Constants.LibrariesRepositoryFolderName, arch);

                // Try to find libvlc in the subfolder
                LibVLCCoreHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.CoreLibraryName}.so");
                LibVLCHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.LibraryName}.so");
                LibVLCLogInteropHandle = PreloadNativeLibrary(librariesFolder, $"{Constants.LibVLCLogInteropName}.so");
            }
            else if (IsMac)
            {
                LibVLCHandle = PreloadNativeLibrary(appExecutionDirectory, $"{Constants.LibraryName}.dylib");
                if (LibVLCHandle == IntPtr.Zero)
                {
                    throw new VLCException($"Failed to load required native library {Constants.LibraryName}.dylib");
                }
            }
        }

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

            if (IsWindows)
            {
                return Native.LoadLibrary(libraryPath);
            }
            else if (IsLinux)
            {
                return LinuxNative.dlopen(libraryPath, LinuxNative.RTLD_LAZY);
            }
            else if (IsMac)
            {
                return Native.dlopen(libraryPath);
            }

            throw new PlatformNotSupportedException();
        }

        internal static T GetLibraryFunction<T>(IntPtr libraryHandle, string functionName)
        {
            //TODO : caching
            IntPtr procAddress;
            if (IsWindows)
            {
                procAddress = Native.GetProcAddress(libraryHandle, functionName);
                if (procAddress == IntPtr.Zero)
                {
                    var errorMessage = $"The address of the function '{functionName}' cannot be found in the library";
#if NETSTANDARD1_1
                    throw new Exception(errorMessage);
#else
                    throw new MissingMethodException(errorMessage);
#endif
                }
            }
            else if (IsLinux)
            {
                procAddress = LinuxNative.dlsym(libraryHandle, functionName);
                if (procAddress == IntPtr.Zero)
                {
                    var errorMessage = $"The address of the function '{functionName}' cannot be found in the library. dlsym failed with error {LinuxNative.dlerror()}";
#if NETSTANDARD1_1
                    throw new Exception(errorMessage);
#else
                    throw new MissingMethodException(errorMessage);
#endif
                }
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

#if NETSTANDARD1_1 || NET40
            return (T)(object)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T));
#else
            return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
#endif
        }

        static string GetArchitectureName()
        {
            if (IsWindows)
            {
#if NET40 || NETSTANDARD1_1
                return IntPtr.Size == 8 ? ArchitectureNames.Win64 : ArchitectureNames.Win86;
#else
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X86: return ArchitectureNames.Win86;
                    case Architecture.X64: return ArchitectureNames.Win64;
                    default: throw new PlatformNotSupportedException();
                }
#endif
            }
            else if (IsLinux)
            {
#if NET40 || NETSTANDARD1_1
                // This logic is quite flawed, but we can't detect correctly on NET40 and NETSTANDARD1_1. If you're in that case, please use another framework version.
                return IntPtr.Size == 8 ? ArchitectureNames.Lin64 : ArchitectureNames.LinArm;
#else
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.Arm: return ArchitectureNames.LinArm;
                    case Architecture.X64: return ArchitectureNames.Lin64;
                    default: throw new PlatformNotSupportedException();
                }
#endif
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        static bool IsWindows
        {
#if NET40
            get => Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        }

        static bool IsLinux
        {
#if NET40
            get => Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif
        }

        static bool IsMac
        {
#if NET40
            get => (int)Environment.OSVersion.Platform == 6;
#else
            get => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
        }

        static bool IsX64BitProcess => IntPtr.Size == 8;
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

        internal const string LibVLCLogInteropName = "libvlcLogInterop";

        /// <summary>
        /// The name of the folder that contains the per-architecture folders
        /// </summary>
        internal const string LibrariesRepositoryFolderName = "libvlc";

        internal const string Msvcrt = "msvcrt";
        internal const string Libc = "libc";
        /// <summary>
        /// The library that contains functions to load dynamic libraries on linux
        /// </summary>
        internal const string Libdl = "libdl";
        internal const string libSystem = "libSystem";
        internal const string Kernel32 = "kernel32";
    }

    /// <summary>
    /// The .net RID corresponding to the compatible architectures.
    /// This is also the name of the folder which contains the native libraries
    /// </summary>
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