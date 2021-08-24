#if ANDROID
using System;
using System.Runtime.InteropServices;
using LibVLCSharp.Shared.Helpers;
using Java.Interop;

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
            [DllImport(Constants.LibraryName, EntryPoint = "JNI_OnLoad")]
            internal static extern int JniOnLoad(IntPtr javaVm, IntPtr reserved = default);
        }

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
            InitializeAndroid();

            EnsureVersionsMatch();

            LibVLCLoaded = true;
        }

        static void LoadLibCpp()
        {
            try
            {
                Java.Lang.JavaSystem.LoadLibrary("c++_shared");
            }
            catch (Java.Lang.UnsatisfiedLinkError exception)
            {
                throw new VLCException($"failed to load libc++_shared {nameof(exception)} {exception.Message}" +
                                       $"{Environment.NewLine}" +
                                       $"Please make sure you have installed the VideoLAN.LibVLC.Android NuGet");
            }
        }
        static void InitializeAndroid()
        {
            LoadLibCpp();

            var initLibvlc = Native.JniOnLoad(JniRuntime.CurrentRuntime.InvocationPointer);
            if (initLibvlc == -1)
                throw new VLCException("failed to initialize libvlc with JniOnLoad " +
                                       $"{nameof(JniRuntime.CurrentRuntime.InvocationPointer)}: {JniRuntime.CurrentRuntime.InvocationPointer}");
        }
    }
}
#endif
