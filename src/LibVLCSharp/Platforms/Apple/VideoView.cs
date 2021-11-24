using System;

using LibVLCSharp.Shared;

#if IOS || TVOS
using UIKit;
#elif MAC
using AppKit;
#endif

#if IOS
namespace LibVLCSharp.Platforms.iOS
#elif TVOS
namespace LibVLCSharp.Platforms.tvOS
#elif MAC
namespace LibVLCSharp.Platforms.Mac
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
        Shared.MediaPlayer? _mediaPlayer;

        /// <summary>
        /// The MediaPlayer object attached to this VideoView. Use this to manage playback and more
        /// </summary>
        public Shared.MediaPlayer? MediaPlayer
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
            if (MediaPlayer != null && MediaPlayer.NativeReference != IntPtr.Zero)
            {
                MediaPlayer.NsObject = Handle;
            }
        }

        void Detach()
        {
            if (MediaPlayer != null && MediaPlayer.NativeReference != IntPtr.Zero)
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
