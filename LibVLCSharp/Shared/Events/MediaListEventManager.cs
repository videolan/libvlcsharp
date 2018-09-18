using System;

namespace LibVLCSharp.Shared
{
    internal class MediaListEventManager : EventManager
    {
        readonly object _lock = new object();
#if IOS
        static EventHandler<MediaListItemAddedEventArgs> _mediaListItemAdded;
        static EventHandler<MediaListWillAddItemEventArgs> _mediaListWillAddItem;
        static EventHandler<MediaListItemDeletedEventArgs> _mediaListItemDeleted;
        static EventHandler<MediaListWillDeleteItemEventArgs> _mediaListWillDeleteItem;
        static EventHandler<EventArgs> _mediaListEndReached;
#else
        EventHandler<MediaListItemAddedEventArgs> _mediaListItemAdded;
        EventHandler<MediaListWillAddItemEventArgs> _mediaListWillAddItem;
        EventHandler<MediaListItemDeletedEventArgs> _mediaListItemDeleted;
        EventHandler<MediaListWillDeleteItemEventArgs> _mediaListWillDeleteItem;
        EventHandler<EventArgs> _mediaListEndReached;

#endif
        public MediaListEventManager(IntPtr ptr) : base(ptr)
        {
        }

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock (_lock)
            {
                switch (eventType)
                {
                    case EventType.MediaListItemAdded:
                        _mediaListItemAdded += eventHandler as EventHandler<MediaListItemAddedEventArgs>;
                        AttachNativeEvent(eventType, OnItemAdded);
                        break;
                    case EventType.MediaListWillAddItem:
                        _mediaListWillAddItem += eventHandler as EventHandler<MediaListWillAddItemEventArgs>;
                        AttachNativeEvent(eventType, OnWillAddItem);
                        break;
                    case EventType.MediaListItemDeleted:
                        _mediaListItemDeleted += eventHandler as EventHandler<MediaListItemDeletedEventArgs>;
                        AttachNativeEvent(eventType, OnItemDeleted);
                        break;
                    case EventType.MediaListViewWillDeleteItem:
                        _mediaListWillDeleteItem += eventHandler as EventHandler<MediaListWillDeleteItemEventArgs>;
                        AttachNativeEvent(eventType, OnWillDeleteItem);
                        break;
                    case EventType.MediaListEndReached:
                        _mediaListEndReached += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnEndReached);
                        break;
                    default:
                        OnEventUnhandled(this, eventType);
                        break;
                }
            }
        }

        protected internal override void DetachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock (_lock)
            {
                switch (eventType)
                {
                    case EventType.MediaListItemAdded:
                        _mediaListItemAdded -= eventHandler as EventHandler<MediaListItemAddedEventArgs>;
                        DetachNativeEvent(eventType, OnItemAdded);
                        break;
                    case EventType.MediaListWillAddItem:
                        _mediaListWillAddItem -= eventHandler as EventHandler<MediaListWillAddItemEventArgs>;
                        DetachNativeEvent(eventType, OnWillAddItem);
                        break;
                    case EventType.MediaListItemDeleted:
                        _mediaListItemDeleted -= eventHandler as EventHandler<MediaListItemDeletedEventArgs>;
                        DetachNativeEvent(eventType, OnItemDeleted);
                        break;
                    case EventType.MediaListViewWillDeleteItem:
                        _mediaListWillDeleteItem -= eventHandler as EventHandler<MediaListWillDeleteItemEventArgs>;
                        DetachNativeEvent(eventType, OnWillDeleteItem);
                        break;
                    case EventType.MediaListEndReached:
                        _mediaListEndReached -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnEndReached);
                        break;
                    default:
                        OnEventUnhandled(this, eventType);
                        break;
                }
            }
        }

#if IOS
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemAdded(IntPtr ptr)
        {
            var itemAdded = RetrieveEvent(ptr).Union.MediaListItemAdded;

            _mediaListItemAdded?.Invoke(null,
                new MediaListItemAddedEventArgs(new Media(itemAdded.MediaInstance), itemAdded.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnWillAddItem(IntPtr ptr)
        {
            var willAddItem = RetrieveEvent(ptr).Union.MediaListWillAddItem;

            _mediaListWillAddItem?.Invoke(null,
                new MediaListWillAddItemEventArgs(new Media(willAddItem.MediaInstance), willAddItem.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemDeleted(IntPtr ptr)
        {
            var itemDeleted = RetrieveEvent(ptr).Union.MediaListItemDeleted;

            _mediaListItemDeleted?.Invoke(null,
                new MediaListItemDeletedEventArgs(new Media(itemDeleted.MediaInstance), itemDeleted.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnWillDeleteItem(IntPtr ptr)
        {
            var willDeleteItem = RetrieveEvent(ptr).Union.MediaListWillDeleteItem;

            _mediaListWillDeleteItem?.Invoke(null,
                new MediaListWillDeleteItemEventArgs(new Media(willDeleteItem.MediaInstance), willDeleteItem.Index));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnEndReached(IntPtr ptr)
        {
            _mediaListEndReached?.Invoke(null, EventArgs.Empty);
        }
#else
        void OnItemAdded(IntPtr ptr)
        {
            var itemAdded = RetrieveEvent(ptr).Union.MediaListItemAdded;

            _mediaListItemAdded?.Invoke(this,
                new MediaListItemAddedEventArgs(new Media(itemAdded.MediaInstance), itemAdded.Index));
        }

        void OnWillAddItem(IntPtr ptr)
        {
            var willAddItem = RetrieveEvent(ptr).Union.MediaListWillAddItem;

            _mediaListWillAddItem?.Invoke(this,
                new MediaListWillAddItemEventArgs(new Media(willAddItem.MediaInstance), willAddItem.Index));
        }

        void OnItemDeleted(IntPtr ptr)
        {
            var itemDeleted = RetrieveEvent(ptr).Union.MediaListItemDeleted;

            _mediaListItemDeleted?.Invoke(this,
                new MediaListItemDeletedEventArgs(new Media(itemDeleted.MediaInstance), itemDeleted.Index));
        }

        void OnWillDeleteItem(IntPtr ptr)
        {
            var willDeleteItem = RetrieveEvent(ptr).Union.MediaListWillDeleteItem;

            _mediaListWillDeleteItem?.Invoke(this,
                new MediaListWillDeleteItemEventArgs(new Media(willDeleteItem.MediaInstance), willDeleteItem.Index));
        }

        void OnEndReached(IntPtr ptr)
        {
            _mediaListEndReached?.Invoke(this, EventArgs.Empty);
        }
#endif
    }
}
