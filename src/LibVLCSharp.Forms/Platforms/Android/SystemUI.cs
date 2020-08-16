using Android.Views;
using LibVLCSharp.Forms;
using Xamarin.Forms;

[assembly: Dependency(typeof(SystemUI))]
namespace LibVLCSharp.Forms
{
    internal class SystemUI : ISystemUI
    {
        public void ShowSystemUI()
        {
            var decorView = Platform.Activity?.Window?.DecorView;
            if (decorView == null)
                return;

            decorView.SystemUiVisibility =
                (StatusBarVisibility)(SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutFullscreen);
        }

        public void HideSystemUI()
        {
            var decorView = Platform.Activity?.Window?.DecorView;
            if (decorView == null)
                return;

            decorView.SystemUiVisibility = 
               decorView.SystemUiVisibility |
               (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky | 
               SystemUiFlags.Fullscreen | 
               SystemUiFlags.HideNavigation |
               SystemUiFlags.LayoutStable | 
               SystemUiFlags.LayoutFullscreen | 
               SystemUiFlags.LayoutHideNavigation);
        }
    }
}
