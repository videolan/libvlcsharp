using System;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed dispatcher for media list events.
    ///
    /// LibVLC 4 removed the native media list event manager (<c>libvlc_media_list_event_manager</c>) and the
    /// <c>libvlc_event_attach</c>/<c>libvlc_event_detach</c> API. Mutations performed through this wrapper
    /// synthesize the legacy media list events to preserve the public event API.
    /// </summary>
    internal class MediaListEventManager : EventManager
    {
        EventHandler<MediaListItemAddedEventArgs>? _itemAdded;
        EventHandler<MediaListWillAddItemEventArgs>? _willAddItem;
        EventHandler<MediaListItemDeletedEventArgs>? _itemDeleted;
        EventHandler<MediaListWillDeleteItemEventArgs>? _willDeleteItem;
        EventHandler<EventArgs>? _endReached;

        internal void AddItemAdded(EventHandler<MediaListItemAddedEventArgs> handler) => _itemAdded += handler;
        internal void RemoveItemAdded(EventHandler<MediaListItemAddedEventArgs> handler) => _itemAdded -= handler;
        internal void AddWillAddItem(EventHandler<MediaListWillAddItemEventArgs> handler) => _willAddItem += handler;
        internal void RemoveWillAddItem(EventHandler<MediaListWillAddItemEventArgs> handler) => _willAddItem -= handler;
        internal void AddItemDeleted(EventHandler<MediaListItemDeletedEventArgs> handler) => _itemDeleted += handler;
        internal void RemoveItemDeleted(EventHandler<MediaListItemDeletedEventArgs> handler) => _itemDeleted -= handler;
        internal void AddWillDeleteItem(EventHandler<MediaListWillDeleteItemEventArgs> handler) => _willDeleteItem += handler;
        internal void RemoveWillDeleteItem(EventHandler<MediaListWillDeleteItemEventArgs> handler) => _willDeleteItem -= handler;
        internal void AddEndReached(EventHandler<EventArgs> handler) => _endReached += handler;
        internal void RemoveEndReached(EventHandler<EventArgs> handler) => _endReached -= handler;

        internal void OnWillAddItem(Media media, int index)
            => _willAddItem?.Invoke(this, new MediaListWillAddItemEventArgs(media, index));

        internal void OnItemAdded(Media media, int index)
            => _itemAdded?.Invoke(this, new MediaListItemAddedEventArgs(media, index));

        internal void OnWillDeleteItem(Media media, int index)
            => _willDeleteItem?.Invoke(this, new MediaListWillDeleteItemEventArgs(media, index));

        internal void OnItemDeleted(Media media, int index)
            => _itemDeleted?.Invoke(this, new MediaListItemDeletedEventArgs(media, index));
    }
}
