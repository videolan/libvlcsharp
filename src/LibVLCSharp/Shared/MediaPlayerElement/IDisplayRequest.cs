namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Interface for display requests
    /// </summary>
    internal interface IDisplayRequest
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
