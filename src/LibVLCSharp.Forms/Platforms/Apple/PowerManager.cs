#if IOS
using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PowerManager))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    /// <summary>
    /// Power manager.
    /// </summary>
    internal class PowerManager : IPowerManager
    {
        /// <summary>
        /// Gets or sets a value indicating whether the screen should be kept on.
        /// </summary>
        public bool KeepScreenOn
        {
            get => UIApplication.SharedApplication.IdleTimerDisabled;
            set => UIApplication.SharedApplication.IdleTimerDisabled = value;
        }
    }
}
#endif
