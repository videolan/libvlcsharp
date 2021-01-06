using System.Collections.Generic;

namespace LibVLCSharp.MediaPlayerElement
{
    /// <summary>
    /// Subtitles tracks manager
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class SubtitlesTracksManager : TracksManager
    {
        /// <summary>
        /// Initialized a new instance of <see cref="SubtitlesTracksManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public SubtitlesTracksManager(IDispatcher? dispatcher) : base(dispatcher, TrackType.Text)
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
                using var subtitleTrack = MediaPlayer?.SelectedTrack(TrackType.Text);
                return GetCurrentTrackId(subtitleTrack?.Id);
            }
            set => SetCurrentTrackId(mp => mp.Select(TrackType.Text, value));
        }

        /// <summary>
        /// Gets the tracks descriptions
        /// </summary>
        public override MediaTrackList? Tracks => MediaPlayer?.Tracks(TrackType.Text);
    }
}
