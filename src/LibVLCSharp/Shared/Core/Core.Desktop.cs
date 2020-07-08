#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.SafeDirectories)]

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// The Core class handles libvlc loading intricacies on various platforms as well as
    /// the libvlc/libvlcsharp version match check.
    /// </summary>
    public static partial class Core
    {
        partial struct Native
        {
            /// <summary>
            /// Initializes the X threading system
            /// </summary>
            /// <remarks>Linux X11 only</remarks>
            /// <returns>non-zero on success, zero on failure</returns>
            [DllImport(Constants.LibX11, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int XInitThreads();

            [DllImport(Constants.Kernel32, SetLastError = true)]
            internal static extern ErrorModes SetErrorMode(ErrorModes uMode);
        }
#if !NETSTANDARD1_1
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
            DisableMessageErrorBox();
            InitializeDesktop(libvlcDirectoryPath);
#if !NETSTANDARD1_1
            EnsureVersionsMatch();
#endif
        }

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

        static void InitializeDesktop(string? libvlcDirectoryPath = null)
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
                    Debug.WriteLine("XInitThreads failed");
                }
                return;
            }

            if (PlatformHelper.IsMac)
            {
#if !NETSTANDARD1_1
                var pluginPath = Path.Combine(Path.GetDirectoryName(typeof(LibVLC).Assembly.Location),
                    Constants.LibVLC, ArchitectureNames.MacOS64, Constants.Plugins);
                Console.Out.WriteLine("PluginPath: " + pluginPath);
                PluginPath(pluginPath);
#endif

            }
#if !NETSTANDARD1_1
            LoadLibVLC(libvlcDirectoryPath);
#endif
        }

#if NETCOREAPP2_0
    internal class CustomMacAssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
    {
        public CustomMacAssemblyLoadContext()
        {
            Default.Resolving += OnResolving;
        }

        [DllImport(Constants.LibSystem, EntryPoint = "dlopen")]
        internal static extern IntPtr Dlopen(string libraryPath, int mode = 1);

        public void LoadLibVLC(string libvlc) => LoadUnmanagedDll(libvlc);

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            Console.Out.WriteLine("LoadUnmanagedDll called with " + unmanagedDllName);
            return Dlopen(unmanagedDllName);
        }

        Assembly OnResolving(System.Runtime.Loader.AssemblyLoadContext context, AssemblyName name)
        {
            Console.Out.WriteLine("OnResolving called ===========");
            return Load(name);
        }
    }
#endif
    }

}
#endif // NETFRAMEWORK || NETSTANDARD
