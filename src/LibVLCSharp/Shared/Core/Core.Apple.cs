#if APPLE
using System;
#if MAC
using System.IO;
using System.Runtime.InteropServices;
#endif

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// The Core class handles libvlc loading intricacies on various platforms as well as
    /// the libvlc/libvlcsharp version match check.
    /// </summary>
    public static partial class Core
    {
#if MAC
        static IntPtr LibvlcHandle;
        static IntPtr LibvlccoreHandle;
#endif
        /// <summary>
        /// Load the native libvlc library (if necessary, depending on platform)
        /// <para/> Ensure that you installed the VideoLAN.LibVLC.[YourPlatform] package in your target project
        /// <para/> This will throw a <see cref="VLCException"/> if the native libvlc libraries cannot be found or loaded.
        /// <para/> It may also throw a <see cref="VLCException"/> if the LibVLC and LibVLCSharp major versions do not match.
        /// See https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/versioning.md for more info about the versioning strategy.
        /// </summary>
        /// <param name="libvlcDirectoryPath">The path to the directory that contains libvlc and libvlccore
        /// No need to specify unless running netstandard 1.1, or using custom location for libvlc
        /// <para/> This parameter is NOT supported on Linux, use LD_LIBRARY_PATH instead.
        /// </param>
        public static void Initialize(string? libvlcDirectoryPath = null)
        {
#if MAC
            if (string.IsNullOrEmpty(libvlcDirectoryPath))
            {
                // Load the VideoLAN.LibVLC.Mac runtime tree by path before any libvlc
                // P/Invoke: its libvlc.dylib only finds libvlccore once that is loaded,
                // and Mono (Xamarin.Mac) caches a failed P/Invoke lookup for good.
                var packagedDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    Constants.LibrariesRepositoryFolderName,
                    MacOSArchitectureFolder(RuntimeInformation.ProcessArchitecture), Constants.Lib);
                if (File.Exists(LibVLCPath(packagedDirectory)))
                {
                    libvlcDirectoryPath = packagedDirectory;
                }
            }
            if (string.IsNullOrEmpty(libvlcDirectoryPath))
            {
                // Preserve Apple loading through the runtime or an already loaded
                // library. Only a missing library triggers explicit path discovery;
                // version mismatches and missing entry points must still propagate.
                try
                {
                    EnsureVersionsMatch();
                    LibVLCLoaded = true;
                    return;
                }
                catch (DllNotFoundException)
                {
                }
            }
            LoadLibVLC(libvlcDirectoryPath);
#endif
            EnsureVersionsMatch();
            LibVLCLoaded = true;
        }
    }
}
#endif
