using LibVLCSharp.Platforms.tvOS;
using LibVLCSharp.Shared;
using System;
using UIKit;

namespace LibVLCSharp.tvOS.Sample
{
    public class ViewController : UIViewController
    {
        VideoView _videoView;
        LibVLC _libVLC;
        Shared.MediaPlayer _mediaPlayer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _libVLC = new LibVLC(enableDebugLogs: true);
            _mediaPlayer = new Shared.MediaPlayer(_libVLC);

            _videoView = new VideoView { MediaPlayer = _mediaPlayer };

            View = _videoView;

            var media = new Media(_libVLC, new Uri("https://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi"));

            _videoView.MediaPlayer.Play(media);

            media.Dispose();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }
    }
}
