using System;
using System.Runtime.InteropServices;
using System.Security;
#if IOS
using ObjCRuntime;
#endif

namespace LibVLCSharp.Shared
{
    public class RendererDiscoverer : Internal
    {
        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_new")]
            internal static extern IntPtr LibVLCRendererDiscovererNew(IntPtr libvlc, string name);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_release")]
            internal static extern void LibVLCRendererDiscovererRelease(IntPtr rendererDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_start")]
            internal static extern int LibVLCRendererDiscovererStart(IntPtr rendererDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_stop")]
            internal static extern void LibVLCRendererDiscovererStop(IntPtr rendererDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_event_manager")]
            internal static extern IntPtr LibVLCRendererDiscovererEventManager(IntPtr rendererDiscoverer);
        }
#if IOS
        static RendererDiscoverer _rd;
#endif
        public RendererDiscoverer(LibVLC libVLC, string name)
            : base(() => Native.LibVLCRendererDiscovererNew(libVLC.NativeReference, name),
                   Native.LibVLCRendererDiscovererRelease, Native.LibVLCRendererDiscovererEventManager)
        {
#if IOS
            _rd = this;
#endif
        }

        public bool Start() => Native.LibVLCRendererDiscovererStart(NativeReference) == 0;

        public void Stop() => Native.LibVLCRendererDiscovererStop(NativeReference);

        #region Events

        readonly object _lock = new object();

#if IOS
        EventHandler<RendererDiscovererItemAddedEventArgs> _itemAdded;
        EventHandler<RendererDiscovererItemDeletedEventArgs> _itemDeleted;
#else
        EventHandler<RendererDiscovererItemAddedEventArgs> _itemAdded;
        EventHandler<RendererDiscovererItemDeletedEventArgs> _itemDeleted;
#endif
        // v3
        public event EventHandler<RendererDiscovererItemAddedEventArgs> ItemAdded
        {
            add
            {
                lock (_lock)
                {
                    _itemAdded += value;
                    AttachEvent(EventType.RendererDiscovererItemAdded, OnItemAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _itemAdded -= value;
                    DetachEvent(EventType.RendererDiscovererItemAdded, OnItemAdded);
                }
            }
        }

        // v3
        public event EventHandler<RendererDiscovererItemDeletedEventArgs> ItemDeleted
        {
            add
            {
                lock (_lock)
                {
                    _itemDeleted += value;
                    AttachEvent(EventType.RendererDiscovererItemDeleted, OnItemDeleted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _itemDeleted -= value;
                    DetachEvent(EventType.RendererDiscovererItemDeleted, OnItemDeleted);
                }
            }
        }

        void OnItemDeleted(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).RendererItem;
            _itemDeleted?.Invoke(this, new RendererDiscovererItemDeletedEventArgs(new RendererItem(rendererItem)));
        }

        void OnItemAdded(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).RendererItem;
            _itemAdded?.Invoke(this, new RendererDiscovererItemAddedEventArgs(new RendererItem(rendererItem)));
        }

#endregion
    }

    public class RendererItem : Internal
    {
        const int VideoRenderer = 0x0002;
        const int AudioRenderer = 0x0001;
        readonly ICustomMarshaler _utf8Marshaler = Utf8StringMarshaler.GetInstance();

        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_name")]
            internal static extern IntPtr LibVLCRendererItemName(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_release")]
            internal static extern void LibVLCRendererItemRelease(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_hold")]
            internal static extern IntPtr LibVLCRendererItemHold(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_type")]
            internal static extern IntPtr LibVLCRendererItemType(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_icon_uri")]
            internal static extern IntPtr LibVLCRendererItemIconUri(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
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