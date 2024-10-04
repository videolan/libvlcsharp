using LibVLCSharp.Shared.MediaPlayerElement;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// Monitors display-related information for an application view.
    /// </summary>
    internal class DisplayInformation : IDisplayInformation
    {
        /// <summary>
        /// Gets the scale factor
        /// </summary>
        public double ScalingFactor => DeviceDisplay.MainDisplayInfo.Density;
    }
}
