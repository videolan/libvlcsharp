using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// The MediaList holds a list of Media types
    /// </summary>
    public class MediaList : Internal, IEnumerable<Media>
    {
        MediaListEventManager? _eventManager;
        readonly object _syncLock = new object();
        bool _nativeLock;

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

            var index = Count;
            EventManager.OnWillAddItem(media, index);
            var added = NativeSync(() => Native.LibVLCMediaListAddMedia(NativeReference, media.NativeReference) == 0);
            if (added)
                EventManager.OnItemAdded(media, index);
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

            EventManager.OnWillAddItem(media, position);
            var inserted = NativeSync(() => Native.LibVLCMediaListInsertMedia(NativeReference, media.NativeReference, position) == 0);
            if (inserted)
                EventManager.OnItemAdded(media, position);
            return inserted;
        }

        /// <summary>
        /// Remove media instance from media list on a position.
        /// </summary>
        /// <param name="positionIndex">position in the array where to remove the iteam</param>
        /// <returns>true on success, false if the media list is read-only or the item was not found</returns>
        public bool RemoveIndex(int positionIndex)
        {
            var media = this[positionIndex];
            if (media == null)
                return false;

            EventManager.OnWillDeleteItem(media, positionIndex);
            var removed = NativeSync(() => Native.LibVLCMediaListRemoveIndex(NativeReference, positionIndex) == 0);
            if (removed)
                EventManager.OnItemDeleted(media, positionIndex);
            return removed;
        }

        /// <summary>
        /// Get count on media list items.
        /// </summary>
        public int Count => NativeSync(() => Native.LibVLCMediaListCount(NativeReference));

        /// <summary>
        /// Gets the element at the specified index
        /// </summary>
        /// <param name="position">position in array where to insert</param>
        /// <returns>media instance at position, or null if not found.
        /// In case of success, Media.Retain() is called to increase the refcount on the media. </returns>
        public Media? this[int position] => NativeSync(() =>
        {
            var ptr = Native.LibVLCMediaListItemAtIndex(NativeReference, position);
            return ptr == IntPtr.Zero ? null : new Media(ptr);
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
        /// The native reference count of the returned media is incremented, so dispose it when done.
        /// </summary>
        public Media? AssociatedMedia
        {
            get
            {
                var ptr = Native.LibVLCMediaListMedia(NativeReference);
                return ptr == IntPtr.Zero ? null : new Media(ptr);
            }
        }

        /// <summary>
        /// Managed media list event dispatcher.
        /// LibVLC 4 removed the native media list event manager, so these events no longer fire;
        /// the dispatcher is kept to preserve the public event API for source compatibility.
        /// </summary>
        MediaListEventManager EventManager
        {
            get
            {
                if (_eventManager != null) return _eventManager;
                _eventManager = new MediaListEventManager();
                return _eventManager;
            }
        }

        /// <summary>Increments the native reference counter for this medialist instance</summary>
        internal void Retain() => Native.LibVLCMediaListRetain(NativeReference);

        #region Events

        /// <summary>
        /// An item has been added to the MediaList
        /// </summary>
        public event EventHandler<MediaListItemAddedEventArgs> ItemAdded
        {
            add => EventManager.AddItemAdded(value);
            remove => EventManager.RemoveItemAdded(value);
        }

        /// <summary>
        /// An item is about to be added to the MediaList
        /// </summary>
        public event EventHandler<MediaListWillAddItemEventArgs> WillAddItem
        {
            add => EventManager.AddWillAddItem(value);
            remove => EventManager.RemoveWillAddItem(value);
        }

        /// <summary>
        /// An item has been deleted from the MediaList
        /// </summary>
        public event EventHandler<MediaListItemDeletedEventArgs> ItemDeleted
        {
            add => EventManager.AddItemDeleted(value);
            remove => EventManager.RemoveItemDeleted(value);
        }

        /// <summary>
        /// An item is about to be deleted from the MediaList
        /// </summary>
        public event EventHandler<MediaListWillDeleteItemEventArgs> WillDeleteItem
        {
            add => EventManager.AddWillDeleteItem(value);
            remove => EventManager.RemoveWillDeleteItem(value);
        }

        /// <summary>
        /// The media list reached its end
        /// </summary>
        public event EventHandler<EventArgs> EndReached
        {
            add => EventManager.AddEndReached(value);
            remove => EventManager.RemoveEndReached(value);
        }

        #endregion

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
