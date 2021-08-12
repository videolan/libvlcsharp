using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Defines the <see cref="Band" />.
    /// </summary>
    public class Band : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the BandId.
        /// </summary>
        public int BandId { get; set; }

        private float bandFrequency;
        /// <summary>
        /// Gets or sets the Band Frequency.
        /// </summary>
        public float BandFrequency
        {
            get { return bandFrequency; }
            set
            {
                bandFrequency = value;
                NotifyPropertyChanged();
            }
        }

        private float amp;
        /// <summary>
        /// Gets the Amplification.
        /// </summary>
        public float Amp
        {
            get { return amp; }
            set
            {
                amp = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Amplification Min value.
        /// </summary>
        public float AmpMin { get; set; } = -20;

        /// <summary>
        /// Amplification Max value.
        /// </summary>
        public float AmpMax { get; set; } = +20;

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

