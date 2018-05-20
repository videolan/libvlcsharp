using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LibVLCSharp.Shared;
using VideoView = LibVLCSharp.Platforms.Android.VideoView;

namespace LibVLCSharp.Android.Sample
{
    [Activity(Label = "LibVLCSharp.Android.Sample", MainLauncher = true)]
    public class MainActivity : Activity
    {
        VideoView _videoView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);            
        }

        protected override void OnResume()
        {
            base.OnResume();

            _videoView = new VideoView(this);
            AddContentView(_videoView, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
            _videoView.MediaPlayer.Play(new Media(_videoView.Instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
        }

        protected override void OnPause()
        {
            base.OnPause();

            _videoView.MediaPlayer.Stop();
            _videoView.Dispose();
        }
    }
}