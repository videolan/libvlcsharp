#nullable enable
namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Interface to get display-related information for an application view
    /// </summary>
    public interface IDisplayInformation
    {
        /// <summary>
        /// Gets the scale factor
        /// </summary>
        public double ScalingFactor { get; }
    }
}
#nullable restore
