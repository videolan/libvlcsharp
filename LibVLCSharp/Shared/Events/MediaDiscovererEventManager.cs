using System;

namespace LibVLCSharp.Shared
{
    internal class MediaDiscovererEventManager : EventManager
    {
        readonly object _lock = new object();
#if IOS
        static EventHandler<EventArgs> _mediaDiscovererStarted;
        static EventHandler<EventArgs> _mediaDiscovererStopped;
#else
        EventHandler<EventArgs> _mediaDiscovererStarted;
        EventHandler<EventArgs> _mediaDiscovererStopped;
#endif
        public MediaDiscovererEventManager(IntPtr ptr) : base(ptr)
        {
        }

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock (_lock)
            {
                switch (eventType)
                {
                    case EventType.MediaDiscovererStarted:
                        _mediaDiscovererStarted += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnStarted);
                        break;
                    case EventType.MediaDiscovererStopped:
                        _mediaDiscovererStopped += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnStopped);
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
                    case EventType.MediaDiscovererStarted:
                        _mediaDiscovererStarted -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnStarted);
                        break;
                    case EventType.MediaDiscovererStopped:
                        _mediaDiscovererStopped -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnStopped);
                        break;
                    default:
                        OnEventUnhandled(this, eventType);
                        break;
                }
            }
        }

#if IOS
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnStarted(IntPtr ptr)
        {
            _mediaDiscovererStarted?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnStopped(IntPtr ptr)
        {
            _mediaDiscovererStopped?.Invoke(null, EventArgs.Empty);
        }
#else
        void OnStarted(IntPtr ptr)
        {
            _mediaDiscovererStarted?.Invoke(this, EventArgs.Empty);
        }

        void OnStopped(IntPtr ptr)
        {
            _mediaDiscovererStopped?.Invoke(this, EventArgs.Empty);
        }
#endif
    }
}
