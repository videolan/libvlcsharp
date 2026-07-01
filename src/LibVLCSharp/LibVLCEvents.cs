using System;

namespace LibVLCSharp
{
    #region Media events

    /// <summary>
    /// Media metadata changed
    /// </summary>
    public class MediaMetaChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Type of the metadata that changed
        /// </summary>
        public readonly MetadataType MetadataType;

        internal MediaMetaChangedEventArgs(MetadataType metadataType)
        {
            MetadataType = metadataType;
        }
    }

    /// <summary>
    /// Media parsed status changed
    /// </summary>
    public class MediaParsedChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new parsed status
        /// </summary>
        public readonly MediaParsedStatus ParsedStatus;

        internal MediaParsedChangedEventArgs(MediaParsedStatus parsedStatus)
        {
            ParsedStatus = parsedStatus;
        }
    }

    /// <summary>
    /// Media sub item added
    /// </summary>
    public class MediaSubItemAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The newly added media subitem
        /// </summary>
        public readonly Media SubItem;

        internal MediaSubItemAddedEventArgs(IntPtr mediaPtr)
        {
            SubItem = new Media(mediaPtr);
        }

        internal MediaSubItemAddedEventArgs(Media media)
        {
            SubItem = media;
        }
    }

    /// <summary>
    /// The duration of the media changed
    /// </summary>
    public class MediaDurationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new media duration
        /// </summary>
        public readonly long Duration;

        internal MediaDurationChangedEventArgs(long duration)
        {
            Duration = duration;
        }
    }

    /// <summary>
    /// A new thumbnail picture was generated
    /// </summary>
    public class MediaThumbnailGeneratedEventArgs : EventArgs
    {
        /// <summary>
        /// New thumbnail
        /// </summary>
        public readonly Picture? Thumbnail;

        internal MediaThumbnailGeneratedEventArgs(IntPtr thumbnailPtr)
        {
            Thumbnail = thumbnailPtr == IntPtr.Zero ? null : new Picture(thumbnailPtr);
        }
    }

    /// <summary>
    /// Attached thumbnails have been found during pre-parsing or playback
    /// </summary>
    public class MediaAttachedThumbnailsFoundEventArgs : EventArgs
    {
        /// <summary>
        /// Attached thumbnails
        /// </summary>
        public readonly PictureList? AttachedThumbnails;

        internal MediaAttachedThumbnailsFoundEventArgs(IntPtr pictureList)
        {
            AttachedThumbnails = pictureList == IntPtr.Zero ? null : new PictureList(pictureList);
        }
    }

    /// <summary>
    /// A media sub item tree has been added
    /// </summary>
    public class MediaSubItemTreeAddedEventArgs : EventArgs
    {
        /// <summary>
        /// New media sub item tree
        /// </summary>
        public readonly Media SubItem;

        internal MediaSubItemTreeAddedEventArgs(IntPtr subItemPtr)
        {
            SubItem = new Media(subItemPtr);
        }

        internal MediaSubItemTreeAddedEventArgs(Media media)
        {
            SubItem = media;
        }
    }

    #endregion

    #region MediaPlayer events

    /// <summary>
    /// The mediaplayer's media changed
    /// </summary>
    public class MediaPlayerMediaChangedEventArgs : EventArgs
    {
        /// <summary>
        /// New mediaplayer's media
        /// </summary>
        public readonly Media Media;

        internal MediaPlayerMediaChangedEventArgs(IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
        }
    }

    /// <summary>
    /// The mediaplayer buffering information
    /// </summary>
    public class MediaPlayerBufferingEventArgs : EventArgs
    {
        /// <summary>
        /// Caching information
        /// </summary>
        public readonly float Cache;

        internal MediaPlayerBufferingEventArgs(float cache)
        {
            Cache = cache;
        }
    }

    /// <summary>
    /// The mediaplayer's time changed
    /// </summary>
    public class MediaPlayerTimeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Mediaplayer's current time
        /// </summary>
        public readonly long Time;

        internal MediaPlayerTimeChangedEventArgs(long time)
        {
            Time = time;
        }
    }

    /// <summary>
    /// The mediaplayer's position changed
    /// </summary>
    public class MediaPlayerPositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Mediaplayer's current position
        /// </summary>
        public readonly double Position;

        internal MediaPlayerPositionChangedEventArgs(double position)
        {
            Position = position;
        }
    }

    /// <summary>
    /// The mediaplayer's seekable status changed
    /// </summary>
    public class MediaPlayerSeekableChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new seekable capability
        /// </summary>
        public readonly int Seekable;

        internal MediaPlayerSeekableChangedEventArgs(int seekable)
        {
            Seekable = seekable;
        }
    }

    /// <summary>
    /// The mediaplayer's pausable status changed
    /// </summary>
    public class MediaPlayerPausableChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new pausable capability
        /// </summary>
        public readonly int Pausable;

        internal MediaPlayerPausableChangedEventArgs(int pausable)
        {
            Pausable = pausable;
        }
    }

    /// <summary>
    /// The mediaplayer's chapter changed
    /// </summary>
    public class MediaPlayerChapterChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new chapter
        /// </summary>
        public readonly int Chapter;

        internal MediaPlayerChapterChangedEventArgs(int chapter)
        {
            Chapter = chapter;
        }
    }

    /// <summary>
    /// The mediaplayer had a snapshot taken
    /// </summary>
    public class MediaPlayerSnapshotTakenEventArgs : EventArgs
    {
        /// <summary>
        /// Filename of the newly taken snapshot
        /// </summary>
        public readonly string Filename;

        internal MediaPlayerSnapshotTakenEventArgs(string filename)
        {
            Filename = filename;
        }
    }

    /// <summary>
    /// The mediaplayer's length changed
    /// </summary>
    public class MediaPlayerLengthChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new mediaplayer length
        /// </summary>
        public readonly long Length;

        internal MediaPlayerLengthChangedEventArgs(long length)
        {
            Length = length;
        }
    }

    /// <summary>
    /// The mediaplayer's video output changed
    /// </summary>
    public class MediaPlayerVoutEventArgs : EventArgs
    {
        /// <summary>
        /// Number of available video outputs
        /// </summary>
        public readonly int Count;

        internal MediaPlayerVoutEventArgs(int count)
        {
            Count = count;
        }
    }

    /// <summary>
    /// The mediaplayer has a new Elementary Stream (ES)
    /// </summary>
    public class MediaPlayerESAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The Id of the new Elementary Stream (ES)
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// The type of the new Elementary Stream (ES)
        /// </summary>
        public readonly TrackType Type;

        internal MediaPlayerESAddedEventArgs(string id, TrackType type)
        {
            Id = id;
            Type = type;
        }
    }

    /// <summary>
    /// An Elementary Stream (ES) was deleted
    /// </summary>
    public class MediaPlayerESDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// The Id of the deleted Elementary Stream (ES)
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// The type of the deleted Elementary Stream (ES)
        /// </summary>
        public readonly TrackType Type;

        internal MediaPlayerESDeletedEventArgs(string id, TrackType type)
        {
            Id = id;
            Type = type;
        }
    }

    /// <summary>
    /// An Elementary Stream (ES) was selected
    /// </summary>
    public class MediaPlayerESSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The Id of the selected Elementary Stream (ES)
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// The type of the seleted Elementary Stream (ES)
        /// </summary>
        public readonly TrackType Type;

        internal MediaPlayerESSelectedEventArgs(string id, TrackType type)
        {
            Id = id;
            Type = type;
        }
    }

    /// <summary>
    /// The mediaplayer's audio device changed
    /// </summary>
    public class MediaPlayerAudioDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// String describing the audio device
        /// </summary>
        public readonly string AudioDevice;

        internal MediaPlayerAudioDeviceEventArgs(string audioDevice)
        {
            AudioDevice = audioDevice;
        }
    }

    /// <summary>
    /// The mediaplayer's volume changed
    /// </summary>
    public class MediaPlayerVolumeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new volume
        /// </summary>
        public readonly float Volume;

        internal MediaPlayerVolumeChangedEventArgs(float volume)
        {
            Volume = volume;
        }
    }

    /// <summary>
    /// The mediaplayer detected a new program
    /// </summary>
    public class MediaPlayerProgramAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The id of the newly added program
        /// </summary>
        public readonly int Id;

        internal MediaPlayerProgramAddedEventArgs(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// The mediaplayer detected a deleted program
    /// </summary>
    public class MediaPlayerProgramDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// The id of the newly deleted program
        /// </summary>
        public readonly int Id;

        internal MediaPlayerProgramDeletedEventArgs(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// The mediaplayer detected a new updated program
    /// </summary>
    public class MediaPlayerProgramUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The id of the newly updated program
        /// </summary>
        public readonly int Id;

        internal MediaPlayerProgramUpdatedEventArgs(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// The mediaplayer detected a new updated program
    /// </summary>
    public class MediaPlayerProgramSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The id of the newly unselected program
        /// </summary>
        public readonly int UnselectedId;

        /// <summary>
        /// The id of the newly selected program
        /// </summary>
        public readonly int SelectedId;

        internal MediaPlayerProgramSelectedEventArgs(int unselectedId, int selectedId)
        {
            UnselectedId = unselectedId;
            SelectedId = selectedId;
        }
    }

    /// <summary>
    /// The mediaplayer started or stopped recording
    /// </summary>
    public class MediaPlayerRecordChangedEventArgs : EventArgs
    {
        /// <summary>
        /// True if the mediaplayer started recording, 
        /// false when the mediaplayer stopped recording
        /// </summary>
        public readonly bool IsRecording;

        /// <summary>
        /// filepath of the recorded file, only valid when <see cref="IsRecording"/> is false
        /// </summary>
        public readonly string? FilePath;

        internal MediaPlayerRecordChangedEventArgs(bool isRecording, string? filePath)
        {
            IsRecording = isRecording;
            FilePath = filePath;
        }
    }

    #endregion

    #region MediaDiscoverer events

    /// <summary>
    /// A media discoverer found a new media item.
    /// </summary>
    public class MediaDiscovererMediaAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The newly discovered media
        /// </summary>
        public readonly Media Media;

        /// <summary>
        /// The parent media of the newly discovered media, if any
        /// </summary>
        public readonly Media? Parent;

        internal MediaDiscovererMediaAddedEventArgs(IntPtr parentPtr, IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
            Parent = parentPtr == IntPtr.Zero ? null : new Media(parentPtr);
        }
    }

    /// <summary>
    /// A media discoverer removed a media item.
    /// </summary>
    public class MediaDiscovererMediaRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// The removed media
        /// </summary>
        public readonly Media Media;

        internal MediaDiscovererMediaRemovedEventArgs(IntPtr mediaPtr)
        {
            Media = new Media(mediaPtr);
        }
    }

    #endregion

    #region MediaList events

    /// <summary>
    /// Base class for MediaList events
    /// </summary>
    public abstract class MediaListBaseEventArgs : EventArgs
    {
        /// <summary>
        /// Current node
        /// </summary>
        public readonly Media Media;

        /// <summary>
        /// Current index
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="media">Current node</param>
        /// <param name="index">Current index</param>
        internal protected MediaListBaseEventArgs(Media media, int index)
        {
            Media = media;
            Index = index;
        }
    }

    /// <summary>
    /// An item has been added to the MediaList
    /// </summary>
    public class MediaListItemAddedEventArgs : MediaListBaseEventArgs
    {
        internal MediaListItemAddedEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    /// <summary>
    /// An item is about to be added to the MediaList
    /// </summary>
    public class MediaListWillAddItemEventArgs : MediaListBaseEventArgs
    {
        internal MediaListWillAddItemEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    /// <summary>
    /// An item has been deleted from the MediaList
    /// </summary>
    public class MediaListItemDeletedEventArgs : MediaListBaseEventArgs
    {
        internal MediaListItemDeletedEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    /// <summary>
    /// An item is about to be deleted from the MediaList
    /// </summary>
    public class MediaListWillDeleteItemEventArgs : MediaListBaseEventArgs
    {
        internal MediaListWillDeleteItemEventArgs(Media media, int index) : base(media, index)
        {
        }
    }

    #endregion

    #region RendererDiscoverer events

    /// <summary>
    /// A new RendererItem has been found
    /// </summary>
    public class RendererDiscovererItemAddedEventArgs : EventArgs
    {
        internal RendererDiscovererItemAddedEventArgs(RendererItem rendererItem)
        {
            RendererItem = rendererItem;
        }

        /// <summary>
        /// The newly found RendererItem
        /// </summary>
        public RendererItem RendererItem { get; }
    }

    /// <summary>
    /// A RendererItem has been deleted
    /// </summary>
    public class RendererDiscovererItemDeletedEventArgs : EventArgs
    {
        internal RendererDiscovererItemDeletedEventArgs(RendererItem rendererItem)
        {
            RendererItem = rendererItem;
        }

        /// <summary>
        /// The deleted RendererItem
        /// </summary>
        public RendererItem RendererItem { get; }
    }

    #endregion

    /// <summary>
    /// The LibVLC Log Event Arg
    /// </summary>
    public sealed class LogEventArgs : EventArgs
    {
        internal LogEventArgs(LogLevel level, string message, string? module, string? sourceFile, uint? sourceLine,
            string? objectName, string? objectHeader, UIntPtr? objectId)
        {
            Level = level;
            Message = message;
            Module = module;
            SourceFile = sourceFile;
            SourceLine = sourceLine;
            ObjectName = objectName;
            ObjectHeader = objectHeader;
            ObjectId = objectId;
            FormattedLog = $"{module} {level}: {message}";
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
        public string? Module { get; }

        /// <summary>
        /// The source file that emitted the message.
        /// This may be <see langword="null"/> if that info is not available, i.e. always if you are using a release version of VLC.
        /// </summary>
        public string? SourceFile { get; }

        /// <summary>
        /// The line in the <see cref="SourceFile"/> at which the message was emitted.
        /// This may be <see langword="null"/> if that info is not available, i.e. always if you are using a release version of VLC.
        /// </summary>
        public uint? SourceLine { get; }

        /// <summary>
        /// The type name of the VLC object that emitted the message.
        /// </summary>
        public string? ObjectName { get; }

        /// <summary>
        /// Optional object header associated with the log message.
        /// </summary>
        public string? ObjectHeader { get; }

        /// <summary>
        /// Temporarily unique native object identifier, or <see langword="null"/> if no object was associated with the message.
        /// </summary>
        public UIntPtr? ObjectId { get; }

        /// <summary>
        /// Helper property with already formatted log message
        /// </summary>
        public string FormattedLog { get; }
    }
}
