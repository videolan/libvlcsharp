using System;

using AppKit;
using Foundation;

using LibVLCSharp.Platforms.Mac;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Mac.Sample
{
    public partial class ViewController : NSViewController
    {
        VideoView _videoView;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _videoView = new VideoView();

            View = _videoView;

            _videoView.MediaPlayer.Play(new Media(
                _videoView.LibVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4",
                Media.FromType.FromLocation));
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
   }
}
