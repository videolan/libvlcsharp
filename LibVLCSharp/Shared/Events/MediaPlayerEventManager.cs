using System;

namespace LibVLCSharp.Shared
{
    internal class MediaPlayerEventManager : EventManager
    {
        readonly object _lock = new object();
#if IOS
        static EventHandler<MediaPlayerMediaChangedEventArgs> _mediaPlayerMediaChanged;
        static EventHandler<EventArgs> _mediaPlayerNothingSpecial;
        static EventHandler<EventArgs> _mediaPlayerOpening;
        static EventHandler<MediaPlayerBufferingEventArgs> _mediaPlayerBuffering;
        static EventHandler<EventArgs> _mediaPlayerPlaying;
        static EventHandler<EventArgs> _mediaPlayerPaused;
        static EventHandler<EventArgs> _mediaPlayerStopped;
        static EventHandler<EventArgs> _mediaPlayerForward;
        static EventHandler<EventArgs> _mediaPlayerBackward;
        static EventHandler<EventArgs> _mediaPlayerEndReached;
        static EventHandler<EventArgs> _mediaPlayerEncounteredError;
        static EventHandler<MediaPlayerTimeChangedEventArgs> _mediaPlayerTimeChanged;
        static EventHandler<MediaPlayerPositionChangedEventArgs> _mediaPlayerPositionChanged;
        static EventHandler<MediaPlayerSeekableChangedEventArgs> _mediaPlayerSeekableChanged;
        static EventHandler<MediaPlayerPausableChangedEventArgs> _mediaPlayerPausableChanged;
        static EventHandler<MediaPlayerTitleChangedEventArgs> _mediaPlayerTitleChanged;
        static EventHandler<MediaPlayerChapterChangedEventArgs> _mediaPlayerChapterChanged; //vlc 3
        static EventHandler<MediaPlayerSnapshotTakenEventArgs> _mediaPlayerSnapshotTaken;
        static EventHandler<MediaPlayerLengthChangedEventArgs> _mediaPlayerLengthChanged;
        static EventHandler<MediaPlayerVoutEventArgs> _mediaPlayerVout;
        static EventHandler<MediaPlayerScrambledChangedEventArgs> _mediaPlayerScrambledChanged;
        static EventHandler<MediaPlayerESAddedEventArgs> _mediaPlayerESAdded; // vlc 3
        static EventHandler<MediaPlayerESDeletedEventArgs> _mediaPlayerESDeleted; // vlc 3
        static EventHandler<MediaPlayerESSelectedEventArgs> _mediaPlayerESSelected; // vlc 3
        static EventHandler<MediaPlayerAudioDeviceEventArgs> _mediaPlayerAudioDevice; // vlc 3
        static EventHandler<EventArgs> _mediaPlayerCorked; // vlc 2.2
        static EventHandler<EventArgs> _mediaPlayerUncorked; // vlc 2.2
        static EventHandler<EventArgs> _mediaPlayerMuted; // vlc 2.2
        static EventHandler<EventArgs> _mediaPlayerUnmuted; // vlc 2.2
        static EventHandler<MediaPlayerVolumeChangedEventArgs> _mediaPlayerVolumeChanged; // vlc 2.2
#else
        EventHandler<MediaPlayerMediaChangedEventArgs> _mediaPlayerMediaChanged;
        EventHandler<EventArgs> _mediaPlayerNothingSpecial;
        EventHandler<EventArgs> _mediaPlayerOpening;
        EventHandler<MediaPlayerBufferingEventArgs> _mediaPlayerBuffering;
        EventHandler<EventArgs> _mediaPlayerPlaying;
        EventHandler<EventArgs> _mediaPlayerPaused;
        EventHandler<EventArgs> _mediaPlayerStopped;
        EventHandler<EventArgs> _mediaPlayerForward;
        EventHandler<EventArgs> _mediaPlayerBackward;
        EventHandler<EventArgs> _mediaPlayerEndReached;
        EventHandler<EventArgs> _mediaPlayerEncounteredError;
        EventHandler<MediaPlayerTimeChangedEventArgs> _mediaPlayerTimeChanged;
        EventHandler<MediaPlayerPositionChangedEventArgs> _mediaPlayerPositionChanged;
        EventHandler<MediaPlayerSeekableChangedEventArgs> _mediaPlayerSeekableChanged;
        EventHandler<MediaPlayerPausableChangedEventArgs> _mediaPlayerPausableChanged;
        EventHandler<MediaPlayerTitleChangedEventArgs> _mediaPlayerTitleChanged;
        EventHandler<MediaPlayerChapterChangedEventArgs> _mediaPlayerChapterChanged; //vlc 3
        EventHandler<MediaPlayerSnapshotTakenEventArgs> _mediaPlayerSnapshotTaken;
        EventHandler<MediaPlayerLengthChangedEventArgs> _mediaPlayerLengthChanged;
        EventHandler<MediaPlayerVoutEventArgs> _mediaPlayerVout;
        EventHandler<MediaPlayerScrambledChangedEventArgs> _mediaPlayerScrambledChanged;
        EventHandler<MediaPlayerESAddedEventArgs> _mediaPlayerESAdded; // vlc 3
        EventHandler<MediaPlayerESDeletedEventArgs> _mediaPlayerESDeleted; // vlc 3
        EventHandler<MediaPlayerESSelectedEventArgs> _mediaPlayerESSelected; // vlc 3
        EventHandler<MediaPlayerAudioDeviceEventArgs> _mediaPlayerAudioDevice; // vlc 3
        EventHandler<EventArgs> _mediaPlayerCorked; // vlc 2.2
        EventHandler<EventArgs> _mediaPlayerUncorked; // vlc 2.2
        EventHandler<EventArgs> _mediaPlayerMuted; // vlc 2.2
        EventHandler<EventArgs> _mediaPlayerUnmuted; // vlc 2.2
        EventHandler<MediaPlayerVolumeChangedEventArgs> _mediaPlayerVolumeChanged; // vlc 2.2
#endif
        public MediaPlayerEventManager(IntPtr ptr) : base(ptr)
        {
        }

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock(_lock)
            {
                switch (eventType)
                {
                    case EventType.MediaPlayerPositionChanged:
                        _mediaPlayerPositionChanged += eventHandler as EventHandler<MediaPlayerPositionChangedEventArgs>;
                        AttachNativeEvent(eventType, OnPositionChanged);
                        break;
                    case EventType.MediaPlayerMediaChanged:
                        _mediaPlayerMediaChanged += eventHandler as EventHandler<MediaPlayerMediaChangedEventArgs>;
                        AttachNativeEvent(eventType, OnMediaChanged);
                        break;
                    case EventType.MediaPlayerNothingSpecial:
                        _mediaPlayerNothingSpecial += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnNothingSpecial);
                        break;
                    case EventType.MediaPlayerOpening:
                        _mediaPlayerOpening += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnOpening);
                        break;
                    case EventType.MediaPlayerBuffering:
                        _mediaPlayerBuffering += eventHandler as EventHandler<MediaPlayerBufferingEventArgs>;
                        AttachNativeEvent(eventType, OnBuffering);
                        break;
                    case EventType.MediaPlayerPlaying:
                        _mediaPlayerPlaying += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnPlaying);
                        break;
                    case EventType.MediaPlayerPaused:
                        _mediaPlayerPaused += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnPaused);
                        break;
                    case EventType.MediaPlayerStopped:
                        _mediaPlayerStopped += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnStopped);
                        break;
                    case EventType.MediaPlayerForward:
                        _mediaPlayerForward += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnForward);
                        break;
                    case EventType.MediaPlayerBackward:
                        _mediaPlayerBackward += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnBackward);
                        break;
                    case EventType.MediaPlayerEndReached:
                        _mediaPlayerEndReached += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnEndReached);
                        break;
                    case EventType.MediaPlayerEncounteredError:
                        _mediaPlayerEncounteredError += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnEncounteredError);
                        break;
                    case EventType.MediaPlayerTimeChanged:
                        _mediaPlayerTimeChanged += eventHandler as EventHandler<MediaPlayerTimeChangedEventArgs>;
                        AttachNativeEvent(eventType, OnTimeChanged);
                        break;
                    case EventType.MediaPlayerSeekableChanged:
                        _mediaPlayerSeekableChanged += eventHandler as EventHandler<MediaPlayerSeekableChangedEventArgs>;
                        AttachNativeEvent(eventType, OnSeekableChanged);
                        break;
                    case EventType.MediaPlayerPausableChanged:
                        _mediaPlayerPausableChanged += eventHandler as EventHandler<MediaPlayerPausableChangedEventArgs>;
                        AttachNativeEvent(eventType, OnPausableChanged);
                        break;
                    case EventType.MediaPlayerTitleChanged:
                        _mediaPlayerTitleChanged += eventHandler as EventHandler<MediaPlayerTitleChangedEventArgs>;
                        AttachNativeEvent(eventType, OnTitleChanged);
                        break;
                    case EventType.MediaPlayerChapterChanged:
                        _mediaPlayerChapterChanged += eventHandler as EventHandler<MediaPlayerChapterChangedEventArgs>;
                        AttachNativeEvent(eventType, OnChapterChanged);
                        break;
                    case EventType.MediaPlayerSnapshotTaken:
                        _mediaPlayerSnapshotTaken += eventHandler as EventHandler<MediaPlayerSnapshotTakenEventArgs>;
                        AttachNativeEvent(eventType, OnSnapshotTaken);
                        break;
                    case EventType.MediaPlayerLengthChanged:
                        _mediaPlayerLengthChanged += eventHandler as EventHandler<MediaPlayerLengthChangedEventArgs>;
                        AttachNativeEvent(eventType, OnLengthChanged);
                        break;
                    case EventType.MediaPlayerVout:
                        _mediaPlayerVout += eventHandler as EventHandler<MediaPlayerVoutEventArgs>;
                        AttachNativeEvent(eventType, OnVout);
                        break;
                    case EventType.MediaPlayerScrambledChanged:
                        _mediaPlayerScrambledChanged += eventHandler as EventHandler<MediaPlayerScrambledChangedEventArgs>;
                        AttachNativeEvent(eventType, OnScrambledChanged);
                        break;
                    case EventType.MediaPlayerESAdded:
                        _mediaPlayerESAdded += eventHandler as EventHandler<MediaPlayerESAddedEventArgs>;
                        AttachNativeEvent(eventType, OnESAdded);
                        break;
                    case EventType.MediaPlayerESDeleted:
                        _mediaPlayerESDeleted += eventHandler as EventHandler<MediaPlayerESDeletedEventArgs>;
                        AttachNativeEvent(eventType, OnESDeleted);
                        break;
                    case EventType.MediaPlayerESSelected:
                        _mediaPlayerESSelected += eventHandler as EventHandler<MediaPlayerESSelectedEventArgs>;
                        AttachNativeEvent(eventType, OnESSelected);
                        break;
                    case EventType.MediaPlayerAudioDevice:
                        _mediaPlayerAudioDevice += eventHandler as EventHandler<MediaPlayerAudioDeviceEventArgs>;
                        AttachNativeEvent(eventType, OnAudioDevice);
                        break;
                    case EventType.MediaPlayerCorked:
                        _mediaPlayerCorked += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnCorked);
                        break;
                    case EventType.MediaPlayerUncorked:
                        _mediaPlayerUncorked += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnUncorked);
                        break;
                    case EventType.MediaPlayerMuted:
                        _mediaPlayerMuted += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnMuted);
                        break;
                    case EventType.MediaPlayerUnmuted:
                        _mediaPlayerUnmuted += eventHandler as EventHandler<EventArgs>;
                        AttachNativeEvent(eventType, OnUnmuted);
                        break;
                    case EventType.MediaPlayerAudioVolume:
                        _mediaPlayerVolumeChanged += eventHandler as EventHandler<MediaPlayerVolumeChangedEventArgs>;
                        AttachNativeEvent(eventType, OnVolumeChanged);
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
                    case EventType.MediaPlayerPositionChanged:
                        _mediaPlayerPositionChanged -= eventHandler as EventHandler<MediaPlayerPositionChangedEventArgs>;
                        DetachNativeEvent(eventType, OnPositionChanged);
                        break;
                    case EventType.MediaPlayerMediaChanged:
                        _mediaPlayerMediaChanged -= eventHandler as EventHandler<MediaPlayerMediaChangedEventArgs>;
                        DetachNativeEvent(eventType, OnMediaChanged);
                        break;
                    case EventType.MediaPlayerNothingSpecial:
                        _mediaPlayerNothingSpecial -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnNothingSpecial);
                        break;
                    case EventType.MediaPlayerOpening:
                        _mediaPlayerOpening -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnOpening);
                        break;
                    case EventType.MediaPlayerBuffering:
                        _mediaPlayerBuffering -= eventHandler as EventHandler<MediaPlayerBufferingEventArgs>;
                        DetachNativeEvent(eventType, OnBuffering);
                        break;
                    case EventType.MediaPlayerPlaying:
                        _mediaPlayerPlaying -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnPlaying);
                        break;
                    case EventType.MediaPlayerPaused:
                        _mediaPlayerPaused -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnPaused);
                        break;
                    case EventType.MediaPlayerStopped:
                        _mediaPlayerStopped -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnStopped);
                        break;
                    case EventType.MediaPlayerForward:
                        _mediaPlayerForward -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnForward);
                        break;
                    case EventType.MediaPlayerBackward:
                        _mediaPlayerBackward -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnBackward);
                        break;
                    case EventType.MediaPlayerEndReached:
                        _mediaPlayerEndReached -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnEndReached);
                        break;
                    case EventType.MediaPlayerEncounteredError:
                        _mediaPlayerEncounteredError -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnEncounteredError);
                        break;
                    case EventType.MediaPlayerTimeChanged:
                        _mediaPlayerTimeChanged -= eventHandler as EventHandler<MediaPlayerTimeChangedEventArgs>;
                        DetachNativeEvent(eventType, OnTimeChanged);
                        break;
                    case EventType.MediaPlayerSeekableChanged:
                        _mediaPlayerSeekableChanged -= eventHandler as EventHandler<MediaPlayerSeekableChangedEventArgs>;
                        DetachNativeEvent(eventType, OnSeekableChanged);
                        break;
                    case EventType.MediaPlayerPausableChanged:
                        _mediaPlayerPausableChanged -= eventHandler as EventHandler<MediaPlayerPausableChangedEventArgs>;
                        DetachNativeEvent(eventType, OnPausableChanged);
                        break;
                    case EventType.MediaPlayerTitleChanged:
                        _mediaPlayerTitleChanged -= eventHandler as EventHandler<MediaPlayerTitleChangedEventArgs>;
                        DetachNativeEvent(eventType, OnTitleChanged);
                        break;
                    case EventType.MediaPlayerChapterChanged:
                        _mediaPlayerChapterChanged -= eventHandler as EventHandler<MediaPlayerChapterChangedEventArgs>;
                        DetachNativeEvent(eventType, OnChapterChanged);
                        break;
                    case EventType.MediaPlayerSnapshotTaken:
                        _mediaPlayerSnapshotTaken -= eventHandler as EventHandler<MediaPlayerSnapshotTakenEventArgs>;
                        DetachNativeEvent(eventType, OnSnapshotTaken);
                        break;
                    case EventType.MediaPlayerLengthChanged:
                        _mediaPlayerLengthChanged -= eventHandler as EventHandler<MediaPlayerLengthChangedEventArgs>;
                        DetachNativeEvent(eventType, OnLengthChanged);
                        break;
                    case EventType.MediaPlayerVout:
                        _mediaPlayerVout -= eventHandler as EventHandler<MediaPlayerVoutEventArgs>;
                        DetachNativeEvent(eventType, OnVout);
                        break;
                    case EventType.MediaPlayerScrambledChanged:
                        _mediaPlayerScrambledChanged -= eventHandler as EventHandler<MediaPlayerScrambledChangedEventArgs>;
                        DetachNativeEvent(eventType, OnScrambledChanged);
                        break;
                    case EventType.MediaPlayerESAdded:
                        _mediaPlayerESAdded -= eventHandler as EventHandler<MediaPlayerESAddedEventArgs>;
                        DetachNativeEvent(eventType, OnESAdded);
                        break;
                    case EventType.MediaPlayerESDeleted:
                        _mediaPlayerESDeleted -= eventHandler as EventHandler<MediaPlayerESDeletedEventArgs>;
                        DetachNativeEvent(eventType, OnESDeleted);
                        break;
                    case EventType.MediaPlayerESSelected:
                        _mediaPlayerESSelected -= eventHandler as EventHandler<MediaPlayerESSelectedEventArgs>;
                        DetachNativeEvent(eventType, OnESSelected);
                        break;
                    case EventType.MediaPlayerAudioDevice:
                        _mediaPlayerAudioDevice -= eventHandler as EventHandler<MediaPlayerAudioDeviceEventArgs>;
                        DetachNativeEvent(eventType, OnAudioDevice);
                        break;
                    case EventType.MediaPlayerCorked:
                        _mediaPlayerCorked -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnCorked);
                        break;
                    case EventType.MediaPlayerUncorked:
                        _mediaPlayerUncorked -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnUncorked);
                        break;
                    case EventType.MediaPlayerMuted:
                        _mediaPlayerMuted -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnMuted);
                        break;
                    case EventType.MediaPlayerUnmuted:
                        _mediaPlayerUnmuted -= eventHandler as EventHandler<EventArgs>;
                        DetachNativeEvent(eventType, OnUnmuted);
                        break;
                    case EventType.MediaPlayerAudioVolume:
                        _mediaPlayerVolumeChanged -= eventHandler as EventHandler<MediaPlayerVolumeChangedEventArgs>;
                        DetachNativeEvent(eventType, OnVolumeChanged);
                        break;
                    default:
                        OnEventUnhandled(this, eventType);
                        break;
                }
            }
        }
        
