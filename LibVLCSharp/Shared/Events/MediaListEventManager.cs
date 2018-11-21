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

        int _mediaListItemAddedRegistrationCount;
        int _mediaListWillAddItemdRegistrationCount;
        int _mediaListItemDeletedRegistrationCount;
        int _mediaListWillDeleteItemRegistrationCount;
        int _mediaListEndReachedRegistrationCount;

        EventCallback _mediaListItemAddedCallback;
        EventCallback _mediaListWillAddItemdCallback;
        EventCallback _mediaListItemDeletedCallback;
        EventCallback _mediaListWillDeleteItemCallback;
        EventCallback _mediaListEndReachedCallback;

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
                        Attach(eventType,
                            ref _mediaListItemAddedRegistrationCount,
                            () => _mediaListItemAdded += eventHandler as EventHandler<MediaListItemAddedEventArgs>,
                            () => _mediaListItemAddedCallback = OnItemAdded);
                        break;
                    case EventType.MediaListWillAddItem:
                        Attach(eventType,
                            ref _mediaListWillAddItemdRegistrationCount,
                            () => _mediaListWillAddItem += eventHandler as EventHandler<MediaListWillAddItemEventArgs>,
                            () => _mediaListWillAddItemdCallback = OnWillAddItem);
                        break;
                    case EventType.MediaListItemDeleted:
                        Attach(eventType,
                            ref _mediaListItemDeletedRegistrationCount,
                            () => _mediaListItemDeleted += eventHandler as EventHandler<MediaListItemDeletedEventArgs>,
                            () => _mediaListItemDeletedCallback = OnItemDeleted);
                        break;
                    case EventType.MediaListViewWillDeleteItem:
                        Attach(eventType,
                            ref _mediaListWillDeleteItemRegistrationCount,
                            () => _mediaListWillDeleteItem += eventHandler as EventHandler<MediaListWillDeleteItemEventArgs>,
                            () => _mediaListWillDeleteItemCallback = OnWillDeleteItem);
                        break;
                    case EventType.MediaListEndReached:
                        Attach(eventType,
                            ref _mediaListEndReachedRegistrationCount,
                            () => _mediaListEndReached += eventHandler as EventHandler<EventArgs>,
                            () => _mediaListEndReachedCallback = OnEndReached);
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
                        Detach(eventType,
                            ref _mediaListItemAddedRegistrationCount,
                            () => _mediaListItemAdded -= eventHandler as EventHandler<MediaListItemAddedEventArgs>,
                            ref _mediaListItemAddedCallback);
                        break;
                    case EventType.MediaListWillAddItem:
                        Detach(eventType,
                            ref _mediaListWillAddItemdRegistrationCount,
                            () => _mediaListWillAddItem -= eventHandler as EventHandler<MediaListWillAddItemEventArgs>,
                            ref _mediaListWillAddItemdCallback);
                        break;
                    case EventType.MediaListItemDeleted:
                        Detach(eventType,
                            ref _mediaListItemDeletedRegistrationCount,
                            () => _mediaListItemDeleted -= eventHandler as EventHandler<MediaListItemDeletedEventArgs>,
                            ref _mediaListItemDeletedCallback);
                        break;
                    case EventType.MediaListViewWillDeleteItem:
                        Detach(eventType,
                            ref _mediaListWillDeleteItemRegistrationCount,
                            () => _mediaListWillDeleteItem -= eventHandler as EventHandler<MediaListWillDeleteItemEventArgs>,
                            ref _mediaListWillDeleteItemCallback);
                        break;
                    case EventType.MediaListEndReached:
                        Detach(eventType,
                            ref _mediaListEndReachedRegistrationCount,
                            () => _mediaListEndReached -= eventHandler as EventHandler<EventArgs>,
                            ref _mediaListEndReachedCallback);
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
