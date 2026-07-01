using LibVLCSharp.Helpers;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// The renderer discoverer can be used to find and use a Chromecast or other distant renderers.
    /// </summary>
    public class RendererDiscoverer : Internal
    {
        RendererDiscovererEventManager? _eventManager;
        const string Bonjour = "Bonjour_renderer";
        const string Mdns = "microdns_renderer";

        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_new")]
            internal static extern IntPtr LibVLCRendererDiscovererNew(IntPtr libvlc, IntPtr name, IntPtr cbs, IntPtr cbsOpaque);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_destroy")]
            internal static extern void LibVLCRendererDiscovererDestroy(IntPtr rendererDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_start")]
            internal static extern int LibVLCRendererDiscovererStart(IntPtr rendererDiscoverer);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_stop")]
            internal static extern void LibVLCRendererDiscovererStop(IntPtr rendererDiscoverer);
        }

        /// <summary>
        /// Create a new renderer discoverer with a LibVLC and protocol name depending on host platform
        /// </summary>
        /// <param name="libVLC">libvlc instance this will be connected to</param>
        /// <param name="name">
        /// The service discovery protocol name depending on platform. Use <see cref="LibVLC.RendererList"/> to find the one for your platform,
        /// or let libvlcsharp find it for you
        /// </param>
        public RendererDiscoverer(LibVLC libVLC, string? name = null)
            : this(libVLC, name, new RendererDiscovererEventManager())
        {
        }

        RendererDiscoverer(LibVLC libVLC, string? name, RendererDiscovererEventManager eventManager)
            : base(() =>
            {
                if(string.IsNullOrEmpty(name))
                {
#if APPLE
                    name = Bonjour;
#else
                    name = Mdns;
#endif
                }

                var nameUtf8 = name.ToUtf8();
                return MarshalUtils.PerformInteropAndFree(() =>
                    Native.LibVLCRendererDiscovererNew(libVLC.NativeReference, nameUtf8, RendererDiscovererCallbacks.Pointer, eventManager.Register()), nameUtf8);
            }, Native.LibVLCRendererDiscovererDestroy)
        {
            _eventManager = eventManager;
        }

        RendererDiscovererEventManager EventManager => _eventManager!;

        /// <summary>
        /// Start the renderer discovery
        /// </summary>
        /// <returns>true if start successful</returns>
        public bool Start() => Native.LibVLCRendererDiscovererStart(NativeReference) == 0;

        /// <summary>
        /// Stop the renderer discovery
        /// </summary>
        public void Stop() => Native.LibVLCRendererDiscovererStop(NativeReference);

        /// <summary>
        /// Dispose override. Releases the native discoverer then frees the callbacks opaque handle.
        /// </summary>
        /// <param name="disposing">true if called from a method</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                // The native discoverer has been destroyed above, so it no longer references the cbs_opaque handle.
                _eventManager?.Unregister();
            }
        }

        /// <summary>
        /// Raised when a renderer item has been found
        /// </summary>
        public event EventHandler<RendererDiscovererItemAddedEventArgs> ItemAdded
        {
            add => EventManager.ItemAdded += value;
            remove => EventManager.ItemAdded -= value;
        }

        /// <summary>
        /// Raised when a renderer item has disappeared
        /// </summary>
        public event EventHandler<RendererDiscovererItemDeletedEventArgs> ItemDeleted
        {
            add => EventManager.ItemDeleted += value;
            remove => EventManager.ItemDeleted -= value;
        }
    }

    /// <summary>
    /// A renderer item represents a device that libvlc can use to render media.
    /// </summary>
    public class RendererItem : Internal
    {
        const int VideoRenderer = 0x0002;
        const int AudioRenderer = 0x0001;

        readonly struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_name")]
            internal static extern IntPtr LibVLCRendererItemName(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_release")]
            internal static extern void LibVLCRendererItemRelease(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_retain")]
            internal static extern IntPtr LibVLCRendererItemRetain(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_type")]
            internal static extern IntPtr LibVLCRendererItemType(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_icon_uri")]
            internal static extern IntPtr LibVLCRendererItemIconUri(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_flags")]
            internal static extern int LibVLCRendererItemFlags(IntPtr rendererItem);
        }

        internal RendererItem(IntPtr reference) : 
            base(() => reference, Native.LibVLCRendererItemRelease)
        {
            Native.LibVLCRendererItemRetain(reference);
        }

        /// <summary>
        /// Name of the renderer item
        /// </summary>
        public string Name => Native.LibVLCRendererItemName(NativeReference).FromUtf8()!;

        /// <summary>
        /// Type of the renderer item
        /// </summary>
        public string Type => Native.LibVLCRendererItemType(NativeReference).FromUtf8()!;

        /// <summary>
        /// IconUri of the renderer item
        /// </summary>
        public string? IconUri => Native.LibVLCRendererItemIconUri(NativeReference).FromUtf8();

        /// <summary>
        /// true if the renderer item can render video
        /// </summary>
        public bool CanRenderVideo => (Native.LibVLCRendererItemFlags(NativeReference) & VideoRenderer) != 0;

        /// <summary>
        /// true if the renderer item can render audio
        /// </summary>
        public bool CanRenderAudio => (Native.LibVLCRendererItemFlags(NativeReference) & AudioRenderer) != 0;
    }
}
