using Android.App;
using Android.Views;

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
            get => (Platform.Activity?.Window?.Attributes?.Flags ?? 0).HasFlag(WindowManagerFlags.KeepScreenOn);
            set
            {
                var window = Platform.Activity?.Window;
                if (window != null)
                {
                    if (value)
                    {
                        window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        window.ClearFlags(WindowManagerFlags.KeepScreenOn);
                    }
                }
            }
        }
    }
}
