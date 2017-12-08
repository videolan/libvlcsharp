using System;
using System.Runtime.InteropServices;
using System.Security;
using VideoLAN.LibVLC.Events;

namespace VideoLAN.LibVLC
{
    public abstract class EventManager
    {
        public struct Internal
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libvlc_event_attach")]
            internal static extern int LibVLCEventAttach(IntPtr eventManager, EventType eventType, EventCallback eventCallback,
                IntPtr userData);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libvlc_event_detach")]
            internal static extern void LibVLCEventDetach(IntPtr eventManager, EventType eventType, EventCallback eventCallback,
                IntPtr userData);
        }

        public IntPtr NativeReference;

        protected EventManager(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new NullReferenceException(nameof(ptr));

            NativeReference = ptr;
        }

        protected void AttachEvent(EventType eventType, EventCallback eventCallback)
        {
            if(Internal.LibVLCEventAttach(NativeReference, eventType, eventCallback, IntPtr.Zero) != 0)
                throw new VLCException($"Could not attach event {eventType}");
        }

        protected void DetachEvent(EventType eventType, EventCallback eventCallback)
        {
            Internal.LibVLCEventDetach(NativeReference, eventType, eventCallback, IntPtr.Zero);
        }

        protected LibVLCEvent RetrieveEvent(IntPtr eventPtr)
        {
            return Marshal.PtrToStructure<LibVLCEvent>(eventPtr);
        }
    }

    public class MediaEventManager : EventManager
    {
        readonly object _lock = new object();

        EventHandler<MediaMetaChangedEventArgs> _mediaMetaChanged;
        EventHandler<MediaParsedChangedEventArgs> _mediaParsedChanged;
        EventHandler<MediaSubItemAddedEventArgs> _mediaSubItemAdded;
        EventHandler<MediaDurationChangedEventArgs> _mediaDurationChanged;
        EventHandler<MediaFreedEventArgs> _mediaFreed;
        EventHandler<MediaStateChangedEventArgs> _mediaStateChanged;
        EventHandler<MediaSubItemTreeAddedEventArgs> _mediaSubItemTreeAdded;

        public MediaEventManager(IntPtr ptr) : base(ptr)
        {
        }

        public event EventHandler<MediaMetaChangedEventArgs> MetaChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaMetaChanged += value;
                    AttachEvent(EventType.MediaMetaChanged, OnMetaChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaMetaChanged -= value;
                    DetachEvent(EventType.MediaMetaChanged, OnMetaChanged);
                }
            }
        }
        
        public event EventHandler<MediaParsedChangedEventArgs> ParsedChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaParsedChanged += value;
                    AttachEvent(EventType.MediaParsedChanged, OnParsedChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaParsedChanged -= value;
                    DetachEvent(EventType.MediaParsedChanged, OnParsedChanged);
                }
            }
        }

        public event EventHandler<MediaSubItemAddedEventArgs> SubItemAdded
        {
            add
            {
                lock (_lock)
                {
                    _mediaSubItemAdded += value;
                    AttachEvent(EventType.MediaSubItemAdded, OnSubItemAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaSubItemAdded -= value;
                    DetachEvent(EventType.MediaSubItemAdded, OnSubItemAdded);
                }
            }
        }

        public event EventHandler<MediaDurationChangedEventArgs> DurationChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaDurationChanged += value;
                    AttachEvent(EventType.MediaDurationChanged, OnDurationChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaDurationChanged -= value;
                    DetachEvent(EventType.MediaDurationChanged, OnDurationChanged);
                }
            }
        }

        public event EventHandler<MediaFreedEventArgs> MediaFreed
        {
            add
            {
                lock (_lock)
                {
                    _mediaFreed += value;
                    AttachEvent(EventType.MediaFreed, OnMediaFreed);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaFreed -= value;
                    DetachEvent(EventType.MediaFreed, OnMediaFreed);
                }
            }
        }

        public event EventHandler<MediaStateChangedEventArgs> StateChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaStateChanged += value;
                    AttachEvent(EventType.MediaStateChanged, OnMediaStateChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaStateChanged -= value;
                    DetachEvent(EventType.MediaStateChanged, OnMediaStateChanged);
                }
            }
        }

        public event EventHandler<MediaSubItemTreeAddedEventArgs> SubItemTreeAdded
        {
            add
            {
                lock (_lock)
                {
                    _mediaSubItemTreeAdded += value;
                    AttachEvent(EventType.MediaSubItemTreeAdded, OnSubItemTreeAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaSubItemTreeAdded -= value;
                    DetachEvent(EventType.MediaSubItemTreeAdded, OnSubItemTreeAdded);
                }
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
    }

    public class MediaPlayerEventManager : EventManager
    {
        readonly object _lock = new object();

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

        public MediaPlayerEventManager(IntPtr ptr) : base(ptr)
        {
        }

        public event EventHandler<MediaPlayerMediaChangedEventArgs> MediaChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerMediaChanged += value;
                    AttachEvent(EventType.MediaPlayerMediaChanged, OnMediaChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerMediaChanged -= value;
                    DetachEvent(EventType.MediaPlayerMediaChanged, OnMediaChanged);
                }
            }
        }

        public event EventHandler<EventArgs> NothingSpecial
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerNothingSpecial += value;
                    AttachEvent(EventType.MediaPlayerNothingSpecial, OnNothingSpecial);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerNothingSpecial -= value;
                    DetachEvent(EventType.MediaPlayerNothingSpecial, OnNothingSpecial);
                }
            }
        }

        public event EventHandler<EventArgs> Opening
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerOpening += value;
                    AttachEvent(EventType.MediaPlayerOpening, OnOpening);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerOpening -= value;
                    DetachEvent(EventType.MediaPlayerOpening, OnOpening);
                }
            }
        }

        public event EventHandler<MediaPlayerBufferingEventArgs> Buffering
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerBuffering += value;
                    AttachEvent(EventType.MediaPlayerBuffering, OnBuffering);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerBuffering -= value;
                    DetachEvent(EventType.MediaPlayerBuffering, OnBuffering);
                }
            }
        }

        public event EventHandler<EventArgs> Playing
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerPlaying += value;
                    AttachEvent(EventType.MediaPlayerPlaying, OnPlaying);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerPlaying -= value;
                    DetachEvent(EventType.MediaPlayerPlaying, OnPlaying);
                }
            }
        }

        public event EventHandler<EventArgs> Paused
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerPaused += value;
                    AttachEvent(EventType.MediaPlayerPaused, OnPaused);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerPaused -= value;
                    DetachEvent(EventType.MediaPlayerPaused, OnPaused);
                }
            }
        }

        public event EventHandler<EventArgs> Stopped
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerStopped += value;
                    AttachEvent(EventType.MediaPlayerStopped, OnStopped);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerStopped -= value;
                    DetachEvent(EventType.MediaPlayerStopped, OnStopped);
                }
            }
        }

        public event EventHandler<EventArgs> Forward
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerForward += value;
                    AttachEvent(EventType.MediaPlayerForward, OnForward);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerForward -= value;
                    DetachEvent(EventType.MediaPlayerForward, OnForward);
                }
            }
        }

        public event EventHandler<EventArgs> Backward
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerBackward += value;
                    AttachEvent(EventType.MediaPlayerBackward, OnBackward);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerBackward -= value;
                    DetachEvent(EventType.MediaPlayerBackward, OnBackward);
                }
            }
        }

        public event EventHandler<EventArgs> EndReached
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerEndReached += value;
                    AttachEvent(EventType.MediaPlayerEndReached, OnEndReached);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerEndReached -= value;
                    DetachEvent(EventType.MediaPlayerEndReached, OnEndReached);
                }
            }
        }

        public event EventHandler<EventArgs> EncounteredError
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerEncounteredError += value;
                    AttachEvent(EventType.MediaPlayerEncounteredError, OnEncounteredError);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerEncounteredError -= value;
                    DetachEvent(EventType.MediaPlayerEncounteredError, OnEncounteredError);
                }
            }
        }

        public event EventHandler<MediaPlayerTimeChangedEventArgs> TimeChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerTimeChanged += value;
                    AttachEvent(EventType.MediaPlayerTimeChanged, OnTimeChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerTimeChanged -= value;
                    DetachEvent(EventType.MediaPlayerTimeChanged, OnTimeChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerPositionChangedEventArgs> PositionChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerPositionChanged += value;
                    AttachEvent(EventType.MediaPlayerPositionChanged, OnPositionChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerPositionChanged -= value;
                    DetachEvent(EventType.MediaPlayerPositionChanged, OnPositionChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerSeekableChangedEventArgs> SeekableChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerSeekableChanged += value;
                    AttachEvent(EventType.MediaPlayerSeekableChanged, OnSeekableChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerSeekableChanged -= value;
                    DetachEvent(EventType.MediaPlayerSeekableChanged, OnSeekableChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerPausableChangedEventArgs> PausableChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerPausableChanged += value;
                    AttachEvent(EventType.MediaPlayerPausableChanged, OnPausableChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerPausableChanged -= value;
                    DetachEvent(EventType.MediaPlayerPausableChanged, OnPausableChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerTitleChangedEventArgs> TitleChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerTitleChanged += value;
                    AttachEvent(EventType.MediaPlayerTitleChanged, OnTitleChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerTitleChanged -= value;
                    DetachEvent(EventType.MediaPlayerTitleChanged, OnTitleChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerChapterChangedEventArgs> ChapterChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerChapterChanged += value;
                    AttachEvent(EventType.MediaPlayerChapterChanged, OnChapterChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerChapterChanged -= value;
                    DetachEvent(EventType.MediaPlayerChapterChanged, OnChapterChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerSnapshotTakenEventArgs> SnapshotTaken
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerSnapshotTaken += value;
                    AttachEvent(EventType.MediaPlayerSnapshotTaken, OnSnapshotTaken);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerSnapshotTaken -= value;
                    DetachEvent(EventType.MediaPlayerSnapshotTaken, OnSnapshotTaken);
                }
            }
        }

        public event EventHandler<MediaPlayerLengthChangedEventArgs> LengthChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerLengthChanged += value;
                    AttachEvent(EventType.MediaPlayerLengthChanged, OnLengthChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerLengthChanged -= value;
                    DetachEvent(EventType.MediaPlayerLengthChanged, OnLengthChanged);
                }
            }
        }

        public event EventHandler<MediaPlayerVoutEventArgs> Vout
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerVout += value;
                    AttachEvent(EventType.MediaPlayerVout, OnVout);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerVout -= value;
                    DetachEvent(EventType.MediaPlayerVout, OnVout);
                }
            }
        }

        public event EventHandler<MediaPlayerScrambledChangedEventArgs> ScrambledChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerScrambledChanged += value;
                    AttachEvent(EventType.MediaPlayerScrambledChanged, OnScrambledChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerScrambledChanged -= value;
                    DetachEvent(EventType.MediaPlayerScrambledChanged, OnScrambledChanged);
                }
            }
        }

        // v3
        public event EventHandler<MediaPlayerESAddedEventArgs> ESAdded
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerESAdded += value;
                    AttachEvent(EventType.MediaPlayerESAdded, OnESAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerESAdded -= value;
                    DetachEvent(EventType.MediaPlayerESAdded, OnESAdded);
                }
            }
        }

        // v3
        public event EventHandler<MediaPlayerESDeletedEventArgs> ESDeleted
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerESDeleted += value;
                    AttachEvent(EventType.MediaPlayerESDeleted, OnESDeleted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerESDeleted -= value;
                    DetachEvent(EventType.MediaPlayerESDeleted, OnESDeleted);
                }
            }
        }

        // v3
        public event EventHandler<MediaPlayerESSelectedEventArgs> ESSelected
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerESSelected += value;
                    AttachEvent(EventType.MediaPlayerESSelected, OnESSelected);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerESSelected -= value;
                    DetachEvent(EventType.MediaPlayerESSelected, OnESSelected);
                }
            }
        }

        // v3
        public event EventHandler<MediaPlayerAudioDeviceEventArgs> AudioDevice
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerAudioDevice += value;
                    AttachEvent(EventType.MediaPlayerAudioDevice, OnAudioDevice);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerAudioDevice -= value;
                    DetachEvent(EventType.MediaPlayerAudioDevice, OnAudioDevice);
                }
            }
        }

        // v2.2
        public event EventHandler<EventArgs> Corked
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerCorked += value;
                    AttachEvent(EventType.MediaPlayerCorked, OnCorked);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerCorked -= value;
                    DetachEvent(EventType.MediaPlayerCorked, OnCorked);
                }
            }
        }

        // v2.2
        public event EventHandler<EventArgs> Uncorked
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerUncorked += value;
                    AttachEvent(EventType.MediaPlayerUncorked, OnUncorked);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerUncorked -= value;
                    DetachEvent(EventType.MediaPlayerUncorked, OnUncorked);
                }
            }
        }

        // v2.2
        public event EventHandler<EventArgs> Muted
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerMuted += value;
                    AttachEvent(EventType.MediaPlayerMuted, OnMuted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerMuted -= value;
                    DetachEvent(EventType.MediaPlayerMuted, OnMuted);
                }
            }
        }

        // v2.2
        public event EventHandler<EventArgs> Unmuted
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerUnmuted += value;
                    AttachEvent(EventType.MediaPlayerUnmuted, OnUnmuted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerUnmuted -= value;
                    DetachEvent(EventType.MediaPlayerUnmuted, OnUnmuted);
                }
            }
        }

        // v2.2
        public event EventHandler<MediaPlayerVolumeChangedEventArgs> VolumeChanged
        {
            add
            {
                lock (_lock)
                {
                    _mediaPlayerVolumeChanged += value;
                    AttachEvent(EventType.MediaPlayerAudioVolume, OnVolumeChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaPlayerVolumeChanged -= value;
                    DetachEvent(EventType.MediaPlayerAudioVolume, OnVolumeChanged);
                }
            }
        }

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
            var deviceName = (string) Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(deviceNamePtr);
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
    }

    public class MediaListEventManager : EventManager
    {
        readonly object _lock = new object();

        EventHandler<MediaListItemAddedEventArgs> _mediaListItemAdded;
        EventHandler<MediaListWillAddItemEventArgs> _mediaListWillAddItem;
        EventHandler<MediaListItemDeletedEventArgs> _mediaListItemDeleted;
        EventHandler<MediaListWillDeleteItemEventArgs> _mediaListWillDeleteItem;
        EventHandler<EventArgs> _mediaListEndReached;

        public MediaListEventManager(IntPtr ptr) : base(ptr)
        {
        }

        public event EventHandler<MediaListItemAddedEventArgs> ItemAdded
        {
            add
            {
                lock (_lock)
                {
                    _mediaListItemAdded += value;
                    AttachEvent(EventType.MediaListItemAdded, OnItemAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListItemAdded -= value;
                    DetachEvent(EventType.MediaListItemAdded, OnItemAdded);
                }
            }
        }
        public event EventHandler<MediaListWillAddItemEventArgs> WillAddItem
        {
            add
            {
                lock (_lock)
                {
                    _mediaListWillAddItem += value;
                    AttachEvent(EventType.MediaListWillAddItem, OnWillAddItem);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListWillAddItem -= value;
                    DetachEvent(EventType.MediaListWillAddItem, OnWillAddItem);
                }
            }
        }

        public event EventHandler<MediaListItemDeletedEventArgs> ItemDeleted
        {
            add
            {
                lock (_lock)
                {
                    _mediaListItemDeleted += value;
                    AttachEvent(EventType.MediaListItemDeleted, OnItemDeleted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListItemDeleted -= value;
                    DetachEvent(EventType.MediaListItemDeleted, OnItemDeleted);
                }
            }
        }

        public event EventHandler<MediaListWillDeleteItemEventArgs> WillDeleteItem
        {
            add
            {
                lock (_lock)
                {
                    _mediaListWillDeleteItem += value;
                    AttachEvent(EventType.MediaListWillDeleteItem, OnWillDeleteItem);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListWillDeleteItem -= value;
                    DetachEvent(EventType.MediaListWillDeleteItem, OnWillDeleteItem);
                }
            }
        }

        // v3
        public event EventHandler<EventArgs> EndReached
        {
            add
            {
                lock (_lock)
                {
                    _mediaListEndReached += value;
                    AttachEvent(EventType.MediaPlayerEndReached, OnEndReached);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListEndReached -= value;
                    DetachEvent(EventType.MediaPlayerEndReached, OnEndReached);
                }
            }
        }

        void OnItemAdded(IntPtr ptr)
        {
            var itemAdded = RetrieveEvent(ptr).Union.MediaListItemAdded;

            _mediaListItemAdded?.Invoke(this,
                new MediaListItemAddedEventArgs(new Media(itemAdded.MediaInstance), itemAdded.Index ));
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
    }

    public class MediaListPlayerEventManager : EventManager
    {
        readonly object _lock = new object();

        EventHandler<EventArgs> _mediaListPlayerPlayed;
        EventHandler<MediaListPlayerNextItemSetEventArgs> _mediaListPlayerNextItemSet;
        EventHandler<EventArgs> _mediaListPlayerStopped;

        public MediaListPlayerEventManager(IntPtr ptr) : base(ptr)
        {
        }

        public event EventHandler<EventArgs> Played
        {
            add
            {
                lock (_lock)
                {
                    _mediaListPlayerPlayed += value;
                    AttachEvent(EventType.MediaListPlayerPlayed, OnPlayed);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListPlayerPlayed -= value;
                    DetachEvent(EventType.MediaListPlayerPlayed, OnPlayed);
                }
            }
        }

        public event EventHandler<MediaListPlayerNextItemSetEventArgs> NextItemSet
        {
            add
            {
                lock (_lock)
                {
                    _mediaListPlayerNextItemSet += value;
                    AttachEvent(EventType.MediaListPlayerNextItemSet, OnNextItemSet);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListPlayerNextItemSet -= value;
                    DetachEvent(EventType.MediaListPlayerNextItemSet, OnNextItemSet);
                }
            }
        }

        public event EventHandler<EventArgs> Stopped
        {
            add
            {
                lock (_lock)
                {
                    _mediaListPlayerStopped += value;
                    AttachEvent(EventType.MediaListPlayerStopped, OnStopped);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaListPlayerStopped -= value;
                    DetachEvent(EventType.MediaListPlayerStopped, OnStopped);
                }
            }
        }

        void OnPlayed(IntPtr ptr)
        {
            _mediaListPlayerPlayed?.Invoke(this, EventArgs.Empty);
        }

        void OnNextItemSet(IntPtr ptr)
        {
            var mediaPtr = RetrieveEvent(ptr).Union.MediaListPlayerNextItemSet.MediaInstance;

            _mediaListPlayerNextItemSet?.Invoke(this, new MediaListPlayerNextItemSetEventArgs(new Media(mediaPtr)));
        }

        void OnStopped(IntPtr ptr)
        {
            _mediaListPlayerStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MediaDiscovererEventManager : EventManager
    {
        readonly object _lock = new object();

        EventHandler<EventArgs> _mediaDiscovererStarted;
        EventHandler<EventArgs> _mediaDiscovererStopped;

        public MediaDiscovererEventManager(IntPtr ptr) : base(ptr)
        {
        }

        // v3
        public event EventHandler<EventArgs> Started
        {
            add
            {
                lock (_lock)
                {
                    _mediaDiscovererStarted += value;
                    AttachEvent(EventType.MediaDiscovererStarted, OnStarted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaDiscovererStarted -= value;
                    DetachEvent(EventType.MediaDiscovererStarted, OnStarted);
                }
            }
        }

        // v3
        public event EventHandler<EventArgs> Stopped
        {
            add
            {
                lock (_lock)
                {
                    _mediaDiscovererStopped += value;
                    AttachEvent(EventType.MediaDiscovererStopped, OnStopped);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _mediaDiscovererStopped -= value;
                    DetachEvent(EventType.MediaDiscovererStopped, OnStopped);
                }
            }
        }

        void OnStarted(IntPtr ptr)
        {
            _mediaDiscovererStarted?.Invoke(this, EventArgs.Empty);
        }

        void OnStopped(IntPtr ptr)
        {
            _mediaDiscovererStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    public class VLMEventManager : EventManager
    {
        readonly object _lock = new object();

        EventHandler<VLMMediaEventArgs> _vlmMediaAdded;
        EventHandler<VLMMediaEventArgs> _vlmMediaRemoved;
        EventHandler<VLMMediaEventArgs> _vlmMediaChanged;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStarted;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStopped;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStatusInit;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStatusOpening;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStatusPlaying;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStatusPause;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStatusEnd;
        EventHandler<VLMMediaEventArgs> _vlmMediaInstanceStatusError;

        public VLMEventManager(IntPtr ptr) : base(ptr)
        {
        }

        public event EventHandler<VLMMediaEventArgs> MediaAdded
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaAdded += value;
                    AttachEvent(EventType.VlmMediaAdded, OnMediaAdded);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaAdded -= value;
                    DetachEvent(EventType.VlmMediaAdded, OnMediaAdded);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaRemoved
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaRemoved += value;
                    AttachEvent(EventType.VlmMediaRemoved, OnMediaRemoved);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaRemoved -= value;
                    DetachEvent(EventType.VlmMediaRemoved, OnMediaRemoved);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaChanged
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaChanged += value;
                    AttachEvent(EventType.VlmMediaChanged, OnMediaChanged);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaChanged -= value;
                    DetachEvent(EventType.VlmMediaChanged, OnMediaChanged);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStarted
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStarted += value;
                    AttachEvent(EventType.VlmMediaInstanceStarted, OnMediaInstanceStarted);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStarted -= value;
                    DetachEvent(EventType.VlmMediaInstanceStarted, OnMediaInstanceStarted);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStopped
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStopped += value;
                    AttachEvent(EventType.VlmMediaInstanceStopped, OnMediaInstanceStopped);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStopped -= value;
                    DetachEvent(EventType.VlmMediaInstanceStopped, OnMediaInstanceStopped);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStatusInit
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusInit += value;
                    AttachEvent(EventType.VlmMediaInstanceStatusInit, OnMediaInstanceStatusInit);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusInit -= value;
                    DetachEvent(EventType.VlmMediaInstanceStatusInit, OnMediaInstanceStatusInit);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStatusOpening
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusOpening += value;
                    AttachEvent(EventType.VlmMediaInstanceStatusOpening, OnMediaInstanceStatusOpening);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusOpening -= value;
                    DetachEvent(EventType.VlmMediaInstanceStatusOpening, OnMediaInstanceStatusOpening);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStatusPlaying
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusPlaying += value;
                    AttachEvent(EventType.VlmMediaInstanceStatusPlaying, OnMediaInstanceStatusPlaying);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusPlaying -= value;
                    DetachEvent(EventType.VlmMediaInstanceStatusPlaying, OnMediaInstanceStatusPlaying);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStatusPause
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusPause += value;
                    AttachEvent(EventType.VlmMediaInstanceStatusPause, OnMediaInstanceStatusPause);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusPause -= value;
                    DetachEvent(EventType.VlmMediaInstanceStatusPause, OnMediaInstanceStatusPause);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStatusEnd
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusEnd += value;
                    AttachEvent(EventType.VlmMediaInstanceStatusEnd, OnMediaInstanceStatusEnd);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusEnd -= value;
                    DetachEvent(EventType.VlmMediaInstanceStatusEnd, OnMediaInstanceStatusEnd);
                }
            }
        }

        public event EventHandler<VLMMediaEventArgs> MediaInstanceStatusError
        {
            add
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusError += value;
                    AttachEvent(EventType.VlmMediaInstanceStatusError, OnMediaInstanceStatusError);
                }
            }
            remove
            {
                lock (_lock)
                {
                    _vlmMediaInstanceStatusError -= value;
                    DetachEvent(EventType.VlmMediaInstanceStatusError, OnMediaInstanceStatusError);
                }
            }
        }

        void OnMediaAdded(IntPtr ptr)
        {
            _vlmMediaAdded?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr)));
        }

        void OnMediaRemoved(IntPtr ptr)
        {
            _vlmMediaRemoved?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr)));
        }

        void OnMediaChanged(IntPtr ptr)
        {
            _vlmMediaChanged?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr)));
        }

        void OnMediaInstanceStarted(IntPtr ptr)
        {
            _vlmMediaInstanceStarted?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStopped(IntPtr ptr)
        {
            _vlmMediaInstanceStopped?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStatusInit(IntPtr ptr)
        {
            _vlmMediaInstanceStatusInit?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStatusOpening(IntPtr ptr)
        {
            _vlmMediaInstanceStatusOpening?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStatusPlaying(IntPtr ptr)
        {
            _vlmMediaInstanceStatusPlaying?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStatusPause(IntPtr ptr)
        {
            _vlmMediaInstanceStatusPause?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStatusEnd(IntPtr ptr)
        {
            _vlmMediaInstanceStatusEnd?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        void OnMediaInstanceStatusError(IntPtr ptr)
        {
            _vlmMediaInstanceStatusError?.Invoke(this, new VLMMediaEventArgs(MediaName(ptr), InstanceName(ptr)));
        }

        string MediaName(IntPtr ptr)
        {
            var mediaNamePtr = RetrieveEvent(ptr).Union.VlmMediaEvent.MediaName;
            return (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(mediaNamePtr);
        }

        string InstanceName(IntPtr ptr)
        {
            var instanceNamePtr = RetrieveEvent(ptr).Union.VlmMediaEvent.InstanceName;
            return (string)Utf8StringMarshaler.GetInstance().MarshalNativeToManaged(instanceNamePtr);
        }
    }

    public class VLCException : Exception
    {
        public readonly string Reason;

        public VLCException(string reason = "")
        {
            Reason = reason;
        }
    }

    [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventCallback(IntPtr args);
}
