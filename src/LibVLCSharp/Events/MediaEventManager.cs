using System;

namespace LibVLCSharp
{
    internal class MediaEventManager : EventManager
    {
        EventHandler<MediaMetaChangedEventArgs>? _mediaMetaChanged;
        EventHandler<MediaParsedChangedEventArgs>? _mediaParsedChanged;
        EventHandler<MediaSubItemAddedEventArgs>? _mediaSubItemAdded;
        EventHandler<MediaDurationChangedEventArgs>? _mediaDurationChanged;
        EventHandler<MediaStateChangedEventArgs>? _mediaStateChanged;
        EventHandler<MediaSubItemTreeAddedEventArgs>? _mediaSubItemTreeAdded;
        EventHandler<MediaThumbnailGeneratedEventArgs>? _mediaThumbnailGenerated;
        EventHandler<MediaAttachedThumbnailsFoundEventArgs>? _mediaAttachedThumbnailsFound;

        public MediaEventManager(IntPtr ptr) : base(ptr)
        {
        }

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            switch (eventType)
            {
                case EventType.MediaMetaChanged:
                    _mediaMetaChanged += eventHandler as EventHandler<MediaMetaChangedEventArgs>;
                    Attach(eventType, OnMetaChanged);
                    break;
                case EventType.MediaParsedChanged:
                    _mediaParsedChanged += eventHandler as EventHandler<MediaParsedChangedEventArgs>;
                    Attach(eventType, OnParsedChanged);
                    break;
                case EventType.MediaSubItemAdded:
                    _mediaSubItemAdded += eventHandler as EventHandler<MediaSubItemAddedEventArgs>;
                    Attach(eventType, OnSubItemAdded);
                    break;
                case EventType.MediaDurationChanged:
                    _mediaDurationChanged += eventHandler as EventHandler<MediaDurationChangedEventArgs>;
                    Attach(eventType, OnDurationChanged);
                    break;
                case EventType.MediaStateChanged:
                    _mediaStateChanged += eventHandler as EventHandler<MediaStateChangedEventArgs>;
                    Attach(eventType, OnMediaStateChanged);
                    break;
                case EventType.MediaSubItemTreeAdded:
                    _mediaSubItemTreeAdded += eventHandler as EventHandler<MediaSubItemTreeAddedEventArgs>;
                    Attach(eventType, OnSubItemTreeAdded);
                    break;
                case EventType.MediaThumbnailGenerated:
                    _mediaThumbnailGenerated += eventHandler as EventHandler<MediaThumbnailGeneratedEventArgs>;
                    Attach(eventType, OnThumbnailGenerated);
                    break;
                case EventType.MediaAttachedThumbnailsFound:
                    _mediaAttachedThumbnailsFound += eventHandler as EventHandler<MediaAttachedThumbnailsFoundEventArgs>;
                    Attach(eventType, OnAttachedThumbnailsFound);
                    break;
                default:
                    OnEventUnhandled(this, eventType);
                    break;
            }
        }

        protected internal override void DetachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            switch (eventType)
            {
                case EventType.MediaMetaChanged:
                    _mediaMetaChanged -= eventHandler as EventHandler<MediaMetaChangedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaParsedChanged:
                    _mediaParsedChanged -= eventHandler as EventHandler<MediaParsedChangedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaSubItemAdded:
                    _mediaSubItemAdded -= eventHandler as EventHandler<MediaSubItemAddedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaDurationChanged:
                    _mediaDurationChanged -= eventHandler as EventHandler<MediaDurationChangedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaStateChanged:
                    _mediaStateChanged -= eventHandler as EventHandler<MediaStateChangedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaSubItemTreeAdded:
                    _mediaSubItemTreeAdded -= eventHandler as EventHandler<MediaSubItemTreeAddedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaThumbnailGenerated:
                    _mediaThumbnailGenerated -= eventHandler as EventHandler<MediaThumbnailGeneratedEventArgs>;
                    Detach(eventType);
                    break;
                case EventType.MediaAttachedThumbnailsFound:
                    _mediaAttachedThumbnailsFound -= eventHandler as EventHandler<MediaAttachedThumbnailsFoundEventArgs>;
                    Detach(eventType);
                    break;
                default:
                    OnEventUnhandled(this, eventType);
                    break;
            }
        }

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

        void OnThumbnailGenerated(IntPtr ptr)
        {
            _mediaThumbnailGenerated?.Invoke(this,
                new MediaThumbnailGeneratedEventArgs(RetrieveEvent(ptr).Union.MediaThumbnailGenerated.Thumbnail));
        }

        void OnAttachedThumbnailsFound(IntPtr ptr)
        {
            _mediaAttachedThumbnailsFound?.Invoke(this,
                new MediaAttachedThumbnailsFoundEventArgs(RetrieveEvent(ptr).Union.MediaAttachedThumbnailsFound.Thumbmails));
        }
    }
}
