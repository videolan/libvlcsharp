namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Interface to get display-related information for an application view
    /// </summary>
    internal interface IDisplayInformation
    {
        /// <summary>
        /// Gets the scale factor
        /// </summary>
        double ScalingFactor { get; }
    }
}
