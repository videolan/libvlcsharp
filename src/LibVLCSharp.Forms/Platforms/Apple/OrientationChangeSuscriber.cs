#if IOS
using UIKit;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Platforms.iOS
{
    /// <summary>
    /// Suscribes AppDelegate.cs to orientation change event.
    /// </summary>
    public static class OrientationChangeSuscriber
    {
        private static UIInterfaceOrientationMask OrientationMode;

        /// <summary>
        /// Susbscriber.
        /// </summary>
        /// <param name="application">UIApplication from GetSupportedInterfaceOrientations method.</param>
        /// <returns>the desired orientation to lock</returns>
        public static UIInterfaceOrientationMask Suscribe(object application)
        {
            MessagingCenter.Subscribe<OrientationHandler>(application, "Landscape", o =>
            {
                OrientationMode = UIInterfaceOrientationMask.Landscape;

            });
            MessagingCenter.Subscribe<OrientationHandler>(application, "Portrait", o =>
            {
                OrientationMode = UIInterfaceOrientationMask.Portrait;

            });
            MessagingCenter.Subscribe<OrientationHandler>(application, "All", o =>
            {
                OrientationMode = UIInterfaceOrientationMask.All;

            });

            return OrientationMode;
        }
    }
}
#endif
