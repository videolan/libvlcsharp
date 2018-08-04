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
            set 
            { 
                if (MediaPlayer.NativeReference != IntPtr.Zero)
                {        
                    MediaPlayer.Hwnd = value; 
                }
            }
        }
#elif ANDROID
        public void SetAndroidContext(IntPtr handle)
        {
            if (MediaPlayer.NativeReference != IntPtr.Zero)
            { 
                MediaPlayer.SetAndroidContext(handle);
            }
        }
#elif COCOA
        public IntPtr NsObject 
        { 
            set 
            {
                if (MediaPlayer.NativeReference != IntPtr.Zero)
                { 
                    MediaPlayer.NsObject = value; }
                }
            }
        }
#endif
    }
}
