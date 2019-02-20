using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    /// <summary>LibVLCEvent types</summary>
    internal enum EventType
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
    internal readonly struct LibVLCEvent
    {
        internal readonly EventType Type;

        internal readonly IntPtr Sender;

        internal readonly EventUnion Union;

        [StructLayout(LayoutKind.Explicit)]
        internal readonly struct EventUnion
        {
            // media
            [FieldOffset(0)]
            internal readonly MediaMetaChanged MediaMetaChanged;
            [FieldOffset(0)]
            internal readonly MediaSubItemAdded MediaSubItemAdded;
            [FieldOffset(0)]
            internal readonly MediaDurationChanged MediaDurationChanged;
            [FieldOffset(0)]
            internal readonly MediaParsedChanged MediaParsedChanged;
            [FieldOffset(0)]
            internal readonly MediaFreed MediaFreed;
            [FieldOffset(0)]
            internal readonly MediaStateChanged MediaStateChanged;
            [FieldOffset(0)]
            internal readonly MediaSubItemTreeAdded MediaSubItemTreeAdded;

            // mediaplayer
            [FieldOffset(0)]
            internal readonly MediaPlayerBuffering MediaPlayerBuffering;
            [FieldOffset(0)]
            internal readonly MediaPlayerChapterChanged MediaPlayerChapterChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerPositionChanged MediaPlayerPositionChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerTimeChanged MediaPlayerTimeChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerTitleChanged MediaPlayerTitleChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerSeekableChanged MediaPlayerSeekableChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerPausableChanged MediaPlayerPausableChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerScrambledChanged MediaPlayerScrambledChanged;
            [FieldOffset(0)]
            internal readonly MediaPlayerVoutChanged MediaPlayerVoutChanged;

            // medialist
            [FieldOffset(0)]
            internal readonly MediaListItemAdded MediaListItemAdded;
            [FieldOffset(0)]
            internal readonly MediaListWillAddItem MediaListWillAddItem;
            [FieldOffset(0)]
            internal readonly MediaListItemDeleted MediaListItemDeleted;
            [FieldOffset(0)]
            internal readonly MediaListWillDeleteItem MediaListWillDeleteItem;
            [FieldOffset(0)]
            internal readonly MediaListPlayerNextItemSet MediaListPlayerNextItemSet;

            // mediaplayer
            [FieldOffset(0)]
            internal readonly MediaPlayerSnapshotTaken MediaPlayerSnapshotTaken;
            [FieldOffset(0)]
            internal readonly MediaPlayerLengthChanged MediaPlayerLengthChanged;
            [FieldOffset(0)]
            internal readonly VlmMediaEvent VlmMediaEvent;
            [FieldOffset(0)]
            internal readonly MediaPlayerMediaChanged MediaPlayerMediaChanged;
            [FieldOffset(0)]
            internal readonly EsChanged EsChanged;
            [FieldOffset(0)]
            internal readonly VolumeChanged MediaPlayerVolumeChanged;
            [FieldOffset(0)]
            internal readonly AudioDeviceChanged AudioDeviceChanged;

            // renderer discoverer
            [FieldOffset(0)]
            internal readonly RendererDiscovererItemAdded RendererDiscovererItemAdded;
            [FieldOffset(0)]
            internal readonly RendererDiscovererItemDeleted RendererDiscovererItemDeleted; 
        }

        #region Media
        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaMetaChanged
        {
            internal readonly MetadataType MetaType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaSubItemAdded
        {
            internal readonly IntPtr NewChild;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaDurationChanged
        {
            internal readonly long NewDuration;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaParsedChanged
        {
            internal readonly MediaParsedStatus NewStatus;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaFreed
        {
            internal readonly IntPtr MediaInstance;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaStateChanged
        {
            internal readonly VLCState NewState;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaSubItemTreeAdded
        {
            internal readonly IntPtr MediaInstance;
        }

        #endregion

        #region MediaPlayer 

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerBuffering
        {
            internal readonly float NewCache;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerChapterChanged
        {
            internal readonly int NewChapter;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerPositionChanged
        {
            internal readonly float NewPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerTimeChanged
        {
            internal readonly long NewTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerTitleChanged
        {
            internal readonly int NewTitle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerSeekableChanged
        {
            internal readonly int NewSeekable;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerPausableChanged
        {
            internal readonly int NewPausable;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerScrambledChanged
        {
            internal readonly int NewScrambled;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerVoutChanged
        {
            internal readonly int NewCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerSnapshotTaken
        {
            internal readonly IntPtr Filename;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerLengthChanged
        {
            internal readonly long NewLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct EsChanged
        {
            internal readonly TrackType Type;
            internal readonly int Id;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct AudioDeviceChanged
        {
            internal readonly IntPtr Device;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaPlayerMediaChanged
        {
            internal readonly IntPtr NewMedia;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct VolumeChanged
        {
            internal readonly float Volume;
        }

        #endregion

        #region MediaList

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaListItemAdded
        {
            internal readonly IntPtr MediaInstance;
            internal readonly int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaListWillAddItem
        {
            internal readonly IntPtr MediaInstance;
            internal readonly int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaListItemDeleted
        {
            internal readonly IntPtr MediaInstance;
            internal readonly int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaListWillDeleteItem
        {
            internal readonly IntPtr MediaInstance;
            internal readonly int Index;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct MediaListPlayerNextItemSet
        {
            internal readonly IntPtr MediaInstance;
        }

        #endregion MediaList

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct VlmMediaEvent
        {
            internal readonly IntPtr MediaName;
            internal readonly IntPtr InstanceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct RendererDiscovererItemAdded
        {
            internal readonly IntPtr item;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct RendererDiscovererItemDeleted
        {
            internal readonly IntPtr item;
        }
    }

    #region Media events

    public class MediaMetaChangedEventArgs : EventArgs
    {
        public readonly MetadataType MetadataType;

        internal MediaMetaChangedEventArgs(MetadataType metadataType)
        {
            MetadataType = metadataType;
        }
    }

    public class MediaParsedChangedEventArgs : EventArgs
    {
        public readonly MediaParsedStatus ParsedStatus;

        internal MediaParsedChangedEventArgs(MediaParsedStatus parsedStatus)
        {
            ParsedStatus = parsedStatus;
        }
    }

    public class MediaSubItemAddedEventArgs : EventArgs
    {
        public readonly Media SubItem;

        internal MediaSubItemAddedEventArgs(IntPtr mediaPtr)
        {
            SubItem = new Media(mediaPtr);
        }
    }

    public class MediaDurationChangedEventArgs : EventArgs
    {
        public readonly long Duration;

        internal MediaDurationChangedEventArgs(long duration)
        {
            Duration = duration;
        }
    }

    public class MediaFreedEventArgs : EventArgs
    {
        public readonly Media Media;

        internal MediaFreedEventArgs(IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
        }
    }

    public class MediaStateChangedEventArgs : EventArgs
    {
        public readonly VLCState State;

        internal MediaStateChangedEventArgs(VLCState state)
        {
            State = state;
        }
    }

    public class MediaSubItemTreeAddedEventArgs : EventArgs
    {
        public readonly Media SubItem;

        internal MediaSubItemTreeAddedEventArgs(IntPtr subItemPtr)
        {
            SubItem = new Media(subItemPtr);
        }
    }

    #endregion

    #region MediaPlayer events

    public class MediaPlayerMediaChangedEventArgs : EventArgs
    {
        public readonly Media Media;

        internal MediaPlayerMediaChangedEventArgs(IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
        }
    }

    public class MediaPlayerBufferingEventArgs : EventArgs
    {
        public readonly float Cache;

        internal MediaPlayerBufferingEventArgs(float cache)
        {
            Cache = cache;
        }
    }

    public class MediaPlayerTimeChangedEventArgs : EventArgs
    {
        public readonly long Time;

        internal MediaPlayerTimeChangedEventArgs(long time)
        {
            Time = time;
        }
    }

    public class MediaPlayerPositionChangedEventArgs : EventArgs
    {
        public readonly float Position;

        internal MediaPlayerPositionChangedEventArgs(float position)
        {
            Position = position;
        }
    }

    public class MediaPlayerSeekableChangedEventArgs : EventArgs
    {
        public readonly int Seekable;

        internal MediaPlayerSeekableChangedEventArgs(int seekable)
        {
            Seekable = seekable;
        }
    }

    public class MediaPlayerPausableChangedEventArgs : EventArgs
    {
        public readonly int Pausable;

        internal MediaPlayerPausableChangedEventArgs(int pausable)
        {
            Pausable = pausable;
        }
    }

    public class MediaPlayerTitleChangedEventArgs : EventArgs
    {
        public readonly int Title;

        internal MediaPlayerTitleChangedEventArgs(int title)
        {
            Title = title;
        }
    }

    public class MediaPlayerChapterChangedEventArgs : EventArgs
    {
        public readonly int Chapter;

        internal MediaPlayerChapterChangedEventArgs(int chapter)
        {
            Chapter = chapter;
        }
    }

    public class MediaPlayerSnapshotTakenEventArgs : EventArgs
    {
        public readonly string Filename;

        internal MediaPlayerSnapshotTakenEventArgs(string filename)
        {
            Filename = filename;
        }
    }

    public class MediaPlayerLengthChangedEventArgs : EventArgs
    {
        public readonly long Length;

        internal MediaPlayerLengthChangedEventArgs(long length)
        {
            Length = length;
        }
    }

    public class MediaPlayerVoutEventArgs : EventArgs
    {
        public readonly int Count;

        internal MediaPlayerVoutEventArgs(int count)
        {
            Count = count;
        }
    }

    public class MediaPlayerScrambledChangedEventArgs : EventArgs
    {
        public readonly int Scrambled;

        internal MediaPlayerScrambledChangedEventArgs(int scrambled)
        {
            Scrambled = scrambled;
        }
    }

    public class MediaPlayerESAddedEventArgs : EventArgs
    {
        public readonly int Id;

        internal MediaPlayerESAddedEventArgs(int id)
        {
            Id = id;
        }
    }

    public class MediaPlayerESDeletedEventArgs : EventArgs
    {
        public readonly int Id;

        internal MediaPlayerESDeletedEventArgs(int id)
        {
            Id = id;
        }
    }

    public class MediaPlayerESSelectedEventArgs : EventArgs
    {
        public readonly int Id;

        internal MediaPlayerESSelectedEventArgs(int id)
        {
            Id = id;
        }
    }

    public class MediaPlayerAudioDeviceEventArgs : EventArgs
    {
        public readonly string AudioDevice;

        internal MediaPlayerAudioDeviceEventArgs(string audioDevice)
        {
            AudioDevice = audioDevice;
        }
    }

    public class MediaPlayerVolumeChangedEventArgs : EventArgs
    {
        public readonly float Volume;

        internal MediaPlayerVolumeChangedEventArgs(float volume)
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

        internal protected MediaListBaseEventArgs(Media media, int index)
        {
            Media = media;
            Index = index;
        }
    }

    public class MediaListItemAddedEventArgs : MediaListBaseEventArgs
    {
        internal MediaListItemAddedEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    public class MediaListWillAddItemEventArgs : MediaListBaseEventArgs
    {
        internal MediaListWillAddItemEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    public class MediaListItemDeletedEventArgs : MediaListBaseEventArgs
    {
        internal MediaListItemDeletedEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    public class MediaListWillDeleteItemEventArgs : MediaListBaseEventArgs
    {
        internal MediaListWillDeleteItemEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    #endregion

    #region MediaListPlayer events

    public class MediaListPlayerNextItemSetEventArgs : EventArgs
    {
        public readonly Media Media;

        internal MediaListPlayerNextItemSetEventArgs(Media media)
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

        internal VLMMediaEventArgs(string mediaName = "", string instanceName = "")
        {
            MediaName = mediaName;
            InstanceName = instanceName;
        }
    }

    #endregion

    #region RendererDiscoverer events

    public class RendererDiscovererItemAddedEventArgs : EventArgs
    {
        internal RendererDiscovererItemAddedEventArgs(RendererItem rendererItem)
        {
            RendererItem = rendererItem;
        }

        public RendererItem RendererItem { get; }
    }

    public class RendererDiscovererItemDeletedEventArgs : EventArgs
    {
        internal RendererDiscovererItemDeletedEventArgs(RendererItem rendererItem)
        {
            RendererItem = rendererItem;
        }

        public RendererItem RendererItem { get; }
    }

    #endregion
    public sealed class LogEventArgs : EventArgs
    {
        internal LogEventArgs(LogLevel level, string message, string module, string sourceFile, uint? sourceLine)
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