using System;

namespace VideoLAN.LibVLC.Events
{
    /// <summary>LibVLCEvent types</summary>
    public enum EventType
    {
        MediaMetaChanged = 0,
        MediaSubItemAdded = 1,
        MediaDurationChanged = 2,
        MediaParsedChanged = 3,
        MediaFreed = 4,
        MediaStateChanged = 5,
        MediaSubItemTreeAdded = 6,
        MediaPlayerMediaChanged = 256,
        MediaPlayerNothingSpecial = 257,
        MediaPlayerOpening = 258,
        MediaPlayerBuffering = 259,
        MediaPlayerPlaying = 260,
        MediaPlayerPaused = 261,
        MediaPlayerStopped = 262,
        MediaPlayerForward = 263,
        MediaPlayerBackward = 264,
        MediaPlayerEndReached = 265,
        MediaPlayerEncounteredError = 266,
        MediaPlayerTimeChanged = 267,
        MediaPlayerPositionChanged = 268,
        MediaPlayerSeekableChanged = 269,
        MediaPlayerPausableChanged = 270,
        MediaPlayerTitleChanged = 271,
        MediaPlayerSnapshotTaken = 272,
        MediaPlayerLengthChanged = 273,
        MediaPlayerVout = 274,
        MediaPlayerScrambledChanged = 275,
        MediaPlayerESAdded = 276,
        MediaPlayerESDeleted = 277,
        MediaPlayerESSelected = 278,
        MediaPlayerCorked = 279,
        MediaPlayerUncorked = 280,
        MediaPlayerMuted = 281,
        MediaPlayerUnmuted = 282,
        MediaPlayerAudioVolume = 283,
        MediaPlayerAudioDevice = 284,
        MediaPlayerChapterChanged = 285,
        MediaListItemAdded = 512,
        MediaListWillAddItem = 513,
        MediaListItemDeleted = 514,
        MediaListWillDeleteItem = 515,
        MediaListEndReached = 516,
        MediaListViewItemAdded = 768,
        MediaListViewWillAddItem = 769,
        MediaListViewItemDeleted = 770,
        MediaListViewWillDeleteItem = 771,
        MediaListPlayerPlayed = 1024,
        MediaListPlayerNextItemSet = 1025,
        MediaListPlayerStopped = 1026,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_start()</para>
        /// </remarks>
        MediaDiscovererStarted = 1280,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        MediaDiscovererStopped = 1281,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        RendererDiscovererItemAdded = 1282,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        RendererDiscovererItemDeleted = 1283,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaAdded = 1536,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaRemoved = 1537,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaChanged = 1538,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStarted = 1539,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStopped = 1540,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStatusInit = 1541,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStatusOpening = 1542,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStatusPlaying = 1543,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStatusPause = 1544,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStatusEnd = 1545,

        /// <remarks>
        /// <para>Useless event, it will be triggered only when calling</para>
        /// <para>libvlc_media_discoverer_stop()</para>
        /// </remarks>
        VlmMediaInstanceStatusError = 1546
    }

    /// <summary>Renderer item</summary>
    /// <remarks>
    /// <para>This struct is passed by a</para>
    /// <para>or deleted.</para>
    /// <para>An item is valid until the</para>
    /// <para>is called with the same pointer.</para>
    /// <para>libvlc_renderer_discoverer_event_manager()</para>
    /// </remarks>
    /// <summary>A LibVLC event</summary>
    public struct LibVLCEvent
    {
        public EventType Type;

        public IntPtr Sender;

        public EventUnion Union;

        public struct EventUnion
        {
            // media
            public MediaMetaChanged MediaMetaChanged;
            public MediaSubItemAdded MediaSubItemAdded;
            public MediaDurationChanged MediaDurationChanged;
            public MediaParsedChanged MediaParsedChanged;
            public MediaFreed MediaFreed;
            public MediaStateChanged MediaStateChanged;
            public MediaSubItemTreeAdded MediaSubItemTreeAdded;

            // mediaplayer
            public MediaPlayerBuffering MediaPlayerBuffering;
            public MediaPlayerChapterChanged MediaPlayerChapterChanged;
            public MediaPlayerPositionChanged MediaPlayerPositionChanged;
            public MediaPlayerTimeChanged MediaPlayerTimeChanged;
            public MediaPlayerTitleChanged MediaPlayerTitleChanged;
            public MediaPlayerSeekableChanged MediaPlayerSeekableChanged;
            public MediaPlayerPausableChanged MediaPlayerPausableChanged;
            public MediaPlayerScrambledChanged MediaPlayerScrambledChanged;
            public MediaPlayerVoutChanged MediaPlayerVoutChanged;
            public MediaPlayerSnapshotTaken MediaPlayerSnapshotTaken;
            public MediaPlayerLengthChanged MediaPlayerLengthChanged;
            public MediaPlayerMediaChanged MediaPlayerMediaChanged;
            public EsChanged EsChanged;
            public VolumeChanged MediaPlayerVolumeChanged;
            public AudioDeviceChanged AudioDeviceChanged;

