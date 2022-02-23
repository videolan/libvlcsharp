using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// The PictureList holds a list of Picture types
    /// </summary>
    public class PictureList : Internal, IEnumerable<Picture>
    {
        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_list_count")]
            internal static extern UIntPtr LibVLCPictureListCount(IntPtr pictureList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_list_at")]
            internal static extern IntPtr LibVLCPictureListAt(IntPtr pictureList, UIntPtr index);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_picture_list_destroy")]
            internal static extern void LibVLCPictureListDestroy(IntPtr pictureList);
        }

        internal PictureList(IntPtr pictureListPtr) : base(() => pictureListPtr, Native.LibVLCPictureListDestroy)
        {
        }

        /// <summary>
        /// Get count on picture list items.
        /// </summary>
        public uint Count => Native.LibVLCPictureListCount(NativeReference).ToUInt32();

        /// <summary>
        /// Gets the element at the specified index
        /// </summary>
        /// <param name="position">position of the desired picture item</param>
        /// <returns>the picture instance or null if not found</returns>
        public Picture? this[int position]
        {
            get 
            {
                var ptr = Native.LibVLCPictureListAt(NativeReference, (UIntPtr)position);
                return ptr == IntPtr.Zero ? null : new Picture(ptr);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection of picture
        /// </summary>
        /// <returns>an enumerator over a picture collection</returns>
        public IEnumerator<Picture> GetEnumerator() => new PictureListEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal class PictureListEnumerator : IEnumerator<Picture>
        {
            int position = -1;
            PictureList? _pictureList;

            internal PictureListEnumerator(PictureList mediaList)
            {
                _pictureList = mediaList;
            }

            public bool MoveNext()
            {
                position++;
                return position < (_pictureList?.Count ?? 0);
            }

            void IEnumerator.Reset()
            {
                position = -1;
            }

            public void Dispose()
            {
                position = -1;
                _pictureList = default;
            }

            object IEnumerator.Current => Current;

            public Picture Current
            {
                get
                {
                    if (_pictureList == null)
                    {
                        throw new ObjectDisposedException(nameof(PictureListEnumerator));
                    }
                    return _pictureList[position] ?? throw new ArgumentOutOfRangeException(nameof(position));
                }
            }
        }
    }
}

