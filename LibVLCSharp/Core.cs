using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VideoLAN.LibVLC
{
    public static class Core
    {
        struct Native
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr LoadPackagedLibrary(string dllToLoad);
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InitializeWindows();
            }
        }

        //TODO: check if Store app
        //TODO: Add Unload library func using handles
        static void InitializeWindows()
        {
            var myPath = new Uri(typeof(Instance).Assembly.CodeBase).LocalPath;
            var appExecutionDirectory = Path.GetDirectoryName(myPath);
            if (appExecutionDirectory == null)
                throw new NullReferenceException(nameof(appExecutionDirectory));

            var arch = Environment.Is64BitProcess ? Win64 : Win86;
           
            var libvlccorePath = Path.Combine(Path.Combine(appExecutionDirectory, Libvlc), Path.Combine(arch, $"{Libvlccore}.dll"));
            var libvlcPath = Path.Combine(Path.Combine(appExecutionDirectory, Libvlc), Path.Combine(arch, $"{Libvlc}.dll"));

            LoadLibvlcLibraries(libvlccorePath, libvlcPath);    
        }

        static void LoadLibvlcLibraries(string libvlccorePath, string libvlcPath)
        {
            if(string.IsNullOrEmpty(libvlccorePath)) throw new NullReferenceException(nameof(libvlccorePath));
            if(string.IsNullOrEmpty(libvlcPath)) throw new NullReferenceException(nameof(libvlcPath));

            _libvlccoreHandle = Native.LoadLibrary(libvlccorePath);
            if(_libvlccoreHandle == IntPtr.Zero)
                throw new InvalidOperationException("failed to load libvlccore with path " + libvlccorePath + ". Aborting...");

            _libvlcHandle = Native.LoadLibrary(libvlcPath);
            if (_libvlcHandle == IntPtr.Zero)
                throw new InvalidOperationException("failed to load libvlc with path " + libvlcPath + ". Aborting...");
        }
    }
}