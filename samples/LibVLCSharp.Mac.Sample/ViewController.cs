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
        LibVLC _libVLC;
        Shared.MediaPlayer _mediaPlayer;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _libVLC = new LibVLC(enableDebugLogs: true);
            _mediaPlayer = new Shared.MediaPlayer(_libVLC);

            _videoView = new VideoView { MediaPlayer = _mediaPlayer };

            View = _videoView;

            var media = new Media(_libVLC, new Uri("https://streams.videolan.org/misc/unity-samples/BigBuckBunny.avi"));
            _videoView.MediaPlayer.Play(media);
            media.Dispose();
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

        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();

            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }
    }
}
