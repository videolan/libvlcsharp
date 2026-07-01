using System;

namespace LibVLCSharp
{
    /// <summary>
    /// Managed dispatcher for media events.
    ///
    /// LibVLC 4 removed the native media event manager (<c>libvlc_media_event_manager</c>) and the
    /// <c>libvlc_event_attach</c>/<c>libvlc_event_detach</c> API. Media notifications emitted by the player
    /// are delivered through <c>libvlc_media_player_cbs</c> and bridged back onto this existing event API.
    /// </summary>
    internal class MediaEventManager : EventManager
    {
        static readonly MetadataType[] s_metadataTypes =
        {
            MetadataType.Title,
            MetadataType.Artist,
            MetadataType.Genre,
            MetadataType.Copyright,
            MetadataType.Album,
            MetadataType.TrackNumber,
            MetadataType.Description,
            MetadataType.Rating,
            MetadataType.Date,
            MetadataType.Setting,
            MetadataType.URL,
            MetadataType.Language,
            MetadataType.NowPlaying,
            MetadataType.Publisher,
            MetadataType.EncodedBy,
            MetadataType.ArtworkURL,
            MetadataType.TrackID,
            MetadataType.TrackTotal,
            MetadataType.Director,
            MetadataType.Season,
            MetadataType.Episode,
            MetadataType.ShowName,
            MetadataType.Actors,
            MetadataType.AlbumArtist,
            MetadataType.DiscNumber,
            MetadataType.DiscTotal
        };

        EventHandler<MediaMetaChangedEventArgs>? _metaChanged;
        EventHandler<MediaParsedChangedEventArgs>? _parsedChanged;
        EventHandler<MediaSubItemAddedEventArgs>? _subItemAdded;
        EventHandler<MediaDurationChangedEventArgs>? _durationChanged;
        EventHandler<MediaSubItemTreeAddedEventArgs>? _subItemTreeAdded;
        EventHandler<MediaThumbnailGeneratedEventArgs>? _thumbnailGenerated;
        EventHandler<MediaAttachedThumbnailsFoundEventArgs>? _attachedThumbnailsFound;
        readonly string?[] _metadataSnapshot = new string?[s_metadataTypes.Length];
        int _subItemCount;

        internal void AddMetaChanged(EventHandler<MediaMetaChangedEventArgs> handler) => _metaChanged += handler;
        internal void RemoveMetaChanged(EventHandler<MediaMetaChangedEventArgs> handler) => _metaChanged -= handler;
        internal void AddParsedChanged(EventHandler<MediaParsedChangedEventArgs> handler) => _parsedChanged += handler;
        internal void RemoveParsedChanged(EventHandler<MediaParsedChangedEventArgs> handler) => _parsedChanged -= handler;
        internal void AddSubItemAdded(EventHandler<MediaSubItemAddedEventArgs> handler) => _subItemAdded += handler;
        internal void RemoveSubItemAdded(EventHandler<MediaSubItemAddedEventArgs> handler) => _subItemAdded -= handler;
        internal void AddDurationChanged(EventHandler<MediaDurationChangedEventArgs> handler) => _durationChanged += handler;
        internal void RemoveDurationChanged(EventHandler<MediaDurationChangedEventArgs> handler) => _durationChanged -= handler;
        internal void AddSubItemTreeAdded(EventHandler<MediaSubItemTreeAddedEventArgs> handler) => _subItemTreeAdded += handler;
        internal void RemoveSubItemTreeAdded(EventHandler<MediaSubItemTreeAddedEventArgs> handler) => _subItemTreeAdded -= handler;
        internal void AddThumbnailGenerated(EventHandler<MediaThumbnailGeneratedEventArgs> handler) => _thumbnailGenerated += handler;
        internal void RemoveThumbnailGenerated(EventHandler<MediaThumbnailGeneratedEventArgs> handler) => _thumbnailGenerated -= handler;
        internal void AddAttachedThumbnailsFound(EventHandler<MediaAttachedThumbnailsFoundEventArgs> handler) => _attachedThumbnailsFound += handler;
        internal void RemoveAttachedThumbnailsFound(EventHandler<MediaAttachedThumbnailsFoundEventArgs> handler) => _attachedThumbnailsFound -= handler;

        internal void Snapshot(Media media)
        {
            SnapshotMetadata(media);
            _subItemCount = SubItemCount(media);
        }

        internal void OnMetaChanged(MetadataType metadataType)
        {
            _metaChanged?.Invoke(this, new MediaMetaChangedEventArgs(metadataType));
        }

        internal void OnNativeMetaChanged(Media media)
        {
            for (var i = 0; i < s_metadataTypes.Length; i++)
            {
                var current = media.Meta(s_metadataTypes[i]);
                if (current == _metadataSnapshot[i])
                    continue;

                _metadataSnapshot[i] = current;
                OnMetaChanged(s_metadataTypes[i]);
            }
        }

        internal void OnParsedChanged(MediaParsedStatus parsedStatus)
        {
            _parsedChanged?.Invoke(this, new MediaParsedChangedEventArgs(parsedStatus));
        }

        internal void OnDurationChanged(long duration)
        {
            _durationChanged?.Invoke(this, new MediaDurationChangedEventArgs(duration));
        }

        internal void OnSubItemsChanged(Media media)
        {
            MediaList? subItems = null;
            try
            {
                subItems = media.SubItems;
                var index = 0;
                foreach (var subItem in subItems)
                {
                    if (index >= _subItemCount)
                    {
                        _subItemAdded?.Invoke(this, new MediaSubItemAddedEventArgs(subItem));
                        _subItemTreeAdded?.Invoke(this, new MediaSubItemTreeAddedEventArgs(subItem));
                    }
                    index++;
                }
                _subItemCount = subItems.Count;
            }
            finally
            {
                subItems?.Dispose();
            }
        }

        internal void OnAttachedThumbnailsFound(IntPtr pictureList)
        {
            _attachedThumbnailsFound?.Invoke(this, new MediaAttachedThumbnailsFoundEventArgs(pictureList));
        }

        void SnapshotMetadata(Media media)
        {
            for (var i = 0; i < s_metadataTypes.Length; i++)
                _metadataSnapshot[i] = media.Meta(s_metadataTypes[i]);
        }

        static int SubItemCount(Media media)
        {
            MediaList? subItems = null;
            try
            {
                subItems = media.SubItems;
                return subItems.Count;
            }
            catch
            {
                return 0;
            }
            finally
            {
                subItems?.Dispose();
            }
        }
    }
}
