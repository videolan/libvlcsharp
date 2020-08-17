using Android.App;
using Android.Content.PM;
using Android.OS;
using LibVLCSharp.Forms.Platforms.Android;
using LibVLCSharp.Forms.Sample.MediaPlayerElement;

namespace LibVLCSharp.Forms.Sample.MediaPlayerElement.Android
{
    /// <summary>
    /// Represents the main activity.
    /// </summary>
    [Activity(Label = "Sample",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">If the activity is being re-initialized after previously being shut down then this Bundle contains the 
        /// data it most recently supplied in <see cref="Activity.OnSaveInstanceState(Bundle)"/></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Platform.Init(this);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        /// <summary>
        /// Called after <see cref="Activity.OnRestoreInstanceState(Bundle)"/>, <see cref="Activity.OnRestart"/>, or <see cref=" Activity.OnPause"/>, 
        /// for your activity to start interacting with the user.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
