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
    /// In iOS client project, Developer should override the GetSupportedInterfaceOrientations method.
    /// Refer to the sample LibVLCSharp.Forms.Sample.MediaElement to see how to use it.
    /// </summary>
    public class OrientationHandler : IOrientationHandler
    {
        private const string OrientationLabel = "orientation";
        private const string LockLabel = "Lock";
        private const string UnLockLabel = "UnLock";

        /// <summary>
        /// Lock device's orientation.
        /// </summary>
        public void LockOrientation()
        {
            MessagingCenter.Send(this, LockLabel);
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIDevice.CurrentDevice.Orientation), new NSString(OrientationLabel));
        }

        /// <summary>
        /// Unlock device's orientation.
        /// </summary>
        public void UnLockOrientation() => MessagingCenter.Send(this, UnLockLabel);
    }
}
#endif
