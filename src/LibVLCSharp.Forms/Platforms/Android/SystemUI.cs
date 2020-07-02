using Android.App;
using Android.Views;
using Java.Lang;
using LibVLCSharp.Forms;
using Xamarin.Forms;

[assembly: Dependency(typeof(SystemUI))]
namespace LibVLCSharp.Forms
{
    internal class SystemUI : ISystemUI
    {
        public void ShowSystemUI()
        {
            if (Platform.Activity == null)
                return;

            Platform.Activity.Window.DecorView.SystemUiVisibility =
                (StatusBarVisibility)(SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutFullscreen);
        }

        public void HideSystemUI()
        {
            if (Platform.Activity == null)
                return;

            Platform.Activity.Window.DecorView.SystemUiVisibility = 
               Platform.Activity.Window.DecorView.SystemUiVisibility |
               (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky | 
               SystemUiFlags.Fullscreen | 
               SystemUiFlags.HideNavigation |
               SystemUiFlags.LayoutStable | 
               SystemUiFlags.LayoutFullscreen | 
               SystemUiFlags.LayoutHideNavigation);
        }
    }
}
