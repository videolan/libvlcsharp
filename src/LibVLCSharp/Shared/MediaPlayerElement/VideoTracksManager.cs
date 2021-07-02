using System.Collections.Generic;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared.MediaPlayerElement
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
        public override int CurrentTrackId
        {
            get => GetCurrentTrackId(MediaPlayer?.VideoTrack);
            set => SetCurrentTrackId(mp => mp.SetVideoTrack(value));
        }

        /// <summary>
        /// Gets the tracks descriptions
        /// </summary>
        public override IEnumerable<TrackDescription>? Tracks => MediaPlayer?.VideoTrackDescription;
    }
}
