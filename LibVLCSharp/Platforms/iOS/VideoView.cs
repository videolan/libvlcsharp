using System;

using LibVLCSharp.Shared;

using UIKit;

namespace LibVLCSharp.Platforms.iOS
{
    public class VideoView : UIView, IVideoView
    {
        public VideoView(string[] cliOptions = default(string[]))
        {
            Instance = new Instance(cliOptions);
            MediaPlayer = new MediaPlayer(Instance);
            
            Attach();
        }

        public MediaPlayer MediaPlayer { get; }
        public Instance Instance { get; }

        void Attach() => MediaPlayer.NsObject = Handle;

        void Detach() => MediaPlayer.NsObject = IntPtr.Zero;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Detach();

            MediaPlayer.Media?.Dispose();
            MediaPlayer.Dispose();
            Instance.Dispose();
        }
    }
}