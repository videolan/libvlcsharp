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
        EventHandler<MediaPlayerPositionChangedEventArgs> _mediaPlayerPositionChanged;
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

        int _positionChangedRegistrationCount;
        int _mediaChangedRegistrationCount;
        int _nothingSpecialRegistrationCount;
        int _openingRegistrationCount;
        int _bufferingRegistrationCount;
        int _playingRegistrationCount;
        int _pausedRegistrationCount;
        int _stoppedRegistrationCount;
        int _forwardRegistrationCount;
        int _backwardRegistrationCount;
        int _endReachedRegistrationCount;
        int _encounteredErrorRegistrationCount;
        int _timeChangedRegistrationCount;
        int _seekableChangedRegistrationCount;
        int _pausableChangedRegistrationCount;
        int _titleChangedChangedRegistrationCount;
        int _chapterChangedRegistrationCount;
        int _snapshotTakenRegistrationCount;
        int _lengthChangedRegistrationCount;
        int _voutRegistrationCount;
        int _scrambledChangedRegistrationCount;
        int _eSAddedRegistrationCount;
        int _eSDeletedRegistrationCount;
        int _eSSelectedRegistrationCount;
        int _audioDeviceRegistrationCount;
        int _corkedRegistrationCount;
        int _uncorkedRegistrationCount;
        int _mutedRegistrationCount;
        int _unmutedRegistrationCount;
        int _volumeChangedRegistrationCount;
        
        EventCallback _positionChangedCallback;
        EventCallback _mediaChangedCallback;
        EventCallback _nothingSpecialCallback;
        EventCallback _openingCallback;
        EventCallback _bufferingCallback;
        EventCallback _playingCallback;
        EventCallback _pausedCallback;
        EventCallback _stoppedCallback;
        EventCallback _forwardCallback;
        EventCallback _backwardCallback;
        EventCallback _endReachedCallback;
        EventCallback _encounteredErrorCallback;
        EventCallback _timeChangedCallback;
        EventCallback _seekableChangedCallback;
        EventCallback _pausableChangedCallback;
        EventCallback _titleChangedCallback;
        EventCallback _chapterChangedCallback;
        EventCallback _snapshotTakenCallback;
        EventCallback _lengthChangedCallback;
        EventCallback _voutCallback;
        EventCallback _scrambledChangedCallback;
        EventCallback _eSAddedCallback;
        EventCallback _eSDeletedCallback;
        EventCallback _eSSelectedCallback;
        EventCallback _audioDeviceCallback;
        EventCallback _corkedCallback;
        EventCallback _uncorkedCallback;
        EventCallback _mutedCallback;
        EventCallback _unmutedCallback;
        EventCallback _volumeChangedCallback;

        protected internal override void AttachEvent<T>(EventType eventType, EventHandler<T> eventHandler)
        {
            lock(_lock)
            {
                switch (eventType)
                {
                    case EventType.MediaPlayerPositionChanged:
                        Attach(eventType,
                            ref _positionChangedRegistrationCount,
                            () => _mediaPlayerPositionChanged += eventHandler as EventHandler<MediaPlayerPositionChangedEventArgs>,
                            () => _positionChangedCallback = OnPositionChanged);
                        break;
                    case EventType.MediaPlayerMediaChanged:
                        Attach(eventType,
                            ref _mediaChangedRegistrationCount,
                            () => _mediaPlayerMediaChanged += eventHandler as EventHandler<MediaPlayerMediaChangedEventArgs>,
                            () => _mediaChangedCallback = OnMediaChanged);
                        break;
                    case EventType.MediaPlayerNothingSpecial:
                        Attach(eventType,
                            ref _nothingSpecialRegistrationCount,
                            () => _mediaPlayerNothingSpecial += eventHandler as EventHandler<EventArgs>,
                            () => _nothingSpecialCallback = OnNothingSpecial);
                        break;
                    case EventType.MediaPlayerOpening:
                        Attach(eventType,
                            ref _openingRegistrationCount,
                            () => _mediaPlayerOpening += eventHandler as EventHandler<EventArgs>,
                            () => _openingCallback = OnOpening);
                        break;
                    case EventType.MediaPlayerBuffering:
                        Attach(eventType,
                           ref _bufferingRegistrationCount,
                           () => _mediaPlayerBuffering += eventHandler as EventHandler<MediaPlayerBufferingEventArgs>,
                           () => _bufferingCallback = OnBuffering);
                        break;
                    case EventType.MediaPlayerPlaying:
                        Attach(eventType,
                            ref _playingRegistrationCount,
                            () => _mediaPlayerPlaying += eventHandler as EventHandler<EventArgs>,
                            () => _playingCallback = OnPlaying);
                        break;
                    case EventType.MediaPlayerPaused:
                        Attach(eventType,
                            ref _pausedRegistrationCount,
                            () => _mediaPlayerPaused += eventHandler as EventHandler<EventArgs>,
                            () => _pausedCallback = OnPaused);
                        break;
                    case EventType.MediaPlayerStopped:
                        Attach(eventType,
                            ref _stoppedRegistrationCount,
                            () => _mediaPlayerStopped += eventHandler as EventHandler<EventArgs>,
                            () => _stoppedCallback = OnStopped);
                        break;
                    case EventType.MediaPlayerForward:
                        Attach(eventType,
                            ref _forwardRegistrationCount,
                            () => _mediaPlayerForward += eventHandler as EventHandler<EventArgs>,
                            () => _forwardCallback = OnForward);
                        break;
                    case EventType.MediaPlayerBackward:
                        Attach(eventType,
                            ref _backwardRegistrationCount,
                            () => _mediaPlayerBackward += eventHandler as EventHandler<EventArgs>,
                            () => _backwardCallback = OnBackward);
                        break;
                    case EventType.MediaPlayerEndReached:
                        Attach(eventType,
                           ref _endReachedRegistrationCount,
                           () => _mediaPlayerEndReached += eventHandler as EventHandler<EventArgs>,
                           () => _endReachedCallback = OnEndReached);
                        break;
                    case EventType.MediaPlayerEncounteredError:
                        Attach(eventType,
                           ref _encounteredErrorRegistrationCount,
                           () => _mediaPlayerEncounteredError += eventHandler as EventHandler<EventArgs>,
                           () => _encounteredErrorCallback = OnEncounteredError);
                        break;
                    case EventType.MediaPlayerTimeChanged:
                        Attach(eventType,
                           ref _timeChangedRegistrationCount,
                           () => _mediaPlayerTimeChanged += eventHandler as EventHandler<MediaPlayerTimeChangedEventArgs>,
                           () => _timeChangedCallback = OnTimeChanged);
                        break;
                    case EventType.MediaPlayerSeekableChanged:
                        Attach(eventType,
                          ref _seekableChangedRegistrationCount,
                          () => _mediaPlayerSeekableChanged += eventHandler as EventHandler<MediaPlayerSeekableChangedEventArgs>,
                          () => _seekableChangedCallback = OnSeekableChanged);
                        break;
                    case EventType.MediaPlayerPausableChanged:
                        Attach(eventType,
                          ref _pausableChangedRegistrationCount,
                          () => _mediaPlayerPausableChanged += eventHandler as EventHandler<MediaPlayerPausableChangedEventArgs>,
                          () => _pausableChangedCallback = OnPausableChanged);
                        break;
                    case EventType.MediaPlayerTitleChanged:
                        Attach(eventType,
                          ref _titleChangedChangedRegistrationCount,
                          () => _mediaPlayerTitleChanged += eventHandler as EventHandler<MediaPlayerTitleChangedEventArgs>,
                          () => _titleChangedCallback = OnTitleChanged);
                        break;
                    case EventType.MediaPlayerChapterChanged:
                        Attach(eventType,
                          ref _chapterChangedRegistrationCount,
                          () => _mediaPlayerChapterChanged += eventHandler as EventHandler<MediaPlayerChapterChangedEventArgs>,
                          () => _chapterChangedCallback = OnChapterChanged);
                        break;
                    case EventType.MediaPlayerSnapshotTaken:
                        Attach(eventType,
                          ref _snapshotTakenRegistrationCount,
                          () => _mediaPlayerSnapshotTaken += eventHandler as EventHandler<MediaPlayerSnapshotTakenEventArgs>,
                          () => _snapshotTakenCallback = OnSnapshotTaken);
                        break;
                    case EventType.MediaPlayerLengthChanged:
                        Attach(eventType,
                          ref _lengthChangedRegistrationCount,
                          () => _mediaPlayerLengthChanged += eventHandler as EventHandler<MediaPlayerLengthChangedEventArgs>,
                          () => _lengthChangedCallback = OnLengthChanged);
                        break;
                    case EventType.MediaPlayerVout:
                        Attach(eventType,
                          ref _voutRegistrationCount,
                          () => _mediaPlayerVout += eventHandler as EventHandler<MediaPlayerVoutEventArgs>,
                          () => _voutCallback = OnVout);
                        break;
                    case EventType.MediaPlayerScrambledChanged:
                        Attach(eventType,
                          ref _scrambledChangedRegistrationCount,
                          () => _mediaPlayerScrambledChanged += eventHandler as EventHandler<MediaPlayerScrambledChangedEventArgs>,
                          () => _scrambledChangedCallback = OnScrambledChanged);
                        break;
                    case EventType.MediaPlayerESAdded:
                        Attach(eventType,
                         ref _eSAddedRegistrationCount,
                         () => _mediaPlayerESAdded += eventHandler as EventHandler<MediaPlayerESAddedEventArgs>,
                         () => _eSAddedCallback = OnESAdded);
                        break;
                    case EventType.MediaPlayerESDeleted:
                        Attach(eventType,
                         ref _eSDeletedRegistrationCount,
                         () => _mediaPlayerESDeleted += eventHandler as EventHandler<MediaPlayerESDeletedEventArgs>,
                         () => _eSDeletedCallback = OnESDeleted);
                        break;
                    case EventType.MediaPlayerESSelected:
                        Attach(eventType,
                        ref _eSSelectedRegistrationCount,
                        () => _mediaPlayerESSelected += eventHandler as EventHandler<MediaPlayerESSelectedEventArgs>,
                        () => _eSSelectedCallback = OnESSelected);
                       break;
                    case EventType.MediaPlayerAudioDevice:
                        Attach(eventType,
                        ref _audioDeviceRegistrationCount,
                        () => _mediaPlayerAudioDevice += eventHandler as EventHandler<MediaPlayerAudioDeviceEventArgs>,
                        () => _audioDeviceCallback = OnAudioDevice);
                       break;
                    case EventType.MediaPlayerCorked:
                        Attach(eventType,
                        ref _corkedRegistrationCount,
                        () => _mediaPlayerCorked += eventHandler as EventHandler<EventArgs>,
                        () => _corkedCallback = OnCorked);
                       break;
                    case EventType.MediaPlayerUncorked:
                        Attach(eventType,
                        ref _uncorkedRegistrationCount,
                        () => _mediaPlayerUncorked += eventHandler as EventHandler<EventArgs>,
                        () => _uncorkedCallback = OnUncorked);
                       break;
                    case EventType.MediaPlayerMuted:
                        Attach(eventType,
                        ref _mutedRegistrationCount,
                        () => _mediaPlayerMuted += eventHandler as EventHandler<EventArgs>,
                        () => _mutedCallback = OnMuted);
                       break;
                    case EventType.MediaPlayerUnmuted:
                        Attach(eventType,
                        ref _unmutedRegistrationCount,
                        () => _mediaPlayerUnmuted += eventHandler as EventHandler<EventArgs>,
                        () => _unmutedCallback = OnUnmuted);
                       break;
                    case EventType.MediaPlayerAudioVolume:
                        Attach(eventType,
                        ref _volumeChangedRegistrationCount,
                        () => _mediaPlayerVolumeChanged += eventHandler as EventHandler<MediaPlayerVolumeChangedEventArgs>,
                        () => _volumeChangedCallback = OnVolumeChanged);
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
                        Detach(eventType,
                            ref _positionChangedRegistrationCount,
                            () => _mediaPlayerPositionChanged -= eventHandler as EventHandler<MediaPlayerPositionChangedEventArgs>,
                            ref _positionChangedCallback);
                        break;
                    case EventType.MediaPlayerMediaChanged:
                        Detach(eventType,
                            ref _mediaChangedRegistrationCount,
                            () => _mediaPlayerMediaChanged -= eventHandler as EventHandler<MediaPlayerMediaChangedEventArgs>,
                            ref _mediaChangedCallback);
                        break;
                    case EventType.MediaPlayerNothingSpecial:
                        Detach(eventType,
                            ref _nothingSpecialRegistrationCount,
                            () => _mediaPlayerNothingSpecial -= eventHandler as EventHandler<EventArgs>,
                            ref _nothingSpecialCallback);
                        break;
                    case EventType.MediaPlayerOpening:
                        Detach(eventType,
                           ref _openingRegistrationCount,
                           () => _mediaPlayerOpening -= eventHandler as EventHandler<EventArgs>,
                           ref _openingCallback);
                        break;
                    case EventType.MediaPlayerBuffering:
                        Detach(eventType,
                           ref _bufferingRegistrationCount,
                           () => _mediaPlayerBuffering -= eventHandler as EventHandler<MediaPlayerBufferingEventArgs>,
                           ref _bufferingCallback);
                        break;
                    case EventType.MediaPlayerPlaying:
                        Detach(eventType,
                            ref _playingRegistrationCount,
                            () => _mediaPlayerPlaying -= eventHandler as EventHandler<EventArgs>,
                            ref _playingCallback);
                        break;
                    case EventType.MediaPlayerPaused:
                        Detach(eventType,
                           ref _pausedRegistrationCount,
                           () => _mediaPlayerPaused -= eventHandler as EventHandler<EventArgs>,
                           ref _pausedCallback);
                        break;
                    case EventType.MediaPlayerStopped:
                        Detach(eventType,
                           ref _stoppedRegistrationCount,
                           () => _mediaPlayerStopped -= eventHandler as EventHandler<EventArgs>,
                           ref _stoppedCallback);
                        break;
                    case EventType.MediaPlayerForward:
                        Detach(eventType,
                           ref _forwardRegistrationCount,
                           () => _mediaPlayerForward -= eventHandler as EventHandler<EventArgs>,
                           ref _forwardCallback);
                        break;
                    case EventType.MediaPlayerBackward:
                        Detach(eventType,
                           ref _backwardRegistrationCount,
                           () => _mediaPlayerBackward -= eventHandler as EventHandler<EventArgs>,
                           ref _backwardCallback);
                        break;
                    case EventType.MediaPlayerEndReached:
                        Detach(eventType,
                           ref _endReachedRegistrationCount,
                           () => _mediaPlayerEndReached -= eventHandler as EventHandler<EventArgs>,
                           ref _endReachedCallback);
                        break;
                    case EventType.MediaPlayerEncounteredError:
                        Detach(eventType,
                          ref _encounteredErrorRegistrationCount,
                          () => _mediaPlayerEncounteredError -= eventHandler as EventHandler<EventArgs>,
                          ref _encounteredErrorCallback);
                        break;
                    case EventType.MediaPlayerTimeChanged:
                        Detach(eventType,
                          ref _timeChangedRegistrationCount,
                          () => _mediaPlayerTimeChanged -= eventHandler as EventHandler<MediaPlayerTimeChangedEventArgs>,
                          ref _timeChangedCallback);
                        break;
                    case EventType.MediaPlayerSeekableChanged:
                        Detach(eventType,
                          ref _seekableChangedRegistrationCount,
                          () => _mediaPlayerSeekableChanged -= eventHandler as EventHandler<MediaPlayerSeekableChangedEventArgs>,
                          ref _seekableChangedCallback);
                        break;
                    case EventType.MediaPlayerPausableChanged:
                        Detach(eventType,
                          ref _pausableChangedRegistrationCount,
                          () => _mediaPlayerPausableChanged -= eventHandler as EventHandler<MediaPlayerPausableChangedEventArgs>,
                          ref _pausableChangedCallback);
                        break;
                    case EventType.MediaPlayerTitleChanged:
                        Detach(eventType,
                          ref _titleChangedChangedRegistrationCount,
                          () => _mediaPlayerTitleChanged -= eventHandler as EventHandler<MediaPlayerTitleChangedEventArgs>,
                          ref _titleChangedCallback);
                        break;
                    case EventType.MediaPlayerChapterChanged:
                        Detach(eventType,
                          ref _chapterChangedRegistrationCount,
                          () => _mediaPlayerChapterChanged -= eventHandler as EventHandler<MediaPlayerChapterChangedEventArgs>,
                          ref _chapterChangedCallback);
                        break;
                    case EventType.MediaPlayerSnapshotTaken:
                        Detach(eventType,
                          ref _snapshotTakenRegistrationCount,
                          () => _mediaPlayerSnapshotTaken -= eventHandler as EventHandler<MediaPlayerSnapshotTakenEventArgs>,
                          ref _snapshotTakenCallback);
                        break;
                    case EventType.MediaPlayerLengthChanged:
                        Detach(eventType,
                          ref _lengthChangedRegistrationCount,
                          () => _mediaPlayerLengthChanged -= eventHandler as EventHandler<MediaPlayerLengthChangedEventArgs>,
                          ref _lengthChangedCallback);
                        break;
                    case EventType.MediaPlayerVout:
                        Detach(eventType,
                          ref _voutRegistrationCount,
                          () => _mediaPlayerVout -= eventHandler as EventHandler<MediaPlayerVoutEventArgs>,
                          ref _voutCallback);
                        break;
                    case EventType.MediaPlayerScrambledChanged:
                        Detach(eventType,
                          ref _scrambledChangedRegistrationCount,
                          () => _mediaPlayerScrambledChanged -= eventHandler as EventHandler<MediaPlayerScrambledChangedEventArgs>,
                          ref _scrambledChangedCallback);
                        break;
                    case EventType.MediaPlayerESAdded:
                        Detach(eventType,
                          ref _eSAddedRegistrationCount,
                          () => _mediaPlayerESAdded -= eventHandler as EventHandler<MediaPlayerESAddedEventArgs>,
                          ref _eSAddedCallback);
                        break;
                    case EventType.MediaPlayerESDeleted:
                        Detach(eventType,
                          ref _eSDeletedRegistrationCount,
                          () => _mediaPlayerESDeleted -= eventHandler as EventHandler<MediaPlayerESDeletedEventArgs>,
                          ref _eSDeletedCallback);
                        break;
                    case EventType.MediaPlayerESSelected:
                        Detach(eventType,
                          ref _eSSelectedRegistrationCount,
                          () => _mediaPlayerESSelected -= eventHandler as EventHandler<MediaPlayerESSelectedEventArgs>,
                          ref _eSSelectedCallback);
                        break;
                    case EventType.MediaPlayerAudioDevice:
                        Detach(eventType,
                          ref _audioDeviceRegistrationCount,
                          () => _mediaPlayerAudioDevice -= eventHandler as EventHandler<MediaPlayerAudioDeviceEventArgs>,
                          ref _audioDeviceCallback);
                        break;
                    case EventType.MediaPlayerCorked:
                        Detach(eventType,
                          ref _corkedRegistrationCount,
                          () => _mediaPlayerCorked -= eventHandler as EventHandler<EventArgs>,
                          ref _corkedCallback);
                        break;
                    case EventType.MediaPlayerUncorked:
                        Detach(eventType,
                         ref _uncorkedRegistrationCount,
                         () => _mediaPlayerUncorked -= eventHandler as EventHandler<EventArgs>,
                         ref _uncorkedCallback);
                        break;
                    case EventType.MediaPlayerMuted:
                        Detach(eventType,
                         ref _mutedRegistrationCount,
                         () => _mediaPlayerMuted -= eventHandler as EventHandler<EventArgs>,
                         ref _mutedCallback);
                        break;
                    case EventType.MediaPlayerUnmuted:
                        Detach(eventType,
                         ref _unmutedRegistrationCount,
                         () => _mediaPlayerUnmuted -= eventHandler as EventHandler<EventArgs>,
                         ref _unmutedCallback);
                        break;
                    case EventType.MediaPlayerAudioVolume:
                        Detach(eventType,
                         ref _volumeChangedRegistrationCount,
                         () => _mediaPlayerVolumeChanged -= eventHandler as EventHandler<MediaPlayerVolumeChangedEventArgs>,
                         ref _volumeChangedCallback);
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