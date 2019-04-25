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
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);            
        }

        protected override void OnResume()
        {
            base.OnResume();

            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC)
            {
                EnableHardwareDecoding = true
            };

            _videoView = new VideoView(this) { MediaPlayer = _mediaPlayer };
            AddContentView(_videoView, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
            var media = new Media(_libVLC, "https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4", FromType.FromLocation);
            //var configuration = new MediaConfiguration();
            //configuration.EnableHardwareDecoding();
            //media.AddOption(configuration);
            _videoView.MediaPlayer.Play(media);
        }

        protected override void OnPause()
        {
            base.OnPause();

            _videoView.MediaPlayer.Stop();
            _videoView.Dispose();
        }
    }
}