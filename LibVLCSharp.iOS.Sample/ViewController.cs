using LibVLCSharp.Platforms.iOS;
using LibVLCSharp.Shared;

namespace LibVLCSharp.iOS.Sample
{
    public class ViewController : VideoView
    {
        public override void ViewDidLoad()
        {
            AttachView(View);

            MediaPlayer.Play(new Media(Instance, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                Media.FromType.FromLocation));
        }
    }
}