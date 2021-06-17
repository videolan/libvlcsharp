#if IOS
using Foundation;
using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OrientationHandler))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    /// <summary>
    /// Force orientation of iOS device.
    /// In iOs client project, Developer should subscribe the AppDelegate.cs class to
    /// all MessagingCenter below.
    /// Refer to the sample LibVLCSharp.Forms.Sample.MediaElement for to see how to do it.
    /// </summary>
    public class OrientationHandler : IOrientationHandler
    {
        /// <summary>
        /// Force Landscape mode.
        /// </summary>
        public void ForceLandscape()
        {
            MessagingCenter.Send(this, "Landscape");
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
        }
        /// <summary>
        /// Force Portrait mode.
        /// </summary>
        public void ForcePortrait()
        {
            MessagingCenter.Send(this, "Portrait");
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
        }

        /// <summary>
        /// Restore Landscape and Portrait orientation mode.
        /// </summary>
        public void ResetOrientation() => MessagingCenter.Send(this, "All");
    }
}
#endif
