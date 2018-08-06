﻿using System;
using System.Runtime.InteropServices;
using System.Security;
#if IOS
using ObjCRuntime;
#endif

namespace LibVLCSharp.Shared
{
    public class MediaList : Internal
    {
        readonly object _syncLock = new object();
        bool _nativeLock;
#if IOS
        static MediaList _mediaList;
#endif

        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_subitems")]
            internal static extern IntPtr LibVLCMediaSubitems(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_new")]
            internal static extern IntPtr LibVLCMediaListNew(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_release")]
            internal static extern void LibVLCMediaListRelease(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_media_list")]
            internal static extern IntPtr LibVLCMediaDiscovererMediaList(IntPtr discovererMediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_library_media_list")]
            internal static extern IntPtr LibVLCMediaLibraryMediaList(IntPtr libraryMediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_set_media")]
            internal static extern void LibVLCMediaListSetMedia(IntPtr mediaList, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_add_media")]
            internal static extern int LibVLCMediaListAddMedia(IntPtr mediaList, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_insert_media")]
            internal static extern int LibVLCMediaListInsertMedia(IntPtr mediaList, IntPtr media, int positionIndex);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_remove_index")]
            internal static extern int LibVLCMediaListRemoveIndex(IntPtr mediaList, int positionIndex);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_count")]
            internal static extern int LibVLCMediaListCount(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_item_at_index")]
            internal static extern IntPtr LibVLCMediaListItemAtIndex(IntPtr mediaList, int positionIndex);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_index_of_item")]
            internal static extern int LibVLCMediaListIndexOfItem(IntPtr mediaList, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_is_readonly")]
            internal static extern int LibVLCMediaListIsReadonly(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_lock")]
            internal static extern void LibVLCMediaListLock(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_unlock")]
            internal static extern void LibVLCMediaListUnlock(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_event_manager")]
            internal static extern IntPtr LibVLCMediaListEventManager(IntPtr mediaList);
        }

        /// <summary>
        /// Get subitems of media descriptor object.
        /// </summary>
        /// <param name="media"></param>
        public MediaList(Media media)
            : base(() => Native.LibVLCMediaSubitems(media.NativeReference), Native.LibVLCMediaListRelease,
                   Native.LibVLCMediaListEventManager)
        {
#if IOS
            _mediaList = this;
#endif
        }

        /// <summary>
        /// Get media service discover media list.
        /// </summary>
        /// <param name="mediaDiscoverer"></param>
        public MediaList(MediaDiscoverer mediaDiscoverer)
            : base(() => Native.LibVLCMediaDiscovererMediaList(mediaDiscoverer.NativeReference),
                   Native.LibVLCMediaListRelease, Native.LibVLCMediaListEventManager)
        {
#if IOS
            _mediaList = this;
#endif
        }

        /// <summary>
        /// Create an empty media list.
        /// </summary>
        /// <param name="libVLC"></param>
        public MediaList(LibVLC libVLC)
            : base(() => Native.LibVLCMediaListNew(libVLC.NativeReference), Native.LibVLCMediaListRelease,
                   Native.LibVLCMediaListEventManager)

        {
#if IOS
            _mediaList = this;
#endif        
        }

        public MediaList(IntPtr mediaListPtr)
            : base(() => mediaListPtr, Native.LibVLCMediaListRelease, Native.LibVLCMediaListEventManager)
        {
#if IOS
            _mediaList = this;
#endif
        }

        /// <summary>
        /// Associate media instance with this media list instance. If another
        /// media instance was present it will be released. The
        /// MediaList lock should NOT be held upon entering this function.
        /// </summary>
        /// <param name="media">media instance to add</param>
        public void SetMedia(Media media)
        {
            Native.LibVLCMediaListSetMedia(NativeReference, media.NativeReference);
        }

        /// <summary>
        /// Add media instance to media list The MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="media">a media instance</param>
        /// <returns></returns>
        public bool AddMedia(Media media)
        {
            lock (_syncLock)
            {
                if (!_nativeLock)
                    throw new InvalidOperationException("not locked");
                return Native.LibVLCMediaListAddMedia(NativeReference, media.NativeReference) == 0;
            }
        }

        /// <summary>
        /// Insert media instance in media list on a position The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="media">a media instance</param>
        /// <param name="position">position in array where to insert</param>
        /// <returns></returns>
        public bool InsertMedia(Media media, int position)
        {
            lock (_syncLock)
            {
                if (!_nativeLock)
                    throw new InvalidOperationException("not locked");
                return Native.LibVLCMediaListInsertMedia(NativeReference, media.NativeReference, position) == 0;
            }
        }

        /// <summary>
        /// Remove media instance from media list on a position The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="positionIndex">position in array where to insert</param>
        /// <returns></returns>
        public bool RemoveIndex(int positionIndex)
        {
            lock (_syncLock)
            {
                if (!_nativeLock)
                    throw new InvalidOperationException("not locked");
                return Native.LibVLCMediaListRemoveIndex(NativeReference, positionIndex) == 0;
            }
        }

        /// <summary>
        /// Get count on media list items The MediaList lock should be
        /// held upon entering this function.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncLock)
                {
                    if (!_nativeLock)
                        throw new InvalidOperationException("not locked");
                    return Native.LibVLCMediaListCount(NativeReference);
                }
            }
        }

        /// <summary>
        /// List media instance in media list at a position The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="position">position in array where to insert</param>
        /// <returns>media instance at position, or null if not found.</returns>
        public Media this[int position]
        {
            get
            {
                lock (_syncLock)
                {
                    if (!_nativeLock)
                        throw new InvalidOperationException("not locked");
                    var ptr = Native.LibVLCMediaListItemAtIndex(NativeReference, position);
                    return ptr == IntPtr.Zero ? null : new Media(ptr);
                }
            }
        }

        /// <summary>
        /// Find index position of List media instance in media list. Warning: the
        /// function will return the first matched position. The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="media">media instance</param>
        /// <returns>position of media instance or -1 if media not found</returns>
        public int IndexOf(Media media)
        {
            lock (_syncLock)
            {
                if (!_nativeLock)
                    throw new InvalidOperationException("not locked");
                return Native.LibVLCMediaListIndexOfItem(NativeReference, media.NativeReference);
            }
        }

        /// <summary>
        /// This indicates if this media list is read-only from a user point of view
        /// true if readonly, false otherwise
        /// </summary>
        public bool IsReadonly => Native.LibVLCMediaListIsReadonly(NativeReference) == 1;

        /// <summary>
        /// Get lock on media list items
        /// </summary>
        public void Lock()
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
        public void Unlock()
        {
            lock (_syncLock)
            {
                if (!_nativeLock)
                    throw new InvalidOperationException("not locked");

                _nativeLock = false;
                Native.LibVLCMediaListUnlock(NativeReference);
            }
        }

        #region Events

        readonly object _lock = new object();

#if IOS
        static EventHandler<MediaListItemAddedEventArgs> _mediaListItemAdded;
        static EventHandler<MediaListWillAddItemEventArgs> _mediaListWillAddItem;
        static EventHandler<MediaListItemDeletedEventArgs> _mediaListItemDeleted;
        static EventHandler<MediaListWillDeleteItemEventArgs> _mediaListWillDeleteItem;
        static EventHandler<EventArgs> _mediaListEndReached;
#else
        EventHandler<MediaListItemAddedEventArgs> _mediaListItemAdded;
        EventHandler<MediaListWillAddItemEventArgs> _mediaListWillAddItem;
        EventHandler<MediaListItemDeletedEventArgs> _mediaListItemDeleted;
        EventHandler<MediaListWillDeleteItemEventArgs> _mediaListWillDeleteItem;
        EventHandler<EventArgs> _mediaListEndReached;
#endif

        public event EventHandler<MediaListItemAddedEventArgs> ItemAdded
        {
            add
            {
                lock (_lock)
                {
                    _mediaListItemAdded += value;
                    AttachEvent(EventType.MediaListItemAdded, OnItemAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListItemAdded -= value;
                    DetachEvent(EventType.MediaListItemAdded, OnItemAdded);
                }
            }
        }
        public event EventHandler<MediaListWillAddItemEventArgs> WillAddItem
        {
            add
            {
                lock (_lock)
                {
                    _mediaListWillAddItem += value;
                    AttachEvent(EventType.MediaListWillAddItem, OnWillAddItem);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListWillAddItem -= value;
                    DetachEvent(EventType.MediaListWillAddItem, OnWillAddItem);
                }
            }
        }

        public event EventHandler<MediaListItemDeletedEventArgs> ItemDeleted
        {
            add
            {
                lock (_lock)
                {
                    _mediaListItemDeleted += value;
                    AttachEvent(EventType.MediaListItemDeleted, OnItemDeleted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListItemDeleted -= value;
                    DetachEvent(EventType.MediaListItemDeleted, OnItemDeleted);
                }
            }
        }

        public event EventHandler<MediaListWillDeleteItemEventArgs> WillDeleteItem
        {
            add
            {
                lock (_lock)
                {
                    _mediaListWillDeleteItem += value;
                    AttachEvent(EventType.MediaListWillDeleteItem, OnWillDeleteItem);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListWillDeleteItem -= value;
                    DetachEvent(EventType.MediaListWillDeleteItem, OnWillDeleteItem);
                }
            }
        }

        // v3
        public event EventHandler<EventArgs> EndReached
        {
            add
            {
                lock (_lock)
                {
                    _mediaListEndReached += value;
                    AttachEvent(EventType.MediaPlayerEndReached, OnEndReached);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListEndReached -= value;
                    DetachEvent(EventType.MediaPlayerEndReached, OnEndReached);
                }
            }
        }
#if IOS
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemAdded(IntPtr ptr)
        {
            var itemAdded = RetrieveEvent(ptr).Union.MediaListItemAdded;

            _mediaListItemAdded?.Invoke(_mediaList,
                new MediaListItemAddedEventArgs(new Media(itemAdded.MediaInstance), itemAdded.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnWillAddItem(IntPtr ptr)
        {
            var willAddItem = RetrieveEvent(ptr).Union.MediaListWillAddItem;

            _mediaListWillAddItem?.Invoke(_mediaList,
                new MediaListWillAddItemEventArgs(new Media(willAddItem.MediaInstance), willAddItem.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemDeleted(IntPtr ptr)
        {
            var itemDeleted = RetrieveEvent(ptr).Union.MediaListItemDeleted;

            _mediaListItemDeleted?.Invoke(_mediaList,
                new MediaListItemDeletedEventArgs(new Media(itemDeleted.MediaInstance), itemDeleted.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnWillDeleteItem(IntPtr ptr)
        {
            var willDeleteItem = RetrieveEvent(ptr).Union.MediaListWillDeleteItem;

            _mediaListWillDeleteItem?.Invoke(_mediaList,
                new MediaListWillDeleteItemEventArgs(new Media(willDeleteItem.MediaInstance), willDeleteItem.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnEndReached(IntPtr ptr)
        {
            _mediaListEndReached?.Invoke(_mediaList, EventArgs.Empty);
        }
#else
        void OnItemAdded(IntPtr ptr)
        {
            var itemAdded = RetrieveEvent(ptr).Union.MediaListItemAdded;

            _mediaListItemAdded?.Invoke(this,
                new MediaListItemAddedEventArgs(new Media(itemAdded.MediaInstance), itemAdded.Index));
        }

        void OnWillAddItem(IntPtr ptr)
        {
            var willAddItem = RetrieveEvent(ptr).Union.MediaListWillAddItem;

            _mediaListWillAddItem?.Invoke(this,
                new MediaListWillAddItemEventArgs(new Media(willAddItem.MediaInstance), willAddItem.Index));
        }

        void OnItemDeleted(IntPtr ptr)
        {
            var itemDeleted = RetrieveEvent(ptr).Union.MediaListItemDeleted;

            _mediaListItemDeleted?.Invoke(this,
                new MediaListItemDeletedEventArgs(new Media(itemDeleted.MediaInstance), itemDeleted.Index));
        }

        void OnWillDeleteItem(IntPtr ptr)
        {
            var willDeleteItem = RetrieveEvent(ptr).Union.MediaListWillDeleteItem;

            _mediaListWillDeleteItem?.Invoke(this,
                new MediaListWillDeleteItemEventArgs(new Media(willDeleteItem.MediaInstance), willDeleteItem.Index));
        }

        void OnEndReached(IntPtr ptr)
        {
            _mediaListEndReached?.Invoke(this, EventArgs.Empty);
        }
#endif

        #endregion
    }
}