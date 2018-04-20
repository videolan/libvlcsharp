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
        }

        protected override void OnResume()
        {
            base.OnResume();

            _surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            AttachSurfaceView(_surfaceView);

            var media = new Media(new Instance(), "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            MediaPlayer.Media = media;
            MediaPlayer.Play();
        }
    }
}