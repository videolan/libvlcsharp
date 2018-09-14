using System;
using System.Runtime.InteropServices;
using System.Security;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// libvlc v3 check
    /// </summary>
    public class MediaDiscoverer : Internal
    {
        MediaList _mediaList;

        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_new")]
            internal static extern IntPtr LibVLCMediaDiscovererNew(IntPtr libvlc, [MarshalAs(UnmanagedType.LPStr)] string name);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_start")]
            internal static extern int LibVLCMediaDiscovererStart(IntPtr mediaDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_stop")]
            internal static extern void LibVLCMediaDiscovererStop(IntPtr mediaDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_release")]
            internal static extern void LibVLCMediaDiscovererRelease(IntPtr mediaDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_localized_name")]
            internal static extern IntPtr LibVLCMediaDiscovererLocalizedName(IntPtr mediaDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_event_manager")]
            internal static extern IntPtr LibVLCMediaDiscovererEventManager(IntPtr mediaDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                            EntryPoint = "libvlc_media_discoverer_is_running")]
            internal static extern int LibVLCMediaDiscovererIsRunning(IntPtr mediaDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_media_list")]
            internal static extern IntPtr LibVLCMediaDiscovererMediaList(IntPtr discovererMediaList);
        }

        /// <summary>Category of a media discoverer</summary>
        /// <remarks>libvlc_media_discoverer_list_get()</remarks>
        public enum Category
        {
            /// <summary>devices, like portable music player</summary>
            Devices = 0,
            /// <summary>LAN/WAN services, like Upnp, SMB, or SAP</summary>
            Lan = 1,
            /// <summary>Podcasts</summary>
            Podcasts = 2,
            /// <summary>Local directories, like Video, Music or Pictures directories</summary>
            Localdirs = 3
        }

        public struct Description
        {
            public Description(string name, string longName, Category category)
            {
                Name = name;
                LongName = longName;
                Category = category;
            }

            public string Name { get; }
            public string LongName { get; }
            public Category Category { get; }
        }

        public MediaDiscoverer(LibVLC libVLC, string name)
            //v3 check. differen ctors
            : base(() => Native.LibVLCMediaDiscovererNew(libVLC.NativeReference, name), Native.LibVLCMediaDiscovererRelease,
                   Native.LibVLCMediaDiscovererEventManager)
        {
        }

        /// <summary>
        /// Start media discovery.
        /// To stop it, call MediaDiscover::stop() or destroy the object directly.
        /// </summary>
        /// <returns>false in case of error, true otherwise</returns>
        public bool Start() => Native.LibVLCMediaDiscovererStart(NativeReference) == 0;

        /// <summary>
        /// Stop media discovery.
        /// </summary>
        public void Stop() => Native.LibVLCMediaDiscovererStop(NativeReference);

        /// <summary>
        /// Get media service discover object its localized name.
        /// under v3 only
        /// </summary>
        public string LocalizedName => (string)Utf8StringMarshaler.GetInstance()
            .MarshalNativeToManaged(Native.LibVLCMediaDiscovererLocalizedName(NativeReference));

        /// <summary>
        /// Query if media service discover object is running.
        /// </summary>
        public bool IsRunning => Native.LibVLCMediaDiscovererIsRunning(NativeReference) != 0;

        public MediaList MediaList
        {
            get
            {
                if (_mediaList == null)
                {
                    var ptr = Native.LibVLCMediaDiscovererMediaList(NativeReference);
                    if (ptr == IntPtr.Zero) return null;
                    _mediaList = new MediaList(ptr);
                }
                return _mediaList;
            }
        }

        #region Events

        readonly object _lock = new object();

        EventHandler<EventArgs> _mediaDiscovererStarted;
        EventHandler<EventArgs> _mediaDiscovererStopped;

        // v3
        public event EventHandler<EventArgs> Started
        {
            add
            {
                lock (_lock)
                {
                    _mediaDiscovererStarted += value;
                    AttachEvent(EventType.MediaDiscovererStarted, OnStarted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaDiscovererStarted -= value;
                    DetachEvent(EventType.MediaDiscovererStarted, OnStarted);
                }
            }
        }

        // v3
        public event EventHandler<EventArgs> Stopped
        {
            add
            {
                lock (_lock)
                {
                    _mediaDiscovererStopped += value;
                    AttachEvent(EventType.MediaDiscovererStopped, OnStopped);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaDiscovererStopped -= value;
                    DetachEvent(EventType.MediaDiscovererStopped, OnStopped);
                }
            }
        }

        //[MonoPInvokeCallback(typeof(EventCallback))]
        void OnStarted(IntPtr ptr)
        {
            _mediaDiscovererStarted?.Invoke(null, EventArgs.Empty);
        }

        //[MonoPInvokeCallback(typeof(EventCallback))]
        void OnStopped(IntPtr ptr)
        {
            _mediaDiscovererStopped?.Invoke(null, EventArgs.Empty);
        }
        #endregion
    }
}
