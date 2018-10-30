using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
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
    [StructLayout(LayoutKind.Sequential)]
    public struct LibVLCEvent
    {
        public EventType Type;

        public IntPtr Sender;

        public EventUnion Union;

        [StructLayout(LayoutKind.Explicit)]
        public struct EventUnion
        {
            // media
            [FieldOffset(0)]
            public MediaMetaChanged MediaMetaChanged;
            [FieldOffset(0)]
            public MediaSubItemAdded MediaSubItemAdded;
            [FieldOffset(0)]
            public MediaDurationChanged MediaDurationChanged;
            [FieldOffset(0)]
            public MediaParsedChanged MediaParsedChanged;
            [FieldOffset(0)]
            public MediaFreed MediaFreed;
            [FieldOffset(0)]
            public MediaStateChanged MediaStateChanged;
            [FieldOffset(0)]
            public MediaSubItemTreeAdded MediaSubItemTreeAdded;

            // mediaplayer
            [FieldOffset(0)]
            public MediaPlayerBuffering MediaPlayerBuffering;
            [FieldOffset(0)]
            public MediaPlayerChapterChanged MediaPlayerChapterChanged;
            [FieldOffset(0)]
            public MediaPlayerPositionChanged MediaPlayerPositionChanged;
            [FieldOffset(0)]
            public MediaPlayerTimeChanged MediaPlayerTimeChanged;
            [FieldOffset(0)]
            public MediaPlayerTitleChanged MediaPlayerTitleChanged;
            [FieldOffset(0)]
            public MediaPlayerSeekableChanged MediaPlayerSeekableChanged;
            [FieldOffset(0)]
            public MediaPlayerPausableChanged MediaPlayerPausableChanged;
            [FieldOffset(0)]
            public MediaPlayerScrambledChanged MediaPlayerScrambledChanged;
            [FieldOffset(0)]
            public MediaPlayerVoutChanged MediaPlayerVoutChanged;

            // medialist
            [FieldOffset(0)]
            public MediaListItemAdded MediaListItemAdded;
            [FieldOffset(0)]
            public MediaListWillAddItem MediaListWillAddItem;
            [FieldOffset(0)]
            public MediaListItemDeleted MediaListItemDeleted;
            [FieldOffset(0)]
            public MediaListWillDeleteItem MediaListWillDeleteItem;
            [FieldOffset(0)]
            public MediaListPlayerNextItemSet MediaListPlayerNextItemSet;

            // mediaplayer
            [FieldOffset(0)]
            public MediaPlayerSnapshotTaken MediaPlayerSnapshotTaken;
            [FieldOffset(0)]
            public MediaPlayerLengthChanged MediaPlayerLengthChanged;
            [FieldOffset(0)]
            public VlmMediaEvent VlmMediaEvent;
            [FieldOffset(0)]
            public MediaPlayerMediaChanged MediaPlayerMediaChanged;
            [FieldOffset(0)]
            public EsChanged EsChanged;
            [FieldOffset(0)]
            public VolumeChanged MediaPlayerVolumeChanged;
            [FieldOffset(0)]
            public AudioDeviceChanged AudioDeviceChanged;

            // renderer discoverer
            [FieldOffset(0)]
            public RendererDiscovererItemAdded RendererDiscovererItemAdded;
            [FieldOffset(0)]
            public RendererDiscovererItemDeleted RendererDiscovererItemDeleted; 
        }

        #region Media
        [StructLayout(LayoutKind.Sequential)]
        public struct MediaMetaChanged
        {
            public Media.MetadataType MetaType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaSubItemAdded
        {
            public IntPtr NewChild;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaDurationChanged
        {
            public long NewDuration;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaParsedChanged
        {
            public Media.MediaParsedStatus NewStatus;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaFreed
        {
            public IntPtr MediaInstance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaStateChanged
        {
            public VLCState NewState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaSubItemTreeAdded
        {
            public IntPtr MediaInstance;
        }

        #endregion

        #region MediaPlayer 

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerBuffering
        {
            public float NewCache;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerChapterChanged
        {
            public int NewChapter;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerPositionChanged
        {
            public float NewPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerTimeChanged
        {
            public long NewTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerTitleChanged
        {
            public int NewTitle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerSeekableChanged
        {
            public int NewSeekable;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerPausableChanged
        {
            public int NewPausable;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerScrambledChanged
        {
            public int NewScrambled;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerVoutChanged
        {
            public int NewCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerSnapshotTaken
        {
            public IntPtr Filename;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerLengthChanged
        {
            public long NewLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EsChanged
        {
            public TrackType Type;
            public int Id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AudioDeviceChanged
        {
            public IntPtr Device;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaPlayerMediaChanged
        {
            public IntPtr NewMedia;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VolumeChanged
        {
            public float Volume;
        }

        #endregion

        #region MediaList

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaListItemAdded
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaListWillAddItem
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaListItemDeleted
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaListWillDeleteItem
        {
            public IntPtr MediaInstance;
            public int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MediaListPlayerNextItemSet
        {
            public IntPtr MediaInstance;
        }

        #endregion MediaList

        [StructLayout(LayoutKind.Sequential)]
        public struct VlmMediaEvent
        {
            public IntPtr MediaName;
            public IntPtr InstanceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RendererDiscovererItemAdded
        {
            public IntPtr item;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RendererDiscovererItemDeleted
        {
            public IntPtr item;
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

    #region RendererDiscoverer events

    public class RendererDiscovererItemAddedEventArgs : EventArgs
    {
        public RendererDiscovererItemAddedEventArgs(RendererItem rendererItem)
        {
            RendererItem = rendererItem;
        }

        public RendererItem RendererItem { get; }
    }

    public class RendererDiscovererItemDeletedEventArgs : EventArgs
    {
        public RendererDiscovererItemDeletedEventArgs(RendererItem rendererItem)
        {
            RendererItem = rendererItem;
        }

        public RendererItem RendererItem { get; }
    }

    #endregion
    public sealed class LogEventArgs : EventArgs
    {
        public LogEventArgs(LogLevel level, string message, string module, string sourceFile, uint? sourceLine)
        {
            Level = level;
            Message = message;
            Module = module;
            SourceFile = sourceFile;
            SourceLine = sourceLine;
        }

        /// <summary>
        /// The severity of the log message.
        /// By default, you will only get error messages, but you can get all messages by specifying "-vv" in the options.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// The log message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The name of the module that emitted the message
        /// </summary>
        public string Module { get; }

        /// <summary>
        /// The source file that emitted the message.
        /// This may be <see langword="null"/> if that info is not available, i.e. always if you are using a release version of VLC.
        /// </summary>
        public string SourceFile { get; }

        /// <summary>
        /// The line in the <see cref="SourceFile"/> at which the message was emitted.
        /// This may be <see langword="null"/> if that info is not available, i.e. always if you are using a release version of VLC.
        /// </summary>
        public uint? SourceLine { get; }
    }
}