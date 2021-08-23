namespace LibVLCSharp.MediaPlayerElement
{
    /// <summary>
    /// Video tracks manager
    /// </summary>
    internal class VideoTracksManager : TracksManager
    {
        /// <summary>
        /// Initialized a new instance of <see cref="VideoTracksManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public VideoTracksManager(IDispatcher? dispatcher) : base(dispatcher, TrackType.Video)
        {
        }

        /// <summary>
        /// Gets or sets the current track identifier
        /// </summary>
        /// <remarks>returns -1 if no active input</remarks>
        public override string CurrentTrackId
        {
            get
            {
                return MediaPlayer?.SelectedTrack(TrackType.Video)?.Id ?? string.Empty;
            }

            set
            {
                if (value != null)
                    MediaPlayer?.Select(TrackType.Video, value);
            }
        }

        public override MediaTrackList? Tracks => MediaPlayer?.Tracks(TrackType.Video);
    }
}
