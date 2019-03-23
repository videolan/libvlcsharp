using UIKit;

namespace LibVLCSharp.Forms.PowerManagement
{
    /// <summary>
    /// Power manager.
    /// </summary>
    public static class PowerManager
    {
        /// <summary>
        /// Gets or sets a value indicating whether the screen should be kept on.
        /// </summary>
        public static bool KeepScreenOn
        {
            get => UIApplication.SharedApplication.IdleTimerDisabled;
            set => UIApplication.SharedApplication.IdleTimerDisabled = value;
        }
    }
}
