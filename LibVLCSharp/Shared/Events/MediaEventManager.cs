using System;

namespace LibVLCSharp.Shared
{
    internal class MediaEventManager : EventManager
    {
        readonly object _lock = new object();
#if IOS
        static EventHandler<MediaMetaChangedEventArgs> _mediaMetaChanged;
        static EventHandler<MediaParsedChangedEventArgs> _mediaParsedChanged;
        static EventHandler<MediaSubItemAddedEventArgs> _mediaSubItemAdded;
        static EventHandler<MediaDurationChangedEventArgs> _mediaDurationChanged;
        static EventHandler<MediaFreedEventArgs> _mediaFreed;
        static EventHandler<MediaStateChangedEventArgs> _mediaStateChanged;
        static EventHandler<MediaSubItemTreeAddedEventArgs> _mediaSubItemTreeAdded;
#else
        EventHandler<MediaMetaChangedEventArgs> _mediaMetaChanged;
        EventHandler<MediaParsedChangedEventArgs> _mediaParsedChanged;
        EventHandler<MediaSubItemAddedEventArgs> _mediaSubItemAdded;
        EventHandler<MediaDurationChangedEventArgs> _mediaDurationChanged;
        EventHandler<MediaFreedEventArgs> _mediaFreed;
        EventHandler<MediaStateChangedEventArgs> _mediaStateChanged;
        EventHandler<MediaSubItemTreeAddedEventArgs> _mediaSubItemTreeAdded;
#endif
        public MediaEventManager(IntPtr ptr) : base(ptr)
        {
        }

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock(_lock)
            {
                switch (eventType)
                {
                    case EventType.MediaMetaChanged:
                        _mediaMetaChanged += eventHandler as EventHandler<MediaMetaChangedEventArgs>;
                        AttachNativeEvent(eventType, OnMetaChanged);
                        break;
                    case EventType.MediaParsedChanged:
                        _mediaParsedChanged += eventHandler as EventHandler<MediaParsedChangedEventArgs>;
                        AttachNativeEvent(eventType, OnParsedChanged);
                        break;
                    case EventType.MediaSubItemAdded:
                        _mediaSubItemAdded += eventHandler as EventHandler<MediaSubItemAddedEventArgs>;
                        AttachNativeEvent(eventType, OnSubItemAdded);
                        break;
                    case EventType.MediaDurationChanged:
                        _mediaDurationChanged += eventHandler as EventHandler<MediaDurationChangedEventArgs>;
                        AttachNativeEvent(eventType, OnDurationChanged);
                        break;
                    case EventType.MediaFreed:
                        _mediaFreed += eventHandler as EventHandler<MediaFreedEventArgs>;
                        AttachNativeEvent(eventType, OnMediaFreed);
                        break;
                    case EventType.MediaStateChanged:
                        _mediaStateChanged += eventHandler as EventHandler<MediaStateChangedEventArgs>;
                        AttachNativeEvent(eventType, OnMediaStateChanged);
                        break;
                    case EventType.MediaSubItemTreeAdded:
                        _mediaSubItemTreeAdded += eventHandler as EventHandler<MediaSubItemTreeAddedEventArgs>;
                        AttachNativeEvent(eventType, OnSubItemTreeAdded);
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
                    case EventType.MediaMetaChanged:
                        _mediaMetaChanged -= eventHandler as EventHandler<MediaMetaChangedEventArgs>;
                        DetachNativeEvent(eventType, OnMetaChanged);
                        break;
                    case EventType.MediaParsedChanged:
                        _mediaParsedChanged -= eventHandler as EventHandler<MediaParsedChangedEventArgs>;
                        DetachNativeEvent(eventType, OnParsedChanged);
                        break;
                    case EventType.MediaSubItemAdded:
                        _mediaSubItemAdded -= eventHandler as EventHandler<MediaSubItemAddedEventArgs>;
                        DetachNativeEvent(eventType, OnSubItemAdded);
                        break;
                    case EventType.MediaDurationChanged:
                        _mediaDurationChanged -= eventHandler as EventHandler<MediaDurationChangedEventArgs>;
                        DetachNativeEvent(eventType, OnDurationChanged);
                        break;
                    case EventType.MediaFreed:
                        _mediaFreed -= eventHandler as EventHandler<MediaFreedEventArgs>;
                        DetachNativeEvent(eventType, OnMediaFreed);
                        break;
                    case EventType.MediaStateChanged:
                        _mediaStateChanged -= eventHandler as EventHandler<MediaStateChangedEventArgs>;
                        DetachNativeEvent(eventType, OnMediaStateChanged);
                        break;
                    case EventType.MediaSubItemTreeAdded:
                        _mediaSubItemTreeAdded -= eventHandler as EventHandler<MediaSubItemTreeAddedEventArgs>;
                        DetachNativeEvent(eventType, OnSubItemTreeAdded);
                        break;
                    default:
                        OnEventUnhandled(this, eventType);
                        break;
                }
            }
        }

#if IOS
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnSubItemTreeAdded(IntPtr ptr)
        {
            _mediaSubItemTreeAdded?.Invoke(null,
                new MediaSubItemTreeAddedEventArgs(RetrieveEvent(ptr).Union.MediaSubItemTreeAdded.MediaInstance));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnMediaStateChanged(IntPtr ptr)
        {
            _mediaStateChanged?.Invoke(null,
                new MediaStateChangedEventArgs(RetrieveEvent(ptr).Union.MediaStateChanged.NewState));    
        }
        
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnMediaFreed(IntPtr ptr)
        {
            _mediaFreed?.Invoke(null, new MediaFreedEventArgs(RetrieveEvent(ptr).Union.MediaFreed.MediaInstance));    
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnDurationChanged(IntPtr ptr)
        {
            _mediaDurationChanged?.Invoke(null, 
                new MediaDurationChangedEventArgs(RetrieveEvent(ptr).Union.MediaDurationChanged.NewDuration));    
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnSubItemAdded(IntPtr ptr)
        {
            _mediaSubItemAdded?.Invoke(null, 
                new MediaSubItemAddedEventArgs(RetrieveEvent(ptr).Union.MediaSubItemAdded.NewChild));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnParsedChanged(IntPtr ptr)
        {
            _mediaParsedChanged?.Invoke(null,
                new MediaParsedChangedEventArgs(RetrieveEvent(ptr).Union.MediaParsedChanged.NewStatus));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnMetaChanged(IntPtr ptr)
        {
            _mediaMetaChanged?.Invoke(null,
                new MediaMetaChangedEventArgs(RetrieveEvent(ptr).Union.MediaMetaChanged.MetaType));
        }
#else
        void OnSubItemTreeAdded(IntPtr ptr)
        {
            _mediaSubItemTreeAdded?.Invoke(this,
                new MediaSubItemTreeAddedEventArgs(RetrieveEvent(ptr).Union.MediaSubItemTreeAdded.MediaInstance));
        }

        void OnMediaStateChanged(IntPtr ptr)
        {
            _mediaStateChanged?.Invoke(this,
                new MediaStateChangedEventArgs(RetrieveEvent(ptr).Union.MediaStateChanged.NewState));    
        }
        
        void OnMediaFreed(IntPtr ptr)
        {
            _mediaFreed?.Invoke(this, new MediaFreedEventArgs(RetrieveEvent(ptr).Union.MediaFreed.MediaInstance));    
        }

        void OnDurationChanged(IntPtr ptr)
        {
            _mediaDurationChanged?.Invoke(this, 
                new MediaDurationChangedEventArgs(RetrieveEvent(ptr).Union.MediaDurationChanged.NewDuration));    
        }

        void OnSubItemAdded(IntPtr ptr)
        {
            _mediaSubItemAdded?.Invoke(this, 
                new MediaSubItemAddedEventArgs(RetrieveEvent(ptr).Union.MediaSubItemAdded.NewChild));
        }

        void OnParsedChanged(IntPtr ptr)
        {
            _mediaParsedChanged?.Invoke(this,
                new MediaParsedChangedEventArgs(RetrieveEvent(ptr).Union.MediaParsedChanged.NewStatus));
        }

        void OnMetaChanged(IntPtr ptr)
        {
            _mediaMetaChanged?.Invoke(this,
                new MediaMetaChangedEventArgs(RetrieveEvent(ptr).Union.MediaMetaChanged.MetaType));
        }
#endif
    }
}