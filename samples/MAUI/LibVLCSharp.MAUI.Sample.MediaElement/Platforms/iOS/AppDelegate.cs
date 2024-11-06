using Foundation;
using Microsoft.Maui;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    /// <summary>
    /// The AppDelegate class is responsible for handling application-level events.
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        /// <summary>
        /// This method is used to create and configure the Maui application instance.
        /// </summary>
        /// <returns>Returns the configured <see cref="MauiApp"/> application instance.</returns>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
