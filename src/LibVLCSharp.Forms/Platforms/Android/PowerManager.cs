using Android.Views;
using LibVLCSharp.Forms;
using Xamarin.Forms;

[assembly: Dependency(typeof(PowerManager))]
namespace LibVLCSharp.Forms
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
