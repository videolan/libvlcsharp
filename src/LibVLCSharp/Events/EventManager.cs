using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// Base class for the managed event dispatchers.
    ///
    /// Starting with LibVLC 4, the native <c>libvlc_event_attach</c>/<c>libvlc_event_detach</c> API and the
    /// per-object event managers were removed. Events are now delivered through a versioned callbacks struct
    /// passed to the owning object's constructor (e.g. <c>libvlc_media_player_new</c>). This class no longer
    /// talks to native code: it only stores the managed subscribers. The native callbacks struct dispatches
    /// into the typed <c>On...</c> methods of the concrete subclasses, which in turn raise the managed events.
    /// </summary>
    internal abstract class EventManager
    {
        GCHandle _self;

        /// <summary>
        /// Allocates a GCHandle on this dispatcher and returns the opaque pointer to be passed as the
        /// <c>cbs_opaque</c> argument of the native constructor. Must be paired with <see cref="Unregister"/>.
        /// </summary>
        internal IntPtr Register()
        {
            if (!_self.IsAllocated)
                _self = GCHandle.Alloc(this);
            return GCHandle.ToIntPtr(_self);
        }

        /// <summary>
        /// Frees the GCHandle allocated by <see cref="Register"/>. Called when the owning object is disposed.
        /// </summary>
        internal void Unregister()
        {
            if (_self.IsAllocated)
                _self.Free();
        }

        /// <summary>
        /// Resolves the dispatcher instance from the opaque pointer passed to a native callback.
        /// </summary>
        internal static T? FromOpaque<T>(IntPtr opaque) where T : EventManager
        {
            if (opaque == IntPtr.Zero)
                return null;
            return GCHandle.FromIntPtr(opaque).Target as T;
        }

    }
}
