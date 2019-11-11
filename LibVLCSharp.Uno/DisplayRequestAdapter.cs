using LibVLCSharp.Shared.MediaPlayerElement;
using Windows.System.Display;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Represents a display request
    /// </summary>
    internal class DisplayRequestAdapter : IDisplayRequest
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DisplayRequestAdapter"/> class
        /// </summary>
        public DisplayRequestAdapter()
        {
            DisplayRequest = new DisplayRequest();
        }

        private DisplayRequest DisplayRequest { get; }

        /// <summary>
        /// Activates a display request
        /// </summary>
        public void RequestActive()
        {
            DisplayRequest.RequestActive();
        }

        /// <summary>
        /// Deactivates a display request
        /// </summary>
        public void RequestRelease()
        {
            DisplayRequest.RequestRelease();
        }
    }
}
