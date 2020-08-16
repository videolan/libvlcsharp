using Android.Views;
using LibVLCSharp.Forms.Platforms.Android;
using LibVLCSharp.Forms.Shared;
using Xamarin.Forms;

[assembly: Dependency(typeof(SystemUI))]
namespace LibVLCSharp.Forms.Platforms.Android
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
