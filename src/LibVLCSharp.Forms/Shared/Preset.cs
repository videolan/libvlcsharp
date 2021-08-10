using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Defines the <see cref="Preset" />.
    /// </summary>
    public class Preset : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the PresetId.
        /// </summary>
        public int PresetId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string? Name { get; set; }

        private float preamp;
        /// <summary>
        /// Gets or sets the Preset amplification.
        /// </summary>
        public float Preamp
        {
            get { return preamp; }
            set
            {
                preamp = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the BandCount.
        /// </summary>
        public int BandCount { get; set; }

        /// <summary>
        /// Gets or sets the Bands.
        /// </summary>
        public List<Band>? Bands { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="presetId">The preset Id</param>
        /// <param name="name">The preset name</param>
        internal Preset(int presetId, string? name)
        {
            PresetId = presetId;
            Name = name;
        }

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
