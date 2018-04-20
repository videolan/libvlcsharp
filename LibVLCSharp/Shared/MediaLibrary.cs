using System;
using System.Runtime.InteropServices;
using System.Security;
using LibVLCSharp.Shared;

namespace LibVLCSharp
{
    public class MediaLibrary : Internal
    {
        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_library_new")]
            internal static extern IntPtr LibVLCMediaLibraryNew(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_library_release")]
            internal static extern void LibVLCMediaLibraryRelease(IntPtr mediaLibrary);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_library_load")]
            internal static extern int LibVLCMediaLibraryLoad(IntPtr mediaLibrary);
        }

        public MediaLibrary(Instance instance) 
            : base(() => Native.LibVLCMediaLibraryNew(instance.NativeReference), Native.LibVLCMediaLibraryRelease)
        {
        }

        /// <summary>
        /// Load media library.
        /// </summary>
        /// <returns>true on success</returns>
        public bool Load() => Native.LibVLCMediaLibraryLoad(NativeReference) == 0;
    }
}
