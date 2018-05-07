using LibVLCSharp.Platforms.iOS;
using LibVLCSharp.Shared;

using UIKit;

namespace LibVLCSharp.iOS.Sample
{
    public class ViewController : UIViewController
    {
        VideoView _videoView;

        public override void ViewDidLoad()
        {
            _videoView = new VideoView();

            View = _videoView;
            
            _videoView.Attach();
            _videoView.MediaPlayer.Play(new Media(_videoView.Instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
        }
    }
}