            // medialist
            public MediaListItemAdded MediaListItemAdded;
            public MediaListWillAddItem MediaListWillAddItem;
            public MediaListItemDeleted MediaListItemDeleted;
            public MediaListWillDeleteItem MediaListWillDeleteItem;
            public MediaListPlayerNextItemSet MediaListPlayerNextItemSet;
           
            // vlm
            public VlmMediaEvent VlmMediaEvent;

            // renderer
            public RendererDiscovererItemAdded RendererDiscovererItemAdded;
            public RendererDiscovererItemDeleted RendererDiscovererItemDeleted;
        }

        #region Media

        public struct MediaMetaChanged
        {
            public Media.MetadataType MetaType;
        }

        public struct MediaSubItemAdded
        {
            public IntPtr NewChild;
        }

        public struct MediaDurationChanged
        {
            public long NewDuration;
        }

        public struct MediaParsedChanged
        {
            public Media.MediaParsedStatus NewStatus;
        }

        public struct MediaFreed
        {
            public IntPtr MediaInstance;
        }
        
        public struct MediaStateChanged
        {
            public VLCState NewState;
        }
        
        public struct MediaSubItemTreeAdded
        {
            public IntPtr MediaInstance;
        }

        #endregion

        #region MediaPlayer 

        public struct MediaPlayerBuffering
        {
            public float NewCache;
        }

        public struct MediaPlayerChapterChanged
        {
            public int NewChapter;
        }

        public struct MediaPlayerPositionChanged
        {
            public float NewPosition;
        }

        public struct MediaPlayerTimeChanged
        {
            public long NewTime;
        }

        public struct MediaPlayerTitleChanged
        {
            public int NewTitle;
        }

        public struct MediaPlayerSeekableChanged
        {
            public int NewSeekable;
        }

        public struct MediaPlayerPausableChanged
        {
            public int NewPausable;
        }

        public struct MediaPlayerScrambledChanged
        {
            public int NewScrambled;
        }

        public struct MediaPlayerVoutChanged
        {
            public int NewCount;
        }

        public struct MediaPlayerSnapshotTaken
        {
            public IntPtr Filename;
        }

        public struct MediaPlayerLengthChanged
        {
            public long NewLength;
        }

        public struct EsChanged
        {
            public TrackType Type;
            public int Id;
        }

        public struct AudioDeviceChanged
        {
            public IntPtr Device;
        }

        public struct MediaPlayerMediaChanged
        {
            public IntPtr NewMedia;
        }

        public struct VolumeChanged
        {
            public float Volume;
        }

        #endregion

        #region MediaList

        public struct MediaListItemAdded
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        public struct MediaListWillAddItem
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        public struct MediaListItemDeleted
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        public struct MediaListWillDeleteItem
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        public struct MediaListPlayerNextItemSet
        {
            public IntPtr MediaInstance;
        }

        #endregion MediaList

        public struct VlmMediaEvent
        {
            public IntPtr MediaName;
            public IntPtr InstanceName;
        }
        
        public struct RendererDiscovererItemAdded
        {
            public IntPtr Item;
        }

        public struct RendererDiscovererItemDeleted
        {
            public IntPtr Item;
        }
    }

    #region Media events

    public class MediaMetaChangedEventArgs : EventArgs
    {
        public readonly Media.MetadataType MetadataType;

        public MediaMetaChangedEventArgs(Media.MetadataType metadataType)
        {
            MetadataType = metadataType;
        }
    }

    public class MediaParsedChangedEventArgs : EventArgs
    {
        public readonly Media.MediaParsedStatus ParsedStatus;

        public MediaParsedChangedEventArgs(Media.MediaParsedStatus parsedStatus)
        {
            ParsedStatus = parsedStatus;
        }
    }

    public class MediaSubItemAddedEventArgs : EventArgs
    {
        public readonly Media SubItem;

        public MediaSubItemAddedEventArgs(IntPtr mediaPtr)
        {
            SubItem = new Media(mediaPtr);
        }
    }

    public class MediaDurationChangedEventArgs : EventArgs
    {
        public readonly long Duration;

        public MediaDurationChangedEventArgs(long duration)
        {
            Duration = duration;
        }
    }

    public class MediaFreedEventArgs : EventArgs
    {
        public readonly Media Media;

        public MediaFreedEventArgs(IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
        }
    }

    public class MediaStateChangedEventArgs : EventArgs
    {
        public readonly VLCState State;

        public MediaStateChangedEventArgs(VLCState state)
        {
            State = state;
        }
    }

    public class MediaSubItemTreeAddedEventArgs : EventArgs
    {
        public readonly Media SubItem;

        public MediaSubItemTreeAddedEventArgs(IntPtr subItemPtr)
        {
            SubItem = new Media(subItemPtr);
        }
    }

    #endregion

    #region MediaPlayer events

