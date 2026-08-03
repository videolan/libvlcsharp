using LibVLCSharp.Shared.MediaPlayerElement;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Monitors display-related information for an application view
    /// </summary>
    internal class DisplayInformation : IDisplayInformation
    {
        private Microsoft.UI.Xaml.FrameworkElement Owner { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayInformation"/> class.
        /// </summary>
        /// <param name="owner">Element whose display scale is used.</param>
        public DisplayInformation(Microsoft.UI.Xaml.FrameworkElement owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Gets the scale factor
        /// </summary>
#if WINDOWS
        public double ScalingFactor => Owner.XamlRoot?.RasterizationScale ?? 1d;
#else
        public double ScalingFactor => Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
#endif
    }
}
