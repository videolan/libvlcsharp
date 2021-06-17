using Foundation;
using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Sample.MediaPlayerElement;
using LibVLCSharp.Forms.Shared;
using UIKit;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Sample.MediaElement.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private static UIInterfaceOrientationMask orientationMode;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            LibVLCSharpFormsRenderer.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, [Transient] UIWindow forWindow)
        {
            MessagingCenter.Subscribe<OrientationHandler>(this, "Landscape", o =>
            {
                orientationMode = UIInterfaceOrientationMask.Landscape;

            });
            MessagingCenter.Subscribe<OrientationHandler>(this, "Portrait", o =>
            {
                orientationMode = UIInterfaceOrientationMask.Portrait;

            });
            MessagingCenter.Subscribe<OrientationHandler>(this, "All", o =>
            {
                orientationMode = UIInterfaceOrientationMask.All;

            });

            return orientationMode;
        }
    }
}