    public class MediaPlayerMediaChangedEventArgs : EventArgs
    {
        public readonly Media Media;

        public MediaPlayerMediaChangedEventArgs(IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
        }
    }

    public class MediaPlayerBufferingEventArgs : EventArgs
    {
        public readonly float Cache;

        public MediaPlayerBufferingEventArgs(float cache)
        {
            Cache = cache;
        }
    }

    public class MediaPlayerTimeChangedEventArgs : EventArgs
    {
        public readonly long Time;

        public MediaPlayerTimeChangedEventArgs(long time)
        {
            Time = time;
        }
    }

    public class MediaPlayerPositionChangedEventArgs : EventArgs
    {
        public readonly float Position;

        public MediaPlayerPositionChangedEventArgs(float position)
        {
            Position = position;
        }
    }

    public class MediaPlayerSeekableChangedEventArgs : EventArgs
    {
        public readonly int Seekable;

        public MediaPlayerSeekableChangedEventArgs(int seekable)
        {
            Seekable = seekable;
        }
    }

    public class MediaPlayerPausableChangedEventArgs : EventArgs
    {
        public readonly int Pausable;

        public MediaPlayerPausableChangedEventArgs(int pausable)
        {
            Pausable = pausable;
        }
    }

    public class MediaPlayerTitleChangedEventArgs : EventArgs
    {
        public readonly int Title;

        public MediaPlayerTitleChangedEventArgs(int title)
        {
            Title = title;
        }
    }

    public class MediaPlayerChapterChangedEventArgs : EventArgs
    {
        public readonly int Chapter;

        public MediaPlayerChapterChangedEventArgs(int chapter)
        {
            Chapter = chapter;
        }
    }

    public class MediaPlayerSnapshotTakenEventArgs : EventArgs
    {
        public readonly string Filename;

        public MediaPlayerSnapshotTakenEventArgs(string filename)
        {
            Filename = filename;
        }
    }

    public class MediaPlayerLengthChangedEventArgs : EventArgs
    {
        public readonly long Length;

        public MediaPlayerLengthChangedEventArgs(long length)
        {
            Length = length;
        }
    }

    public class MediaPlayerVoutEventArgs : EventArgs
    {
        public readonly int Count;

        public MediaPlayerVoutEventArgs(int count)
        {
            Count = count;
        }
    }

    public class MediaPlayerScrambledChangedEventArgs : EventArgs
    {
        public readonly int Scrambled;

        public MediaPlayerScrambledChangedEventArgs(int scrambled)
        {
            Scrambled = scrambled;
        }
    }

    public class MediaPlayerESAddedEventArgs : EventArgs
    {
        public readonly int Id;

        public MediaPlayerESAddedEventArgs(int id)
        {
            Id = id;
        }
    }

    public class MediaPlayerESDeletedEventArgs : EventArgs
    {
        public readonly int Id;

        public MediaPlayerESDeletedEventArgs(int id)
        {
            Id = id;
        }
    }

    public class MediaPlayerESSelectedEventArgs : EventArgs
    {
        public readonly int Id;

        public MediaPlayerESSelectedEventArgs(int id)
        {
            Id = id;
        }
    }

    public class MediaPlayerAudioDeviceEventArgs : EventArgs
    {
        public readonly string AudioDevice;

        public MediaPlayerAudioDeviceEventArgs(string audioDevice)
        {
            AudioDevice = audioDevice;
        }
    }

    public class MediaPlayerVolumeChangedEventArgs : EventArgs
    {
        public readonly float Volume;

        public MediaPlayerVolumeChangedEventArgs(float volume)
        {
            Volume = volume;
        }
    }

    #endregion

    #region MediaList events

    public abstract class MediaListBaseEventArgs : EventArgs
    {
        public readonly Media Media;
        public readonly int Index;

        protected MediaListBaseEventArgs(Media media, int index)
        {
            Media = media;
            Index = index;
        }
    }

    public class MediaListItemAddedEventArgs : MediaListBaseEventArgs
    {
        public MediaListItemAddedEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    public class MediaListWillAddItemEventArgs : MediaListBaseEventArgs
    {
        public MediaListWillAddItemEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    public class MediaListItemDeletedEventArgs : MediaListBaseEventArgs
    {
        public MediaListItemDeletedEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    public class MediaListWillDeleteItemEventArgs : MediaListBaseEventArgs
    {
        public MediaListWillDeleteItemEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    #endregion

    #region MediaListPlayer events

    public class MediaListPlayerNextItemSetEventArgs : EventArgs
    {
        public readonly Media Media;

        public MediaListPlayerNextItemSetEventArgs(Media media)
        {
            Media = media;
        }
    }

    #endregion

    #region VLM events

    public class VLMMediaEventArgs : EventArgs
    {
        public readonly string InstanceName;
        public readonly string MediaName;

        public VLMMediaEventArgs(string mediaName = "", string instanceName = "")
        {
            MediaName = mediaName;
            InstanceName = instanceName;
        }
    }

    #endregion
}