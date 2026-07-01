using LibVLCSharp.Helpers;
using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed mirror of the native <c>libvlc_media_discoverer_cbs</c> struct passed to
    /// <c>libvlc_media_discoverer_new</c> in LibVLC 4.
    ///
    /// The struct content (the function pointers) is identical for every discoverer; only the
    /// <c>cbs_opaque</c> pointer differs per instance. A single immutable native copy is built once and
    /// shared. Both callbacks are optional.
    /// </summary>
    internal static class MediaDiscovererCallbacks
    {
        // libvlc_media_discoverer_cbs: callbacks "available since version 0".
        const uint Version = 0;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void MediaAddedCb(IntPtr opaque, IntPtr parent, IntPtr media);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void MediaRemovedCb(IntPtr opaque, IntPtr media);

        // Native struct layout; order MUST match struct libvlc_media_discoverer_cbs exactly.
        [StructLayout(LayoutKind.Sequential)]
        struct NativeCallbacks
        {
            public uint Version;
            public IntPtr OnMediaAdded;
            public IntPtr OnMediaRemoved;
        }

        // Keep the delegate instances rooted for the lifetime of the process.
        static readonly MediaAddedCb s_mediaAdded = OnMediaAdded;
        static readonly MediaRemovedCb s_mediaRemoved = OnMediaRemoved;

        static readonly IntPtr s_pointer = Build();

        /// <summary>
        /// Pointer to the shared, process-lifetime native callbacks struct.
        /// </summary>
        internal static IntPtr Pointer => s_pointer;

        static IntPtr Build()
        {
            var cbs = new NativeCallbacks
            {
                Version = Version,
                OnMediaAdded = Marshal.GetFunctionPointerForDelegate(s_mediaAdded),
                OnMediaRemoved = Marshal.GetFunctionPointerForDelegate(s_mediaRemoved),
            };

            var ptr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(cbs));
            Marshal.StructureToPtr(cbs, ptr, false);
            return ptr;
        }

        static MediaDiscovererEventManager? Manager(IntPtr opaque) => EventManager.FromOpaque<MediaDiscovererEventManager>(opaque);

        static void Guarded(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // A managed exception must never cross back into native code.
                Core.Log(ex.ToString());
            }
        }

        static void OnMediaAdded(IntPtr opaque, IntPtr parent, IntPtr media) => Guarded(() => Manager(opaque)?.OnMediaAdded(parent, media));
        static void OnMediaRemoved(IntPtr opaque, IntPtr media) => Guarded(() => Manager(opaque)?.OnMediaRemoved(media));
    }
}
