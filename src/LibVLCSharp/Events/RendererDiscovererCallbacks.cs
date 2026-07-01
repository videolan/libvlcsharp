using LibVLCSharp.Helpers;
using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed mirror of the native <c>libvlc_renderer_discoverer_cbs</c> struct passed to
    /// <c>libvlc_renderer_discoverer_new</c> in LibVLC 4.
    ///
    /// The struct content (the function pointers) is identical for every renderer discoverer; only the
    /// <c>cbs_opaque</c> pointer differs per instance. We therefore build a single immutable native copy
    /// once and share its pointer across all renderer discoverers.
    /// </summary>
    internal static class RendererDiscovererCallbacks
    {
        // libvlc_renderer_discoverer_cbs: callbacks available since version 0.
        const uint Version = 0;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ItemAddedCb(IntPtr opaque, IntPtr item);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ItemRemovedCb(IntPtr opaque, IntPtr item);

        // Native struct layout; order MUST match struct libvlc_renderer_discoverer_cbs exactly.
        [StructLayout(LayoutKind.Sequential)]
        struct NativeCallbacks
        {
            public uint Version;
            public IntPtr OnItemAdded;
            public IntPtr OnItemRemoved;
        }

        // Keep the delegate instances rooted for the lifetime of the process so the native function
        // pointers stored in the shared struct remain valid.
        static readonly ItemAddedCb s_itemAdded = OnItemAdded;
        static readonly ItemRemovedCb s_itemRemoved = OnItemRemoved;

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
                OnItemAdded = Marshal.GetFunctionPointerForDelegate(s_itemAdded),
                OnItemRemoved = Marshal.GetFunctionPointerForDelegate(s_itemRemoved),
            };

            var ptr = Marshal.AllocHGlobal(MarshalUtils.SizeOf(cbs));
            Marshal.StructureToPtr(cbs, ptr, false);
            return ptr;
        }

        static RendererDiscovererEventManager? Manager(IntPtr opaque)
            => EventManager.FromOpaque<RendererDiscovererEventManager>(opaque);

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

        static void OnItemAdded(IntPtr opaque, IntPtr item) => Guarded(() => Manager(opaque)?.OnItemAdded(item));
        static void OnItemRemoved(IntPtr opaque, IntPtr item) => Guarded(() => Manager(opaque)?.OnItemRemoved(item));
    }
}
