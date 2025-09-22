using System;
using System.Runtime.InteropServices;
using LibVLCSharp.Helpers;

namespace LibVLCSharp
{
    /// <summary>
    /// LibVLC Picture.
    /// This type works with the thumbnailer feature.
    /// </summary>
    public class Picture : Internal
    {
        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_retain")]
            internal static extern void LibVLCPictureRetain(IntPtr picture);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_release")]
            internal static extern void LibVLCPictureRelease(IntPtr picture);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_save")]
            internal static extern int LibVLCPictureSave(IntPtr picture, IntPtr path);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_get_buffer")]
            internal static extern IntPtr LibVLCPictureGetBuffer(IntPtr picture, out UIntPtr size);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_type")]
            internal static extern PictureType LibVLCPictureType(IntPtr picture);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_get_stride")]
            internal static extern uint LibVLCPictureGetStride(IntPtr picture);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_get_width")]
            internal static extern uint LibVLCPictureGetWidth(IntPtr picture);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_get_height")]
            internal static extern uint LibVLCPictureGetHeight(IntPtr picture);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_get_time")]
            internal static extern long LibVLCPictureGetTime(IntPtr picture);
        }

        internal Picture(IntPtr pictureReference) : base(() => pictureReference, Native.LibVLCPictureRelease)
        {
            Native.LibVLCPictureRetain(pictureReference);
        }

        /// <summary>
        /// Saves this picture to a file. The image format is the same as the one
        /// returned by Type
        /// </summary>
        /// <param name="path">The path to the generated file</param>
        /// <returns>true in case of success, false otherwise</returns>
        public bool Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return Native.LibVLCPictureSave(NativeReference, path.ToUtf8()) == 0;
        }

        /// <summary>
        /// The image internal buffer, including potential padding.
        /// The Picture owns the returned buffer, which must not be modified nor freed
        /// </summary>
        /// <returns>A tuple which holds the pointer to the internal buffer and the size of the buffer.</returns>
        public (IntPtr buffer, UIntPtr size) Buffer => (Native.LibVLCPictureGetBuffer(NativeReference, out var size), size);

        /// <summary>
        /// The picture type
        /// </summary>
        public PictureType Type => Native.LibVLCPictureType(NativeReference);

        /// <summary>
        /// The image stride, ie. the number of bytes per line.
        /// This can only be called on images of type Argb
        /// </summary>
        public uint Stride
        {
            get => Type != PictureType.Argb ? 0 : Native.LibVLCPictureGetStride(NativeReference);
        }

        /// <summary>
        /// The width of the image in pixels
        /// </summary>
        public uint Width => Native.LibVLCPictureGetWidth(NativeReference);

        /// <summary>
        /// The height of the image in pixels
        /// </summary>
        public uint Height => Native.LibVLCPictureGetHeight(NativeReference);

        /// <summary>
        /// The time at which this picture was generated, in milliseconds
        /// </summary>
        public long Time => Native.LibVLCPictureGetTime(NativeReference);
    }

    /// <summary>
    /// Describes the picture type by libvlc
    /// </summary>
    public enum PictureType
    {
        /// <summary>
        /// Argb picture
        /// </summary>
        Argb,

        /// <summary>
        /// Png picture
        /// </summary>
        Png,

        /// <summary>
        /// Jpg picture
        /// </summary>
        Jpg
    }
}
