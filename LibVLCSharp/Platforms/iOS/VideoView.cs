using System;
using LibVLCSharp.Shared;
using UIKit;

namespace LibVLCSharp.Platforms.iOS
{
    public abstract class VideoView : UIViewController, IVideoView
    {
        protected VideoView()
        {
            Instance = new Instance();
            MediaPlayer = new MediaPlayer(Instance);
        }

        public MediaPlayer MediaPlayer { get; }
        public Instance Instance { get; }

        public void AttachView(object surface)
        {
            if(surface == null) throw new NullReferenceException(nameof(surface));

            if (surface is UIView uiView)
            {
                MediaPlayer.NsObject = uiView.Handle;
            }
        }

        public void DetachView()
        {
            MediaPlayer.NsObject = IntPtr.Zero;
        }
    }
}