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
                        _itemAdded += eventHandler as EventHandler<RendererDiscovererItemAddedEventArgs>;
                        AttachNativeEvent(eventType, OnItemAdded);
                        break;
                    case EventType.RendererDiscovererItemDeleted:
                        _itemDeleted += eventHandler as EventHandler<RendererDiscovererItemDeletedEventArgs>;
                        AttachNativeEvent(eventType, OnItemDeleted);
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
                        _itemAdded -= eventHandler as EventHandler<RendererDiscovererItemAddedEventArgs>;
                        DetachNativeEvent(eventType, OnItemAdded);
                        break;
                    case EventType.RendererDiscovererItemDeleted:
                        _itemDeleted -= eventHandler as EventHandler<RendererDiscovererItemDeletedEventArgs>;
                        DetachNativeEvent(eventType, OnItemDeleted);
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
            var rendererItem = RetrieveEvent(args).RendererItem;
            _itemDeleted?.Invoke(null, new RendererDiscovererItemDeletedEventArgs(new RendererItem(rendererItem)));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnItemAdded(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).RendererItem;           
            _itemAdded?.Invoke(null, new RendererDiscovererItemAddedEventArgs(new RendererItem(rendererItem)));
        }
#else
        void OnItemDeleted(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).RendererItem;
            _itemDeleted?.Invoke(this, new RendererDiscovererItemDeletedEventArgs(new RendererItem(rendererItem)));
        }

        void OnItemAdded(IntPtr args)
        {
            var rendererItem = RetrieveEvent(args).RendererItem;           
            _itemAdded?.Invoke(this, new RendererDiscovererItemAddedEventArgs(new RendererItem(rendererItem)));
        }
#endif
    }
}
