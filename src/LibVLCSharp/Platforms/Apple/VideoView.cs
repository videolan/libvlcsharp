using System;

using LibVLCSharp;

#if IOS || TVOS
using UIKit;
#elif MAC
using AppKit;
#endif

#if IOS
namespace LibVLCSharp
#elif TVOS
namespace LibVLCSharp
#elif MAC
namespace LibVLCSharp
#endif
{
    /// <summary>
    /// VideoView implementation for the Apple platform
    /// </summary>
#if IOS || TVOS
    public class VideoView : UIView, IVideoView
#elif MAC
    public class VideoView : NSView, IVideoView
#endif
    {
        MediaPlayer? _mediaPlayer;

        /// <summary>
        /// The MediaPlayer object attached to this VideoView. Use this to manage playback and more
        /// </summary>
        public MediaPlayer? MediaPlayer
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
