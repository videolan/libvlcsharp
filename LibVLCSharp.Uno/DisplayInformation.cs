using LibVLCSharp.Shared.MediaPlayerElement;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Monitors display-related information for an application view
    /// </summary>
    internal class DisplayInformation : IDisplayInformation
    {
        /// <summary>
        /// Gets the scale factor
        /// </summary>
        public double ScalingFactor => Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
    }
}
