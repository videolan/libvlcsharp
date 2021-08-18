using System.Collections.Generic;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Defines the <see cref="Preset" />.
    /// </summary>
    public class Preset
    {
        /// <summary>
        /// Gets or sets the PresetId.
        /// </summary>
        public int PresetId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the Preset amplification.
        /// </summary>
        public float Preamp { get; set; }

        /// <summary>
        /// Gets or sets the BandCount.
        /// </summary>
        public int BandCount { get; set; }

        /// <summary>
        /// Gets or sets the Bands.
        /// </summary>
        public List<Band> Bands { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Preset()
        {
            Bands = new List<Band>();
        }
    }
}
