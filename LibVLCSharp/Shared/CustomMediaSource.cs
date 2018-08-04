using System;

namespace LibVLCSharp.Shared
{
    public class CustomMediaSource : ISource
    {
        public CustomMediaSource(MediaPlayer mediaPlayer)
        {
            MediaPlayer = mediaPlayer;
        }

        public MediaPlayer MediaPlayer { get; protected set; }

#if WINDOWS
        public IntPtr Hwnd
        { 
            set { MediaPlayer.Hwnd = value; }
        }
#elif ANDROID
        public void SetAndroidContext(IntPtr handle)
        {
            MediaPlayer.SetAndroidContext(handle);
        }
#elif COCOA
        public IntPtr NsObject 
        { 
            set { MediaPlayer.NsObject = value; }
        }
#endif
    }
}
