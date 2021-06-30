#if IOS
using UIKit;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Platforms.iOS
{
    /// <summary>
    /// Suscribes AppDelegate.cs to orientation change event.
    /// </summary>
    public static class OrientationChangeListener
    {
        private static UIInterfaceOrientationMask? OrientationMode = null;

        /// <summary>
        /// Susbscriber.
        /// </summary>
        /// <param name="appDelegate">AppDelegate.</param>
        /// <returns>The desired orientation to lock.</returns>
        public static UIInterfaceOrientationMask Subscribe(object appDelegate)
        {
            if (OrientationMode == null)
            {
                MessagingCenter.Subscribe<OrientationHandler>(appDelegate, "Lock", o =>
                {
                    OrientationMode = ConvertToOrientationToMask(UIDevice.CurrentDevice.Orientation);
                });
                MessagingCenter.Subscribe<OrientationHandler>(appDelegate, "UnLock", o =>
                {
                    OrientationMode = UIInterfaceOrientationMask.All;
                });
            }

            return (OrientationMode == null) ? UIInterfaceOrientationMask.All : OrientationMode.GetValueOrDefault();
        }

        /// <summary>
        /// Convert UIDeviceOrientation to UIInterfaceOrientation.
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns>Current orientation of the device.</returns>
        public static UIInterfaceOrientationMask ConvertToOrientationToMask(UIDeviceOrientation orientation)
        {
            return orientation switch
            {
                UIDeviceOrientation.LandscapeLeft => UIInterfaceOrientationMask.LandscapeLeft,
                UIDeviceOrientation.LandscapeRight => UIInterfaceOrientationMask.LandscapeLeft,
                UIDeviceOrientation.Portrait => UIInterfaceOrientationMask.Portrait,
                UIDeviceOrientation.PortraitUpsideDown => UIInterfaceOrientationMask.PortraitUpsideDown,
                _ => UIInterfaceOrientationMask.All,
            };
        }
    }
}
#endif
