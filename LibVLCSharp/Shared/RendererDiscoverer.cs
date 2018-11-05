using System;
using System.Runtime.InteropServices;
using System.Security;

namespace LibVLCSharp.Shared
{
    public class RendererDiscoverer : Internal
    {
        RendererDiscovererEventManager _eventManager;

        struct Native
        {

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_new")]
            internal static extern IntPtr LibVLCRendererDiscovererNew(IntPtr libvlc, string name);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_release")]
            internal static extern void LibVLCRendererDiscovererRelease(IntPtr rendererDiscoverer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_start")]
            internal static extern int LibVLCRendererDiscovererStart(IntPtr rendererDiscoverer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_stop")]
            internal static extern void LibVLCRendererDiscovererStop(IntPtr rendererDiscoverer);


            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_event_manager")]
            internal static extern IntPtr LibVLCRendererDiscovererEventManager(IntPtr rendererDiscoverer);
        }

        public RendererDiscoverer(LibVLC libVLC, string name = Constants.ServiceDiscoveryProtocol)
            : base(() => Native.LibVLCRendererDiscovererNew(libVLC.NativeReference, name), Native.LibVLCRendererDiscovererRelease)
        {
        }

        private RendererDiscovererEventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    var eventManagerPtr = Native.LibVLCRendererDiscovererEventManager(NativeReference);
                    _eventManager = new RendererDiscovererEventManager(eventManagerPtr);
                }
                return _eventManager;
            }
        }

        public bool Start() => Native.LibVLCRendererDiscovererStart(NativeReference) == 0;

        public void Stop() => Native.LibVLCRendererDiscovererStop(NativeReference);

        public event EventHandler<RendererDiscovererItemAddedEventArgs> ItemAdded
        {
            add => EventManager.AttachEvent(EventType.RendererDiscovererItemAdded, value);
            remove => EventManager.DetachEvent(EventType.RendererDiscovererItemAdded, value);
        }

        public event EventHandler<RendererDiscovererItemDeletedEventArgs> ItemDeleted
        {
            add => EventManager.AttachEvent(EventType.RendererDiscovererItemDeleted, value);
            remove => EventManager.DetachEvent(EventType.RendererDiscovererItemDeleted, value);
        }
    }

    public class RendererItem : Internal
    {
        const int VideoRenderer = 0x0002;
        const int AudioRenderer = 0x0001;

        readonly Utf8StringMarshaler _utf8Marshaler = Utf8StringMarshaler.GetInstance();

        struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_name")]
            internal static extern IntPtr LibVLCRendererItemName(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_release")]
            internal static extern void LibVLCRendererItemRelease(IntPtr rendererItem);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_hold")]
            internal static extern IntPtr LibVLCRendererItemHold(IntPtr rendererItem);

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

        public RendererItem(IntPtr reference) : 
            base(() => reference, Native.LibVLCRendererItemRelease)
        {
            Native.LibVLCRendererItemHold(reference);
        }

        public string Name => _utf8Marshaler.MarshalNativeToManaged(Native.LibVLCRendererItemName(NativeReference)) as string;

        public string Type => _utf8Marshaler.MarshalNativeToManaged(Native.LibVLCRendererItemType(NativeReference)) as string;

        public string IconUri => _utf8Marshaler.MarshalNativeToManaged(Native.LibVLCRendererItemIconUri(NativeReference)) as string;

        public bool CanRenderVideo => (Native.LibVLCRendererItemFlags(NativeReference) & VideoRenderer) != 0;

        public bool CanRenderAudio => (Native.LibVLCRendererItemFlags(NativeReference) & AudioRenderer) != 0;
    }
}