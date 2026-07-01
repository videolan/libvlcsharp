using LibVLCSharp.Helpers;
using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// MediaDiscoverer should be used to find media on NAS and any SMB/UPnP-enabled device on your local network.
    /// </summary>
    public class MediaDiscoverer : Internal
    {
        readonly MediaDiscovererEventManager _eventManager;

        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_new")]
            internal static extern IntPtr LibVLCMediaDiscovererNew(IntPtr libvlc, IntPtr name, IntPtr cbs, IntPtr cbsOpaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_start")]
            internal static extern int LibVLCMediaDiscovererStart(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_stop")]
            internal static extern void LibVLCMediaDiscovererStop(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_destroy")]
            internal static extern void LibVLCMediaDiscovererDestroy(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                            EntryPoint = "libvlc_media_discoverer_is_running")]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool LibVLCMediaDiscovererIsRunning(IntPtr mediaDiscoverer);
        }

        /// <summary>
        /// Media discoverer constructor
        /// </summary>
        /// <param name="libVLC">libvlc instance this will be attached to</param>
        /// <param name="name">name from one of LibVLC.MediaDiscoverers</param>
        public MediaDiscoverer(LibVLC libVLC, string name)
            : this(libVLC, name, new MediaDiscovererEventManager())
        {
        }

        MediaDiscoverer(LibVLC libVLC, string name, MediaDiscovererEventManager eventManager)
            : base(() =>
            {
                var nameUtf8 = name.ToUtf8();
                return MarshalUtils.PerformInteropAndFree(() =>
                    Native.LibVLCMediaDiscovererNew(libVLC.NativeReference, nameUtf8, MediaDiscovererCallbacks.Pointer, eventManager.Register()), nameUtf8);
            }, Native.LibVLCMediaDiscovererDestroy)
        {
            _eventManager = eventManager;
        }

        /// <summary>
        /// Raised when the media discoverer found a new media item.
        /// Starting with LibVLC 4, discovered media are delivered through this event instead of a media list.
        /// </summary>
        public event EventHandler<MediaDiscovererMediaAddedEventArgs> MediaAdded
        {
            add => _eventManager.AddMediaAdded(value);
            remove => _eventManager.RemoveMediaAdded(value);
        }

        /// <summary>
        /// Raised when the media discoverer removed a media item.
        /// </summary>
        public event EventHandler<MediaDiscovererMediaRemovedEventArgs> MediaRemoved
        {
            add => _eventManager.AddMediaRemoved(value);
            remove => _eventManager.RemoveMediaRemoved(value);
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
        /// Query if media service discover object is running.
        /// </summary>
        public bool IsRunning => NativeReference != IntPtr.Zero && Native.LibVLCMediaDiscovererIsRunning(NativeReference);

        /// <summary>
        /// Dispose of this media discoverer
        /// </summary>
        /// <param name="disposing">true if called from a method</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(IsRunning)
                {
                    Stop();
                }
            }

            base.Dispose(disposing);

            if (disposing)
            {
                // The native discoverer has been destroyed above, so it no longer references the cbs_opaque handle.
                _eventManager.Unregister();
            }
        }
    }

    /// <summary>Category of a media discoverer</summary>
    /// <remarks>libvlc_media_discoverer_list_get()</remarks>
    public enum MediaDiscovererCategory
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
}
