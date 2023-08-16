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
        MediaList? _mediaList;

        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_new")]
            internal static extern IntPtr LibVLCMediaDiscovererNew(IntPtr libvlc, IntPtr name);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_start")]
            internal static extern int LibVLCMediaDiscovererStart(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_stop")]
            internal static extern void LibVLCMediaDiscovererStop(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_release")]
            internal static extern void LibVLCMediaDiscovererRelease(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                            EntryPoint = "libvlc_media_discoverer_is_running")]
            internal static extern bool LibVLCMediaDiscovererIsRunning(IntPtr mediaDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_discoverer_media_list")]
            internal static extern IntPtr LibVLCMediaDiscovererMediaList(IntPtr discovererMediaList);
        }

        /// <summary>
        /// Media discoverer constructor
        /// </summary>
        /// <param name="libVLC">libvlc instance this will be attached to</param>
        /// <param name="name">name from one of LibVLC.MediaDiscoverers</param>
        public MediaDiscoverer(LibVLC libVLC, string name)
            : base(() =>
            {
                var nameUtf8 = name.ToUtf8();
                return MarshalUtils.PerformInteropAndFree(() =>
                    Native.LibVLCMediaDiscovererNew(libVLC.NativeReference, nameUtf8), nameUtf8);
            }, Native.LibVLCMediaDiscovererRelease)
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
        /// Query if media service discover object is running.
        /// </summary>
        public bool IsRunning => NativeReference != IntPtr.Zero && Native.LibVLCMediaDiscovererIsRunning(NativeReference);

        /// <summary>
        /// The MediaList attached to this MediaDiscoverer
        /// </summary>
        public MediaList? MediaList
        {
            get
            {
                if (_mediaList == null)
                {
                    if (IsDisposed || NativeReference == IntPtr.Zero) return null;

                    var ptr = Native.LibVLCMediaDiscovererMediaList(NativeReference);
                    if (ptr == IntPtr.Zero) return null;
                    _mediaList = new MediaList(ptr);
                }
                return _mediaList;
            }
        }

        /// <summary>
        /// Dispose of this media discoverer
        /// </summary>
        /// <param name="disposing">true if called from a method</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_mediaList != null)
                {
                    _mediaList.Dispose();
                    _mediaList = null;
                }

                if(IsRunning)
                {
                    Stop();
                }
            }

            base.Dispose(disposing);
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
