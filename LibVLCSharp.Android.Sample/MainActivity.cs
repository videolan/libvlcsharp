using Android.App;
using Android.OS;
using Android.Views;
using LibVLCSharp.Platforms.Android;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Android.Sample
{
    [Activity(Label = "LibVLCSharp.Android.Sample", MainLauncher = true)]
    public class MainActivity : VideoView
    {
        SurfaceView _surfaceView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            AttachSurfaceView(_surfaceView);
        }

        protected override void OnResume()
        {
            base.OnResume();

            MediaPlayer.Play(new Media(Instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
        }
    }
}