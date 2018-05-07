using System;

using LibVLCSharp.Shared;

using UIKit;

namespace LibVLCSharp.Platforms.iOS
{
    public class VideoView : UIView, IVideoView
    {
        public VideoView()
        {
            Instance = new Instance();
            MediaPlayer = new MediaPlayer(Instance);
        }

        public MediaPlayer MediaPlayer { get; }
        public Instance Instance { get; }

        public void Attach(UIView view) => MediaPlayer.NsObject = view.Handle;

        public void Attach() => MediaPlayer.NsObject = Handle;

        public void Detach() => MediaPlayer.NsObject = IntPtr.Zero;
    }
}