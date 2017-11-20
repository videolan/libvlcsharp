using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using VideoLAN.LibVLC;

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
        EventHandler<EventArgs> _mediaPlayerAudioVolume; // vlc 2.2

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

        //TODO: Add libvlc API version check > 3.0
        void OnChapterChanged(IntPtr ptr)
        {
            _mediaPlayerChapterChanged?.Invoke(this,
                new MediaPlayerChapterChangedEventArgs(RetrieveEvent(ptr).Union.MediaPlayerChapterChanged.NewChapter));
        }
    }

    public class MediaListEventManager : EventManager
    {
        public MediaListEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaListPlayerEventManager : EventManager
    {
        public MediaListPlayerEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class MediaDiscovererEventManager : EventManager
    {
        public MediaDiscovererEventManager(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class VLMEventManager : EventManager
    {
        public VLMEventManager(IntPtr ptr) : base(ptr)
        {
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
