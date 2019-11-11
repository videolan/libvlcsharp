namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Interface for display requests
    /// </summary>
    public interface IDisplayRequest
    {
        /// <summary>
        /// Activates a display request
        /// </summary>
        void RequestActive();

        /// <summary>
        /// Deactivates a display request
        /// </summary>
        void RequestRelease();
    }
}