#if IOS
        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnMediaChanged(IntPtr ptr)
        {
            _mediaPlayerMediaChanged?.Invoke(null, 
                new MediaPlayerMediaChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerMediaChanged.NewMedia));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnNothingSpecial(IntPtr ptr)
        {
            _mediaPlayerNothingSpecial?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnOpening(IntPtr ptr)
        {
            _mediaPlayerOpening?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnBuffering(IntPtr ptr)
        {
            _mediaPlayerBuffering?.Invoke(null,
                new MediaPlayerBufferingEventArgs(RetrieveEvent(ptr).Union.MediaPlayerBuffering.NewCache));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnPlaying(IntPtr ptr)
        {
            _mediaPlayerPlaying?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnPaused(IntPtr ptr)
        {
            _mediaPlayerPaused?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnStopped(IntPtr ptr)
        {
            _mediaPlayerStopped?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnForward(IntPtr ptr)
        {
            _mediaPlayerForward?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnBackward(IntPtr ptr)
        {
            _mediaPlayerBackward?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnEndReached(IntPtr ptr)
        {
            _mediaPlayerEndReached?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnEncounteredError(IntPtr ptr)
        {
            _mediaPlayerEncounteredError?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnTimeChanged(IntPtr ptr)
        {
            _mediaPlayerTimeChanged?.Invoke(null,
                new MediaPlayerTimeChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerTimeChanged.NewTime));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnPositionChanged(IntPtr ptr)
        {
            _mediaPlayerPositionChanged?.Invoke(null,
                new MediaPlayerPositionChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerPositionChanged.NewPosition));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnSeekableChanged(IntPtr ptr)
        {
            _mediaPlayerSeekableChanged?.Invoke(null,
                new MediaPlayerSeekableChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerSeekableChanged.NewSeekable));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnPausableChanged(IntPtr ptr)
        {
            _mediaPlayerPausableChanged?.Invoke(null, 
                new MediaPlayerPausableChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerPausableChanged.NewPausable));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnTitleChanged(IntPtr ptr)
        {
            _mediaPlayerTitleChanged?.Invoke(null,
                new MediaPlayerTitleChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerTitleChanged.NewTitle));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnChapterChanged(IntPtr ptr)
        {
            _mediaPlayerChapterChanged?.Invoke(null,
                new MediaPlayerChapterChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerChapterChanged.NewChapter));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnSnapshotTaken(IntPtr ptr)
        {
            var filenamePtr = RetrieveEvent(ptr).Union.MediaPlayerSnapshotTaken.Filename;
            var filename = (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(filenamePtr);
            _mediaPlayerSnapshotTaken?.Invoke(null, new MediaPlayerSnapshotTakenEventArgs(filename));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnLengthChanged(IntPtr ptr)
        {
            _mediaPlayerLengthChanged?.Invoke(null, 
                new MediaPlayerLengthChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerLengthChanged.NewLength));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnVout(IntPtr ptr)
        {
            _mediaPlayerVout?.Invoke(null, 
                new MediaPlayerVoutEventArgs(RetrieveEvent(ptr).Union.MediaPlayerVoutChanged.NewCount));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnScrambledChanged(IntPtr ptr)
        {
            _mediaPlayerScrambledChanged?.Invoke(null,
                new MediaPlayerScrambledChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerScrambledChanged.NewScrambled));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnESAdded(IntPtr ptr)
        {
            _mediaPlayerESAdded?.Invoke(null,
                new MediaPlayerESAddedEventArgs(RetrieveEvent(ptr).Union.EsChanged.Id));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnESDeleted(IntPtr ptr)
        {
            _mediaPlayerESDeleted?.Invoke(null,
                new MediaPlayerESDeletedEventArgs(RetrieveEvent(ptr).Union.EsChanged.Id));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnESSelected(IntPtr ptr)
        {
            _mediaPlayerESSelected?.Invoke(null,
                new MediaPlayerESSelectedEventArgs(RetrieveEvent(ptr).Union.EsChanged.Id));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnAudioDevice(IntPtr ptr)
        {
            var deviceNamePtr = RetrieveEvent(ptr).Union.AudioDeviceChanged.Device;
            var deviceName = (string) Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(deviceNamePtr);
            _mediaPlayerAudioDevice?.Invoke(null, new MediaPlayerAudioDeviceEventArgs(deviceName));
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnCorked(IntPtr ptr)
        {
            _mediaPlayerCorked?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnUncorked(IntPtr ptr)
        {
            _mediaPlayerUncorked?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnMuted(IntPtr ptr)
        {
            _mediaPlayerMuted?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnUnmuted(IntPtr ptr)
        {
            _mediaPlayerUnmuted?.Invoke(null, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(EventCallback))]
        static void OnVolumeChanged(IntPtr ptr)
        {
            _mediaPlayerVolumeChanged?.Invoke(null, 
                new MediaPlayerVolumeChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerVolumeChanged.Volume));
        }
#else
        void OnMediaChanged(IntPtr ptr)
        {
            _mediaPlayerMediaChanged?.Invoke(this,
                new MediaPlayerMediaChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerMediaChanged.NewMedia));
        }

        void OnNothingSpecial(IntPtr ptr)
        {
            _mediaPlayerNothingSpecial?.Invoke(this, EventArgs.Empty);
        }

        void OnOpening(IntPtr ptr)
        {
            _mediaPlayerOpening?.Invoke(this, EventArgs.Empty);
        }

        void OnBuffering(IntPtr ptr)
        {
            _mediaPlayerBuffering?.Invoke(this,
                new MediaPlayerBufferingEventArgs(RetrieveEvent(ptr).Union.MediaPlayerBuffering.NewCache));
        }

        void OnPlaying(IntPtr ptr)
        {
            _mediaPlayerPlaying?.Invoke(this, EventArgs.Empty);
        }

        void OnPaused(IntPtr ptr)
        {
            _mediaPlayerPaused?.Invoke(this, EventArgs.Empty);
        }

        void OnStopped(IntPtr ptr)
        {
            _mediaPlayerStopped?.Invoke(this, EventArgs.Empty);
        }

        void OnForward(IntPtr ptr)
        {
            _mediaPlayerForward?.Invoke(this, EventArgs.Empty);
        }

        void OnBackward(IntPtr ptr)
        {
            _mediaPlayerBackward?.Invoke(this, EventArgs.Empty);
        }

        void OnEndReached(IntPtr ptr)
        {
            _mediaPlayerEndReached?.Invoke(this, EventArgs.Empty);
        }

        void OnEncounteredError(IntPtr ptr)
        {
            _mediaPlayerEncounteredError?.Invoke(this, EventArgs.Empty);
        }

        void OnTimeChanged(IntPtr ptr)
        {
            _mediaPlayerTimeChanged?.Invoke(this,
                new MediaPlayerTimeChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerTimeChanged.NewTime));
        }

        void OnPositionChanged(IntPtr ptr)
        {
            _mediaPlayerPositionChanged?.Invoke(this,
                new MediaPlayerPositionChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerPositionChanged.NewPosition));
        }

        void OnSeekableChanged(IntPtr ptr)
        {
            _mediaPlayerSeekableChanged?.Invoke(this,
                new MediaPlayerSeekableChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerSeekableChanged.NewSeekable));
        }

        void OnPausableChanged(IntPtr ptr)
        {
            _mediaPlayerPausableChanged?.Invoke(this,
                new MediaPlayerPausableChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerPausableChanged.NewPausable));
        }

        void OnTitleChanged(IntPtr ptr)
        {
            _mediaPlayerTitleChanged?.Invoke(this,
                new MediaPlayerTitleChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerTitleChanged.NewTitle));
        }

        void OnChapterChanged(IntPtr ptr)
        {
            _mediaPlayerChapterChanged?.Invoke(this,
                new MediaPlayerChapterChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerChapterChanged.NewChapter));
        }

        void OnSnapshotTaken(IntPtr ptr)
        {
            var filenamePtr = RetrieveEvent(ptr).Union.MediaPlayerSnapshotTaken.Filename;
            var filename = (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(filenamePtr);
            _mediaPlayerSnapshotTaken?.Invoke(this, new MediaPlayerSnapshotTakenEventArgs(filename));
        }

        void OnLengthChanged(IntPtr ptr)
        {
            _mediaPlayerLengthChanged?.Invoke(this,
                new MediaPlayerLengthChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerLengthChanged.NewLength));
        }

        void OnVout(IntPtr ptr)
        {
            _mediaPlayerVout?.Invoke(this,
                new MediaPlayerVoutEventArgs(RetrieveEvent(ptr).Union.MediaPlayerVoutChanged.NewCount));
        }

        void OnScrambledChanged(IntPtr ptr)
        {
            _mediaPlayerScrambledChanged?.Invoke(this,
                new MediaPlayerScrambledChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerScrambledChanged.NewScrambled));
        }

        void OnESAdded(IntPtr ptr)
        {
            _mediaPlayerESAdded?.Invoke(this,
                new MediaPlayerESAddedEventArgs(RetrieveEvent(ptr).Union.EsChanged.Id));
        }

        void OnESDeleted(IntPtr ptr)
        {
            _mediaPlayerESDeleted?.Invoke(this,
                new MediaPlayerESDeletedEventArgs(RetrieveEvent(ptr).Union.EsChanged.Id));
        }

        void OnESSelected(IntPtr ptr)
        {
            _mediaPlayerESSelected?.Invoke(this,
                new MediaPlayerESSelectedEventArgs(RetrieveEvent(ptr).Union.EsChanged.Id));
        }

        void OnAudioDevice(IntPtr ptr)
        {
            var deviceNamePtr = RetrieveEvent(ptr).Union.AudioDeviceChanged.Device;
            var deviceName = (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(deviceNamePtr);
            _mediaPlayerAudioDevice?.Invoke(this, new MediaPlayerAudioDeviceEventArgs(deviceName));
        }

        void OnCorked(IntPtr ptr)
        {
            _mediaPlayerCorked?.Invoke(this, EventArgs.Empty);
        }

        void OnUncorked(IntPtr ptr)
        {
            _mediaPlayerUncorked?.Invoke(this, EventArgs.Empty);
        }

        void OnMuted(IntPtr ptr)
        {
            _mediaPlayerMuted?.Invoke(this, EventArgs.Empty);
        }

        void OnUnmuted(IntPtr ptr)
        {
            _mediaPlayerUnmuted?.Invoke(this, EventArgs.Empty);
        }

        void OnVolumeChanged(IntPtr ptr)
        {
            _mediaPlayerVolumeChanged?.Invoke(this,
                new MediaPlayerVolumeChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerVolumeChanged.Volume));
        }
#endif

    }

    public class MediaPlayerChangedEventArgs : EventArgs
    {
        public MediaPlayerChangedEventArgs(LibVLCSharp.Shared.MediaPlayer oldMediaPlayer, LibVLCSharp.Shared.MediaPlayer newMediaPlayer)
        {
            OldMediaPlayer = oldMediaPlayer;
            NewMediaPlayer = newMediaPlayer;
        }

        public LibVLCSharp.Shared.MediaPlayer OldMediaPlayer { get; }
        public LibVLCSharp.Shared.MediaPlayer NewMediaPlayer { get; }
    }
}