using Android.Content.PM;
using LibVLCSharp.Forms.Platforms.Android;
using LibVLCSharp.Forms.Shared;
using Xamarin.Forms;

[assembly: Dependency(typeof(OrientationHandler))]
namespace LibVLCSharp.Forms.Platforms.Android
{
 
    /// <summary>
    /// Force orientation of Android device.
    /// </summary>
    public class OrientationHandler : IOrientationHandler
    {
        /// <summary>
        /// Force Landscape mode.
        /// </summary>
        public void ForceLandscape()
        {
            var activity = Platform.Activity;
            if (activity == null)
                return;
            activity.RequestedOrientation = ScreenOrientation.Landscape;
        }

        /// <summary>
        /// Force Portrait mode.
        /// </summary>
        public void ForcePortrait()
        {
            var activity = Platform.Activity;
            if (activity == null)
                return;
            activity.RequestedOrientation = ScreenOrientation.Portrait;
        }

        /// <summary>
        /// Restore Landscape and Portrait orientation mode.
        /// </summary>
        public void ResetOrientation()
        {
            var activity = Platform.Activity;
            if (activity == null)
                return;
            activity.RequestedOrientation = ScreenOrientation.User;
        }
    }
}
