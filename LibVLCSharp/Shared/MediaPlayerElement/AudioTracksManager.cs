using System.Collections.Generic;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared.MediaPlayerElement
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
        /// <remarks>returns -1 if no active input</remarks>
        public override int CurrentTrackId
        {
            get => GetCurrentTrackId(MediaPlayer?.AudioTrack);
            set => SetCurrentTrackId(mp => mp.SetAudioTrack(value));
        }

        /// <summary>
        /// Gets the tracks descriptions
        /// </summary>
        public override IEnumerable<TrackDescription>? Tracks => MediaPlayer?.AudioTrackDescription;
    }
}
