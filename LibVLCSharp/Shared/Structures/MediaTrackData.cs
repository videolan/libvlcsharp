namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Media track data struct, containing info about audio, video and subtitles track
    /// </summary>
    public readonly struct MediaTrackData
    {
        /// <summary>
        /// Audio track
        /// </summary>
        public readonly AudioTrack Audio;

        /// <summary>
        /// Video track
        /// </summary>
        public readonly VideoTrack Video;

        /// <summary>
        /// Subtitle track
        /// </summary>
        public readonly SubtitleTrack Subtitle;
    }
}
