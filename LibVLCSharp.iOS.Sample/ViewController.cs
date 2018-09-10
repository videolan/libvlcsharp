using LibVLCSharp.Platforms.iOS;
using LibVLCSharp.Shared;
using System.Diagnostics;
using UIKit;

namespace LibVLCSharp.iOS.Sample
{
    public class ViewController : UIViewController
    {
        VideoView _videoView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _videoView = new VideoView();

            View = _videoView;
            
            _videoView.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged; // this is the line causing AOT issue

            _videoView.MediaPlayer.Play(new Media(_videoView.LibVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
        }

        void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            Debug.WriteLine(e.Position);
        }
    }
}