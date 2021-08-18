namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Defines the <see cref="Band" />.
    /// </summary>
    public class Band
    {
        /// <summary>
        /// Gets or sets the BandId.
        /// </summary>
        public int BandId { get; set; }

        /// <summary>
        /// Gets or sets the Band Frequency.
        /// </summary>
        public float BandFrequency { get; set; }

        /// <summary>
        /// Gets the Amplification.
        /// </summary>
        public float Amp { get; set; }

        /// <summary>
        /// Amplification Min value.
        /// </summary>
        public float AmpMin { get; set; } = -20;

        /// <summary>
        /// Amplification Max value.
        /// </summary>
        public float AmpMax { get; set; } = 20;
    }
}
