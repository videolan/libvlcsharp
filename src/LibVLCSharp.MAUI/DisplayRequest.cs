using LibVLCSharp.Shared.MediaPlayerElement;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// Represents a display request
    /// </summary>
    internal class DisplayRequest : IDisplayRequest
    {
        /// <summary>
        /// Activates a display request
        /// </summary>
        public void RequestActive()
        {
            DeviceDisplay.Current.KeepScreenOn = true;
        }

        /// <summary>
        /// Deactivates a display request
        /// </summary>
        public void RequestRelease()
        {
            DeviceDisplay.Current.KeepScreenOn = false;
        }
    }
}
