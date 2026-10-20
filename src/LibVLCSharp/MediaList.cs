using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// The MediaList holds a list of Media types
    /// </summary>
    /// <remarks>
    /// Media returned by the indexer or the enumerator are owned by the list and stay valid until the
    /// list is disposed. Call <see cref="Media.Retain"/> to keep one longer.
    /// </remarks>
    public class MediaList : Internal, IEnumerable<Media>
    {
        readonly object _syncLock = new object();
        bool _nativeLock;
        readonly Dictionary<IntPtr, Media> _items = new Dictionary<IntPtr, Media>();

        readonly struct Native
        {

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_subitems")]
            internal static extern IntPtr LibVLCMediaSubitems(IntPtr media);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_new")]
            internal static extern IntPtr LibVLCMediaListNew();


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_release")]
            internal static extern void LibVLCMediaListRelease(IntPtr mediaList);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_set_media")]
            internal static extern void LibVLCMediaListSetMedia(IntPtr mediaList, IntPtr media);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_add_media")]
            internal static extern int LibVLCMediaListAddMedia(IntPtr mediaList, IntPtr media);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_insert_media")]
            internal static extern int LibVLCMediaListInsertMedia(IntPtr mediaList, IntPtr media, int positionIndex);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_remove_index")]
            internal static extern int LibVLCMediaListRemoveIndex(IntPtr mediaList, int positionIndex);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_count")]
            internal static extern int LibVLCMediaListCount(IntPtr mediaList);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_item_at_index")]
            internal static extern IntPtr LibVLCMediaListItemAtIndex(IntPtr mediaList, int positionIndex);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_index_of_item")]
            internal static extern int LibVLCMediaListIndexOfItem(IntPtr mediaList, IntPtr media);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_is_readonly")]
            internal static extern bool LibVLCMediaListIsReadonly(IntPtr mediaList);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_lock")]
            internal static extern void LibVLCMediaListLock(IntPtr mediaList);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_unlock")]
            internal static extern void LibVLCMediaListUnlock(IntPtr mediaList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_media")]
            internal static extern IntPtr LibVLCMediaListMedia(IntPtr mediaList);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_retain")]
            internal static extern IntPtr LibVLCMediaListRetain(IntPtr mediaList);
        }

        /// <summary>
        /// Get subitems of media descriptor object.
        /// </summary>
        /// <param name="media"></param>
        public MediaList(Media media)
            : base(() => Native.LibVLCMediaSubitems(media.NativeReference), Native.LibVLCMediaListRelease)
        {
        }

        /// <summary>
        /// Create an empty media list.
        /// </summary>
        public MediaList()
            : base(() => Native.LibVLCMediaListNew(), Native.LibVLCMediaListRelease)
        {
        }

        internal MediaList(IntPtr mediaListPtr) : base(() => mediaListPtr, Native.LibVLCMediaListRelease)
        {
        }

        /// <summary>
        /// Associate media instance with this media list instance. If another
        /// media instance was present it will be released.
        /// </summary>
        /// <param name="media">media instance to add</param>
        public void SetMedia(Media media) => Native.LibVLCMediaListSetMedia(NativeReference, media.NativeReference);

        /// <summary>
        /// Add media instance to media list
        /// </summary>
        /// <param name="media">a media instance</param>
        /// <returns>true on success, false if the media list is read-only</returns>
        public bool AddMedia(Media media)
        {
            if (media == null) throw new ArgumentNullException(nameof(media));

            var added = NativeSync(() => Native.LibVLCMediaListAddMedia(NativeReference, media.NativeReference) == 0);
            return added;
        }

        T NativeSync<T>(Func<T> operation)
        {
            try
            {
                lock (_syncLock)
                {
                    if (!_nativeLock)
                        Lock();
                    return operation();
                }
            }
            finally
            {
                lock (_syncLock)
                {
                    if (_nativeLock)
                        Unlock();
                }
            }
        }

        /// <summary>
        /// Insert media instance in media list on a position.
        /// </summary>
        /// <param name="media">a media instance</param>
        /// <param name="position">position in the array where to insert</param>
        /// <returns>true on success, false if the media list is read-only</returns>
        public bool InsertMedia(Media media, int position)
        {
            if (media == null) throw new ArgumentNullException(nameof(media));

            var inserted = NativeSync(() => Native.LibVLCMediaListInsertMedia(NativeReference, media.NativeReference, position) == 0);
            return inserted;
        }

        /// <summary>
        /// Remove media instance from media list on a position.
        /// </summary>
        /// <param name="positionIndex">position in the array where to remove the iteam</param>
        /// <returns>true on success, false if the media list is read-only or the item was not found</returns>
        public bool RemoveIndex(int positionIndex)
        {
            var removed = NativeSync(() => Native.LibVLCMediaListRemoveIndex(NativeReference, positionIndex) == 0);
            return removed;
        }

        /// <summary>
        /// Get count on media list items.
        /// </summary>
        public int Count => NativeSync(() => Native.LibVLCMediaListCount(NativeReference));

        /// <summary>
        /// Gets the element at the specified index.
        /// The media is owned by the list and stays valid until the list is disposed;
        /// call <see cref="Media.Retain"/> to keep it longer.
        /// </summary>
        /// <param name="position">position of the media in the list</param>
        /// <returns>media instance at position, or null if not found</returns>
        public Media? this[int position] => NativeSync(() =>
        {
            var ptr = Native.LibVLCMediaListItemAtIndex(NativeReference, position);
            if (ptr == IntPtr.Zero)
                return null;

            // item_at_index retained the media; the cached wrapper already holds a reference.
            if (_items.TryGetValue(ptr, out var cached) && cached.NativeReference != IntPtr.Zero)
            {
                Media.Native.LibVLCMediaRelease(ptr);
                return cached;
            }

            var media = new Media(ptr);
            _items[ptr] = media;
            return media;
        });

        /// <summary>
        /// Find index position of List media instance in media list. Warning: the
        /// function will return the first matched position.
        /// </summary>
        /// <param name="media">media instance</param>
        /// <returns>position of media instance or -1 if media not found</returns>
        public int IndexOf(Media media) => NativeSync(() => Native.LibVLCMediaListIndexOfItem(NativeReference, media.NativeReference));

        /// <summary>
        /// This indicates if this media list is read-only from a user point of view.
        /// True if readonly, false otherwise
        /// </summary>
        public bool IsReadonly => Native.LibVLCMediaListIsReadonly(NativeReference);

        /// <summary>
        /// Get lock on media list items
        /// </summary>
        internal void Lock()
        {
            lock (_syncLock)
            {
                if (_nativeLock)
                    throw new InvalidOperationException("already locked");

                _nativeLock = true;
                Native.LibVLCMediaListLock(NativeReference);
            }
        }

        /// <summary>
        /// Release lock on media list items The MediaList lock should be held upon entering this function.
        /// </summary>
        internal void Unlock()
        {
            lock (_syncLock)
            {
                if (!_nativeLock)
                    throw new InvalidOperationException("not locked");

                _nativeLock = false;
                Native.LibVLCMediaListUnlock(NativeReference);
            }
        }

        /// <summary>
        /// Get the media instance associated with this media list instance, if any.
        /// This is the inverse of <see cref="SetMedia(Media)"/>.
        /// Each call increments the native media reference count and returns a new
        /// <see cref="Media"/> instance, so dispose it when done.
        /// </summary>
        /// <returns>the associated media, or null if none</returns>
        public Media? GetAssociatedMedia()
        {
            var ptr = Native.LibVLCMediaListMedia(NativeReference);
            return ptr == IntPtr.Zero ? null : new Media(ptr);
        }

        /// <summary>Increments the native reference counter for this medialist instance</summary>
        internal void Retain() => Native.LibVLCMediaListRetain(NativeReference);

        /// <summary>
        /// Release the media handed out by this list, then the list itself
        /// </summary>
        /// <param name="disposing">true if called from a method</param>
        protected override void Dispose(bool disposing)
        {
            lock (_syncLock)
            {
                foreach (var media in _items.Values)
                    media.Dispose();
                _items.Clear();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection of media
        /// </summary>
        /// <returns>an enumerator over a media collection</returns>
        public IEnumerator<Media> GetEnumerator() => new MediaListEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal class MediaListEnumerator : IEnumerator<Media>
        {
            int position = -1;
            MediaList? _mediaList;

            internal MediaListEnumerator(MediaList mediaList)
            {
                _mediaList = mediaList;
            }

            public bool MoveNext()
            {
                position++;
                return position < (_mediaList?.Count ?? 0);
            }

            void IEnumerator.Reset()
            {
                position = -1;
            }

            public void Dispose()
            {
                position = -1;
                _mediaList = default;
            }

            object IEnumerator.Current => Current;

            public Media Current
            {
                get
                {
                    if (_mediaList == null)
                    {
                        throw new ObjectDisposedException(nameof(MediaListEnumerator));
                    }
                    return _mediaList[position] ?? throw new ArgumentOutOfRangeException(nameof(position));
                }
            }
        }
    }
}
