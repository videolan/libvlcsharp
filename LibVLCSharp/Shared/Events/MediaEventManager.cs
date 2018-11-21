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

        int _mediaMetaChangedRegistrationCount;
        int _mediaParsedChangedRegistrationCount;
        int _mediaSubItemChangedRegistrationCount;
        int _mediaDurationChangedRegistrationCount;
        int _mediaFreedRegistrationCount;
        int _mediaStateChangedRegistrationCount;
        int _mediaSubitemTreeAddedRegistrationCount;

        EventCallback _mediaMetaChangedCallback;
        EventCallback _mediaParsedChangedCallback;
        EventCallback _mediaSubItemChangedCallback;
        EventCallback _mediaDurationChangedCallback;
        EventCallback _mediaFreedCallback;
        EventCallback _mediaStateChangedCallback;
        EventCallback _mediaSubitemTreeAddedCallback;

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
                        Attach(eventType,
                            ref _mediaMetaChangedRegistrationCount,
                            () => _mediaMetaChanged += eventHandler as EventHandler<MediaMetaChangedEventArgs>,
                            () => _mediaMetaChangedCallback = OnMetaChanged);
                        break;
                    case EventType.MediaParsedChanged:
                        Attach(eventType,
                            ref _mediaParsedChangedRegistrationCount,
                            () => _mediaParsedChanged += eventHandler as EventHandler<MediaParsedChangedEventArgs>,
                            () => _mediaParsedChangedCallback = OnParsedChanged);
                        break;
                    case EventType.MediaSubItemAdded:
                        Attach(eventType,
                            ref _mediaSubItemChangedRegistrationCount,
                            () => _mediaSubItemAdded += eventHandler as EventHandler<MediaSubItemAddedEventArgs>,
                            () => _mediaSubItemChangedCallback = OnSubItemAdded);
                        break;
                    case EventType.MediaDurationChanged:
                        Attach(eventType,
                           ref _mediaDurationChangedRegistrationCount,
                           () => _mediaDurationChanged += eventHandler as EventHandler<MediaDurationChangedEventArgs>,
                           () => _mediaDurationChangedCallback = OnDurationChanged);
                        break;
                    case EventType.MediaFreed:
                        Attach(eventType,
                           ref _mediaFreedRegistrationCount,
                           () => _mediaFreed += eventHandler as EventHandler<MediaFreedEventArgs>,
                           () => _mediaFreedCallback = OnMediaFreed);
                        break;
                    case EventType.MediaStateChanged:
                        Attach(eventType,
                           ref _mediaStateChangedRegistrationCount,
                           () => _mediaStateChanged += eventHandler as EventHandler<MediaStateChangedEventArgs>,
                           () => _mediaStateChangedCallback = OnMediaStateChanged);
                        break;
                    case EventType.MediaSubItemTreeAdded:
                        Attach(eventType,
                           ref _mediaSubitemTreeAddedRegistrationCount,
                           () => _mediaSubItemTreeAdded += eventHandler as EventHandler<MediaSubItemTreeAddedEventArgs>,
                           () => _mediaSubitemTreeAddedCallback = OnSubItemTreeAdded);
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
                        Detach(eventType,
                            ref _mediaMetaChangedRegistrationCount,
                            () => _mediaMetaChanged -= eventHandler as EventHandler<MediaMetaChangedEventArgs>,
                            ref _mediaMetaChangedCallback);
                        break;
                    case EventType.MediaParsedChanged:
                        Detach(eventType,
                            ref _mediaParsedChangedRegistrationCount,
                            () => _mediaParsedChanged -= eventHandler as EventHandler<MediaParsedChangedEventArgs>,
                            ref _mediaParsedChangedCallback);
                        break;
                    case EventType.MediaSubItemAdded:
                        Detach(eventType,
                            ref _mediaSubItemChangedRegistrationCount,
                            () => _mediaSubItemAdded -= eventHandler as EventHandler<MediaSubItemAddedEventArgs>,
                            ref _mediaSubItemChangedCallback);
                        break;
                    case EventType.MediaDurationChanged:
                        Detach(eventType,
                            ref _mediaDurationChangedRegistrationCount,
                            () => _mediaDurationChanged -= eventHandler as EventHandler<MediaDurationChangedEventArgs>,
                            ref _mediaDurationChangedCallback);
                        break;
                    case EventType.MediaFreed:
                        Detach(eventType,
                            ref _mediaFreedRegistrationCount,
                            () => _mediaFreed -= eventHandler as EventHandler<MediaFreedEventArgs>,
                            ref _mediaFreedCallback);
                        break;
                    case EventType.MediaStateChanged:
                        Detach(eventType,
                            ref _mediaStateChangedRegistrationCount,
                            () => _mediaStateChanged -= eventHandler as EventHandler<MediaStateChangedEventArgs>,
                            ref _mediaStateChangedCallback);
                        break;
                    case EventType.MediaSubItemTreeAdded:
                        Detach(eventType,
                            ref _mediaSubitemTreeAddedRegistrationCount,
                            () => _mediaSubItemTreeAdded -= eventHandler as EventHandler<MediaSubItemTreeAddedEventArgs>,
                            ref _mediaSubitemTreeAddedCallback);
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