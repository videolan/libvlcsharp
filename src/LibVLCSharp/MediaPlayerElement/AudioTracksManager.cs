using System.Collections.Generic;

namespace LibVLCSharp.MediaPlayerElement
{
    /// <summary>
    /// Audio tracks manager
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class AudioTracksManager : TracksManager
    {
        /// <summary>
        /// Initialized a new instance of <see cref="AudioTracksManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public AudioTracksManager(IDispatcher? dispatcher) : base(dispatcher, TrackType.Audio)
        {
        }

        /// <summary>
        /// Gets or sets the current track identifier
        /// </summary>
        /// <remarks>returns "" if no active input</remarks>
        public override string CurrentTrackId
        {
            get
            {
                using var track = MediaPlayer?.SelectedTrack(TrackType.Audio);
                return GetCurrentTrackId(track?.Id);
            }
            set => SetCurrentTrackId(mp => mp.Select(TrackType.Audio, value));
        }

        /// <summary>
        /// Gets the tracks descriptions
        /// </summary>
        public override MediaTrackList? Tracks => MediaPlayer?.Tracks(TrackType.Audio);
    }
}
