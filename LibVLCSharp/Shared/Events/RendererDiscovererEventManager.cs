using System;

namespace LibVLCSharp.Shared
{
    internal class RendererDiscovererEventManager : EventManager
    {
        readonly object _lock = new object();
#if IOS
        static EventHandler<RendererDiscovererItemAddedEventArgs> _itemAdded;
        static EventHandler<RendererDiscovererItemDeletedEventArgs> _itemDeleted;
#else
        EventHandler<RendererDiscovererItemAddedEventArgs> _itemAdded;
        EventHandler<RendererDiscovererItemDeletedEventArgs> _itemDeleted;
#endif

        int _itemAddedRegistrationCount;
        int _itemDeletedRegistrationCount;

        EventCallback _itemAddedCallback;
        EventCallback _itemDeletedCallback;

        internal RendererDiscovererEventManager(IntPtr ptr) : base(ptr)
        {
        }

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock (_lock)
            {
                switch(eventType)
                {
                    case EventType.RendererDiscovererItemAdded:
                        Attach(eventType,
                            ref _itemAddedRegistrationCount,
                            () => _itemAdded += eventHandler as EventHandler<RendererDiscovererItemAddedEventArgs>,
                            () => _itemAddedCallback = OnItemAdded);
                        break;
                    case EventType.RendererDiscovererItemDeleted:
                        Attach(eventType,
                            ref _itemDeletedRegistrationCount,
                            () => _itemDeleted += eventHandler as EventHandler<RendererDiscovererItemDeletedEventArgs>,
                            () => _itemDeletedCallback = OnItemDeleted);
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
                    case EventType.RendererDiscovererItemAdded:
                        Detach(eventType,
                            ref _itemAddedRegistrationCount,
                            () => _itemAdded -= eventHandler as EventHandler<RendererDiscovererItemAddedEventArgs>,
                            ref _itemAddedCallback);
                        break;
                    case EventType.RendererDiscovererItemDeleted:
                        Detach(eventType,
                            ref _itemDeletedRegistrationCount,
                            () => _itemDeleted -= eventHandler as EventHandler<RendererDiscovererItemDeletedEventArgs>,
                            ref _itemDeletedCallback);
                        break;
                    default:
                        OnEventUnhandled(this, eventType);
                        break;
                }
            }
        }

#if IOS
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemDeleted(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).Union.RendererDiscovererItemDeleted;
            _itemDeleted?.Invoke(null, new RendererDiscovererItemDeletedEventArgs(new RendererItem(rendererItem.item)));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemAdded(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).Union.RendererDiscovererItemAdded;           
            _itemAdded?.Invoke(null, new RendererDiscovererItemAddedEventArgs(new RendererItem(rendererItem.item)));
        }
#else
        void OnItemDeleted(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).Union.RendererDiscovererItemDeleted;
            _itemDeleted?.Invoke(this, new RendererDiscovererItemDeletedEventArgs(new RendererItem(rendererItem.item)));
        }

        void OnItemAdded(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).Union.RendererDiscovererItemAdded;           
            _itemAdded?.Invoke(this, new RendererDiscovererItemAddedEventArgs(new RendererItem(rendererItem.item)));
        }
#endif
    }
}
