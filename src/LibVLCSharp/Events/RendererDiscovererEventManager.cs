using System;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed dispatcher for renderer discoverer events.
    ///
    /// In LibVLC 4 the native event manager was removed; items are delivered through the
    /// <c>libvlc_renderer_discoverer_cbs</c> struct (see <see cref="RendererDiscovererCallbacks"/>)
    /// registered at construction. This class only stores the managed subscribers and maps the typed
    /// native callbacks onto the existing <c>RendererDiscoverer</c> events.
    /// </summary>
    internal class RendererDiscovererEventManager : EventManager
    {
        internal event EventHandler<RendererDiscovererItemAddedEventArgs>? ItemAdded;
        internal event EventHandler<RendererDiscovererItemDeletedEventArgs>? ItemDeleted;

        internal void OnItemAdded(IntPtr item)
            => ItemAdded?.Invoke(this, new RendererDiscovererItemAddedEventArgs(new RendererItem(item)));

        internal void OnItemRemoved(IntPtr item)
            => ItemDeleted?.Invoke(this, new RendererDiscovererItemDeletedEventArgs(new RendererItem(item)));
    }
}
