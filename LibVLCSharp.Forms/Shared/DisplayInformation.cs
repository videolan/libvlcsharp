using LibVLCSharp.Shared.MediaPlayerElement;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Monitors display-related information for an application view.
    /// </summary>
    public class DisplayInformation : IDisplayInformation
    {
        /// <summary>
        /// Gets the scale factor
        /// </summary>
        public double ScalingFactor => Device.Info.ScalingFactor;
    }
}
