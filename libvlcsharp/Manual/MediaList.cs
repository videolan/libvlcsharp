using System;
using System.Runtime.InteropServices;
using System.Security;

namespace VideoLAN.LibVLC.Manual
{
    public class MediaList : Internal
    {
        MediaListEventManager _eventManager;

        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_subitems")]
            internal static extern IntPtr LibVLCMediaSubitems(IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_new")]
            internal static extern IntPtr LibVLCMediaListNew(IntPtr instance);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_release")]
            internal static extern void LibVLCMediaListRelease(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_media_list")]
            internal static extern IntPtr LibVLCMediaDiscovererMediaList(IntPtr discovererMediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_library_media_list")]
            internal static extern IntPtr LibVLCMediaLibraryMediaList(IntPtr libraryMediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_set_media")]
            internal static extern void LibVLCMediaListSetMedia(IntPtr mediaList, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_add_media")]
            internal static extern int LibVLCMediaListAddMedia(IntPtr mediaList, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_insert_media")]
            internal static extern int LibVLCMediaListInsertMedia(IntPtr mediaList, IntPtr media, int positionIndex);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_remove_index")]
            internal static extern int LibVLCMediaListRemoveIndex(IntPtr mediaList, int positionIndex);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_count")]
            internal static extern int LibVLCMediaListCount(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_item_at_index")]
            internal static extern IntPtr LibVLCMediaListItemAtIndex(IntPtr mediaList, int positionIndex);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_index_of_item")]
            internal static extern int LibVLCMediaListIndexOfItem(IntPtr mediaList, IntPtr media);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_is_readonly")]
            internal static extern int LibVLCMediaListIsReadonly(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_lock")]
            internal static extern void LibVLCMediaListLock(IntPtr mediaList);
            
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_unlock")]
            internal static extern void LibVLCMediaListUnlock(IntPtr mediaList);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_list_event_manager")]
            internal static extern IntPtr LibVLCMediaListEventManager(IntPtr mediaList);
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
        /// Get media service discover media list.
        /// </summary>
        /// <param name="mediaDiscoverer"></param>
        public MediaList(MediaDiscoverer mediaDiscoverer)
            : base(() => Native.LibVLCMediaDiscovererMediaList(mediaDiscoverer.NativeReference),
                Native.LibVLCMediaListRelease)
        {
        }

        /// <summary>
        /// Get media library subitems.
        /// </summary>
        /// <param name="mediaList"></param>
        public MediaList(MediaLibrary mediaList) 
            : base(() => Native.LibVLCMediaLibraryMediaList(mediaList.NativeReference), Native.LibVLCMediaListRelease)
        {
        }

        /// <summary>
        /// Create an empty media list.
        /// </summary>
        /// <param name="instance"></param>
        public MediaList(Instance instance)
            : base(() => Native.LibVLCMediaListNew(instance.NativeReference), Native.LibVLCMediaListRelease)
        {
        }
        public MediaList(IntPtr mediaListPtr) : base(() => mediaListPtr, Native.LibVLCMediaListRelease)
        {
        }

        /// <summary>
        /// Associate media instance with this media list instance. If another
        /// media instance was present it will be released. The
        /// MediaList lock should NOT be held upon entering this function.
        /// </summary>
        /// <param name="media">media instance to add</param>
        public void SetMedia(Media media) => Native.LibVLCMediaListSetMedia(NativeReference, media.NativeReference);

        /// <summary>
        /// Add media instance to media list The MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="media">a media instance</param>
        /// <returns></returns>
        public bool AddMedia(Media media) =>
            Native.LibVLCMediaListAddMedia(NativeReference, media.NativeReference) == 0;

        /// <summary>
        /// Insert media instance in media list on a position The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="media">a media instance</param>
        /// <param name="position">position in array where to insert</param>
        /// <returns></returns>
        public bool InsertMedia(Media media, int position) =>
            Native.LibVLCMediaListInsertMedia(NativeReference, media.NativeReference, position) == 0;

        /// <summary>
        /// Remove media instance from media list on a position The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="positionIndex">position in array where to insert</param>
        /// <returns></returns>
        public bool RemoveIndex(int positionIndex)
            => Native.LibVLCMediaListRemoveIndex(NativeReference, positionIndex) == 0;

        /// <summary>
        /// Get count on media list items The MediaList lock should be
        /// held upon entering this function.
        /// </summary>
        public int Count => Native.LibVLCMediaListCount(NativeReference);

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
                var ptr = Native.LibVLCMediaListItemAtIndex(NativeReference, position);
                return ptr == IntPtr.Zero ? null : new Media(ptr);
            }
        }

        /// <summary>
        /// Find index position of List media instance in media list. Warning: the
        /// function will return the first matched position. The
        /// MediaList lock should be held upon entering this function.
        /// </summary>
        /// <param name="media">media instance</param>
        /// <returns>position of media instance or -1 if media not found</returns>
        public int IndexOf(Media media) => Native.LibVLCMediaListIndexOfItem(NativeReference, media.NativeReference);

        /// <summary>
        /// This indicates if this media list is read-only from a user point of view
        /// true if readonly, false otherwise
        /// </summary>
        public bool IsReadonly => Native.LibVLCMediaListIsReadonly(NativeReference) == 1;

        /// <summary>
        /// Get lock on media list items
        /// </summary>
        public void Lock() => Native.LibVLCMediaListLock(NativeReference);

        /// <summary>
        /// Release lock on media list items The MediaList lock should be held upon entering this function.
        /// </summary>
        public void Unlock() => Native.LibVLCMediaListUnlock(NativeReference);

        /// <summary>
        /// Get libvlc_event_manager from this media list instance. The
        /// p_event_manager is immutable, so you don't have to hold the lock
        /// </summary>
        public MediaListEventManager EventManager
        {
            get
            {
                if (_eventManager != null) return _eventManager;
                var ptr = Native.LibVLCMediaListEventManager(NativeReference);
                _eventManager = new MediaListEventManager(ptr);
                return _eventManager;
            }
        }
    }
}