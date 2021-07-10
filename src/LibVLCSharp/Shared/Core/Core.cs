﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using LibVLCSharp.Shared.Helpers;

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
#if !UWP10_0 && !NETSTANDARD1_1
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();
#endif
            [DllImport(Constants.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern IntPtr LoadLibraryW(string dllToLoad);

            [DllImport(Constants.Libdl, EntryPoint = "dlopen")]
            internal static extern IntPtr Dlopen(string libraryPath, int mode = 0x002);

            [DllImport(Constants.Libc)]
            internal static extern int setenv(string name, string value, int overwrite = 1);
        }

#if !UWP10_0 && !NETSTANDARD1_1
        /// <summary>
        /// Checks whether the major version of LibVLC and LibVLCSharp match <para/>
        /// Throws a VLCException if the major versions mismatch
        /// </summary>
        static void EnsureVersionsMatch()
        {
            var libvlcMajorVersion = int.Parse(Native.LibVLCVersion().FromUtf8()?.Split('.').FirstOrDefault() ?? "0");
            var libvlcsharpMajorVersion = Assembly.GetExecutingAssembly().GetName().Version.Major;
            if (libvlcMajorVersion != libvlcsharpMajorVersion)
                throw new VLCException($"Version mismatch between LibVLC {libvlcMajorVersion} and LibVLCSharp {libvlcsharpMajorVersion}. " +
                    $"They must share the same major version number");
        }

#endif
        static string LibVLCPath(string dir) => Path.Combine(dir, $"{Constants.LibVLC}{LibraryExtension}");
        static string LibVLCCorePath(string dir) => Path.Combine(dir, $"libvlccore{LibraryExtension}");
        static string LibraryExtension => PlatformHelper.IsWindows ? Constants.WindowsLibraryExtension : Constants.MacLibraryExtension;
#if !NETSTANDARD1_1
        static void PluginPath(string pluginPath) => Native.setenv(Constants.VLCPLUGINPATH, pluginPath);
#endif
        static void Log(string message)
        {
#if !UWP10_0 && !NETSTANDARD1_1
            Trace.WriteLine(message);
#else
            Debug.WriteLine(message);
#endif
        }

#if (MAC || NETFRAMEWORK || NETSTANDARD) && !NETSTANDARD1_1
        static bool Loaded => LibvlcHandle != IntPtr.Zero;
        static List<(string libvlccore, string libvlc)> ComputeLibVLCSearchPaths()
        {
            var paths = new List<(string, string)>();
            string arch;

            if (PlatformHelper.IsMac)
            {
                arch = Path.Combine(ArchitectureNames.MacOS64, Constants.Lib);
            }
            else
            {
                arch = PlatformHelper.IsX64BitProcess ? ArchitectureNames.Win64 : ArchitectureNames.Win86;
            }

            var libvlcAssemblyLocation = typeof(LibVLC).Assembly.Location;

#if !NET40
            if (string.IsNullOrEmpty(libvlcAssemblyLocation)) /* .NET 5 (single file / self contained) and later */
            {
                libvlcAssemblyLocation = AppContext.BaseDirectory;
            }
#endif
            var libvlcDirPath1 = Path.Combine(Path.GetDirectoryName(libvlcAssemblyLocation),
                Constants.LibrariesRepositoryFolderName, arch);

            var libvlccorePath1 = LibVLCCorePath(libvlcDirPath1);

            var libvlcPath1 = LibVLCPath(libvlcDirPath1);
            paths.Add((libvlccorePath1, libvlcPath1));

            var assemblyLocation = Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly()?.Location;
            if(!string.IsNullOrEmpty(assemblyLocation))
            { 
                var libvlcDirPath2 = Path.Combine(Path.GetDirectoryName(assemblyLocation),
                    Constants.LibrariesRepositoryFolderName, arch);

                var libvlccorePath2 = string.Empty;
                if (PlatformHelper.IsWindows)
                {
                    libvlccorePath2 = LibVLCCorePath(libvlcDirPath2);
                }

                var libvlcPath2 = LibVLCPath(libvlcDirPath2);
                paths.Add((libvlccorePath2, libvlcPath2));
            }
            var libvlcPath3 = LibVLCPath(Path.GetDirectoryName(libvlcAssemblyLocation));

            paths.Add((string.Empty, libvlcPath3));

            if (PlatformHelper.IsMac)
            {
                var libvlcPath4 = Path.Combine(Path.Combine(Path.GetDirectoryName(libvlcAssemblyLocation),
                    Constants.Lib), $"{Constants.LibVLC}{LibraryExtension}");
                var libvlccorePath4 = LibVLCCorePath(Path.Combine(Path.GetDirectoryName(libvlcAssemblyLocation), Constants.Lib));
                paths.Add((libvlccorePath4, libvlcPath4));
            }

            return paths;
        }

        static void LoadLibVLC(string? libvlcDirectoryPath = null)
        {
            // full path to directory location of libvlc and libvlccore has been provided
            if (!string.IsNullOrEmpty(libvlcDirectoryPath))
            {
                bool loadResult;
                var libvlccorePath = LibVLCCorePath(libvlcDirectoryPath!);
                loadResult = LoadNativeLibrary(libvlccorePath, out LibvlccoreHandle);
                if (!loadResult)
                {
                    Log($"Failed to load required native libraries at {libvlccorePath}");
                    return;
                }

                var libvlcPath = LibVLCPath(libvlcDirectoryPath!);
                loadResult = LoadNativeLibrary(libvlcPath, out LibvlcHandle);
                if (!loadResult)
                    Log($"Failed to load required native libraries at {libvlcPath}");
                return;
            }

            var paths = ComputeLibVLCSearchPaths();

            foreach (var (libvlccore, libvlc) in paths)
            {
                LoadNativeLibrary(libvlccore, out LibvlccoreHandle);
                var loadResult = LoadNativeLibrary(libvlc, out LibvlcHandle);
                if (loadResult)
                    break;
            }

            if (!Loaded)
            {
                throw new VLCException("Failed to load required native libraries. " +
                    $"{Environment.NewLine}Have you installed the latest LibVLC package from nuget for your target platform?" +
                    $"{Environment.NewLine}Search paths include {string.Join($"; {Environment.NewLine}", paths.Select(p => $"{p.libvlc},{p.libvlccore}"))}");
            }
        }
#endif
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
            if (PlatformHelper.IsMac)
            {
                handle = Native.Dlopen(nativeLibraryPath);
            }
            else
            {
                handle = Native.LoadLibraryW(nativeLibraryPath);
            }

            return handle != IntPtr.Zero;
        }

    }
}
