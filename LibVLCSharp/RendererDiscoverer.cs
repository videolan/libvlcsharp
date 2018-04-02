using System;
using System.Runtime.InteropServices;
using System.Security;

namespace VideoLAN.LibVLC
{
    public class RendererDiscoverer : Internal
    {
        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_new")]
            internal static extern IntPtr LibVLCRendererDiscovererNew(IntPtr instance, string name);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_release")]
            internal static extern void LibVLCRendererDiscovererRelease(IntPtr rendererDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_start")]
            internal static extern int LibVLCRendererDiscovererStart(IntPtr rendererDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_stop")]
            internal static extern void LibVLCRendererDiscovererStop(IntPtr rendererDiscoverer);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_discoverer_event_manager")]
            internal static extern IntPtr LibVLCRendererDiscovererEventManager(IntPtr rendererDiscoverer);
        }

        public RendererDiscoverer(Instance instance, string name) 
            : base(() => Native.LibVLCRendererDiscovererNew(instance.NativeReference, name), 
                  Native.LibVLCRendererDiscovererRelease)
        {
        }

        RendererDiscovererEventManager _eventManager;

        public RendererDiscovererEventManager EventManager
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
    }

    public class RendererItem : Internal
    {
        const int VideoRenderer = 0x0002;
        const int AudioRenderer = 0x0001;

        struct Native
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_name")]
            internal static extern string LibVLCRendererItemName(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_release")]
            internal static extern void LibVLCRendererItemRelease(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_hold")]
            internal static extern IntPtr LibVLCRendererItemHold(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_type")]
            internal static extern string LibVLCRendererItemType(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_icon_uri")]
            internal static extern string LibVLCRendererItemIconUri(IntPtr rendererItem);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_renderer_item_flags")]
            internal static extern int LibVLCRendererItemFlags(IntPtr rendererItem);
        }

        public RendererItem(IntPtr reference) : base(() => reference, Native.LibVLCRendererItemRelease)
        {
            Native.LibVLCRendererItemHold(reference); //fail
        }

        public string Name => Native.LibVLCRendererItemName(NativeReference);

        public string Type => Native.LibVLCRendererItemType(NativeReference);

        public string IconUri => Native.LibVLCRendererItemIconUri(NativeReference);

        public bool CanRenderVideo => (Native.LibVLCRendererItemFlags(NativeReference) & VideoRenderer) != 0;

        public bool CanRenderAudio => (Native.LibVLCRendererItemFlags(NativeReference) & AudioRenderer) != 0;
    }
}