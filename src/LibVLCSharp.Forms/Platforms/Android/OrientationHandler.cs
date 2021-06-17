using Android.Content.PM;
using Android.Util;
using Android.Views;
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
        /// Lock device's orientation.
        /// </summary>
        public void LockOrientation()
        {
            var activity = Platform.Activity;
            if (activity == null)
                return;

            var orientation = GetScreenOrientation();
            if (orientation != null)
                activity.RequestedOrientation = (ScreenOrientation)orientation;
            else
                activity.RequestedOrientation = ScreenOrientation.Sensor;
        }

        /// <summary>
        /// Unlock device's orientation.
        /// </summary>
        public void UnLockOrientation()
        {
            var activity = Platform.Activity;
            if (activity == null)
                return;
            activity.RequestedOrientation = ScreenOrientation.User;
        }

        /// <summary>
        /// Get current orientation of the screen.
        /// </summary>
        /// <returns>The orientation</returns>
        private ScreenOrientation? GetScreenOrientation()
        {
            var activity = Platform.Activity;
            if (activity == null)
                return null;

            ScreenOrientation orientation;
            SurfaceOrientation rotation;
            DisplayMetrics dm;

            var windowManager = activity.WindowManager;
            if(windowManager != null && windowManager.DefaultDisplay != null)
            {
                rotation = windowManager.DefaultDisplay.Rotation;
                dm = new DisplayMetrics();
                windowManager.DefaultDisplay.GetMetrics(dm);
            }
            else
                return null;

            if ((rotation == SurfaceOrientation.Rotation0 || rotation == SurfaceOrientation.Rotation180) && dm.HeightPixels > dm.WidthPixels
                || (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270) && dm.WidthPixels > dm.HeightPixels)
            {
                // The device's natural orientation is portrait
                orientation = rotation switch
                {
                    SurfaceOrientation.Rotation0 => ScreenOrientation.Portrait,
                    SurfaceOrientation.Rotation90 => ScreenOrientation.Landscape,
                    SurfaceOrientation.Rotation180 => ScreenOrientation.ReversePortrait,
                    SurfaceOrientation.Rotation270 => ScreenOrientation.ReverseLandscape,
                    _ => ScreenOrientation.Portrait,
                };
            }
            else
            {
                // The device's natural orientation is landscape or if the device is square
                orientation = rotation switch
                {
                    SurfaceOrientation.Rotation0 => ScreenOrientation.Landscape,
                    SurfaceOrientation.Rotation90 => ScreenOrientation.Portrait,
                    SurfaceOrientation.Rotation180 => ScreenOrientation.ReverseLandscape,
                    SurfaceOrientation.Rotation270 => ScreenOrientation.ReversePortrait,
                    _ => ScreenOrientation.Landscape,
                };
            }

            return orientation;
        }
    }
}
