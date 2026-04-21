using LibVLCSharp.Platforms.iOS;
using LibVLCSharp;
using System;
using UIKit;

namespace LibVLCSharp.iOS.Sample
{
    public class ViewController : UIViewController
    {
        VideoView _videoView;
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _libVLC = new LibVLC(enableDebugLogs: true);
            _mediaPlayer = new MediaPlayer(_libVLC);

            _videoView = new VideoView { MediaPlayer = _mediaPlayer };

            View = _videoView;

            using var media = new Media(new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"));
            _videoView.MediaPlayer.Play(media);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }
    }
}
