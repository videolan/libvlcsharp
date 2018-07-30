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

        const string Win64 = "win-x64";
        const string Win86 = "win-x86";
        const string Winrt64 = "winrt-x64";
        const string Winrt86 = "winrt-x86";

        const string Libvlc = "libvlc";
        const string Libvlccore = "libvlccore";

        public static void Initialize()
        {
#if WINDOWS
            InitializeWindows();
#elif ANDROID
            InitializeAndroid();
#elif NETCORE
            InitializeNetCore();
#endif
        }

#if NETCORE
        static void InitializeNetCore()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                InitializeWindows();
        }
#endif

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
        static void InitializeWindows()
        {
            var myPath = new Uri(typeof(LibVLC).Assembly.CodeBase).LocalPath;
            var appExecutionDirectory = Path.GetDirectoryName(myPath);
            if (appExecutionDirectory == null)
                throw new NullReferenceException(nameof(appExecutionDirectory));

            var arch = Environment.Is64BitProcess ? Win64 : Win86;

            var libvlccorePath = Path.Combine(Path.Combine(appExecutionDirectory, Libvlc),
                Path.Combine(arch, $"{Libvlccore}.dll"));
            var libvlcPath = Path.Combine(Path.Combine(appExecutionDirectory, Libvlc),
                Path.Combine(arch, $"{Libvlc}.dll"));

            Debug.WriteLine(nameof(libvlccorePath) + ": " + libvlccorePath);
            Debug.WriteLine(nameof(libvlcPath) + ": " + libvlcPath);

            LoadLibvlcLibraries(libvlccorePath, libvlcPath);
        }

        //TODO: check if Store app
        static void LoadLibvlcLibraries(string libvlccorePath, string libvlcPath)
        {
            if (string.IsNullOrEmpty(libvlccorePath)) throw new NullReferenceException(nameof(libvlccorePath));
            if (string.IsNullOrEmpty(libvlcPath)) throw new NullReferenceException(nameof(libvlcPath));

            _libvlccoreHandle = Native.LoadLibrary(libvlccorePath);
            if (_libvlccoreHandle == IntPtr.Zero)
                throw new InvalidOperationException("failed to load libvlccore with path " + libvlccorePath +
                                                    ". Aborting...");

            _libvlcHandle = Native.LoadLibrary(libvlcPath);
            if (_libvlcHandle == IntPtr.Zero)
                throw new InvalidOperationException("failed to load libvlc with path " + libvlcPath + ". Aborting...");
        }
    }

    static class Constants
    {
#if IOS
        internal const string LibraryName = "@rpath/DynamicMobileVLCKit.framework/DynamicMobileVLCKit";
#elif MAC
        internal const string LibraryName = "@rpath/VLCKit.framework/VLCKit";
#else
        internal const string LibraryName = "libvlc";    
#endif
    }
}