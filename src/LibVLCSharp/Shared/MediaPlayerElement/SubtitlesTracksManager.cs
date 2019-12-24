using System.Collections.Generic;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared.MediaPlayerElement
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
        public override int CurrentTrackId
        {
            get => GetCurrentTrackId(MediaPlayer?.Spu);
            set => SetCurrentTrackId(mp => mp.SetSpu(value));
        }

        /// <summary>
        /// Gets the tracks descriptions
        /// </summary>
        public override IEnumerable<TrackDescription>? Tracks => MediaPlayer?.SpuDescription;
    }
}
