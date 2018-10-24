using System;

using LibVLCSharp.Shared;

using UIKit;

namespace LibVLCSharp.Platforms.iOS
{
    public class VideoView : UIView, IVideoView
    {
        Shared.MediaPlayer _mediaPlayer;

        /// <summary>
        /// The MediaPlayer object attached to this VideoView. Use this to manage playback and more
        /// </summary>
        public Shared.MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                if (_mediaPlayer != value)
                {
                    Detach();
                    _mediaPlayer = value;

                    if (_mediaPlayer != null)
                    {
                        Attach();
                    }
                }
            }
        }

        void Attach()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.NsObject = Handle;
            }
        }

        void Detach()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.NsObject = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Detach the mediaplayer from the view and dispose the view
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Detach();
        }
    }
}