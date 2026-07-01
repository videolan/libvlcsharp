using System;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed dispatcher for media discoverer events.
    ///
    /// In LibVLC 4 the media discoverer no longer exposes a media list nor a native event manager; discovered
    /// items are delivered through the <c>libvlc_media_discoverer_cbs</c> struct (see
    /// <see cref="MediaDiscovererCallbacks"/>) registered at construction. This class stores the managed
    /// subscribers and raises the <c>MediaDiscoverer</c> events.
    ///
    /// Subscription goes through typed managed events; there is no LibVLC event id involved.
    /// </summary>
    internal class MediaDiscovererEventManager : EventManager
    {
        readonly object _lock = new object();
        EventHandler<MediaDiscovererMediaAddedEventArgs>? _mediaAdded;
        EventHandler<MediaDiscovererMediaRemovedEventArgs>? _mediaRemoved;

        internal void AddMediaAdded(EventHandler<MediaDiscovererMediaAddedEventArgs> handler)
        {
            lock (_lock) _mediaAdded += handler;
        }

        internal void RemoveMediaAdded(EventHandler<MediaDiscovererMediaAddedEventArgs> handler)
        {
            lock (_lock) _mediaAdded -= handler;
        }

        internal void AddMediaRemoved(EventHandler<MediaDiscovererMediaRemovedEventArgs> handler)
        {
            lock (_lock) _mediaRemoved += handler;
        }

        internal void RemoveMediaRemoved(EventHandler<MediaDiscovererMediaRemovedEventArgs> handler)
        {
            lock (_lock) _mediaRemoved -= handler;
        }

        internal void OnMediaAdded(IntPtr parent, IntPtr media)
        {
            EventHandler<MediaDiscovererMediaAddedEventArgs>? handler;
            lock (_lock) handler = _mediaAdded;
            handler?.Invoke(this, new MediaDiscovererMediaAddedEventArgs(parent, media));
        }

        internal void OnMediaRemoved(IntPtr media)
        {
            EventHandler<MediaDiscovererMediaRemovedEventArgs>? handler;
            lock (_lock) handler = _mediaRemoved;
            handler?.Invoke(this, new MediaDiscovererMediaRemovedEventArgs(media));
        }

    }
}
