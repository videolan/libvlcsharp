using System;

namespace LibVLCSharp.Shared
{
    public class MediaSource : IMediaSource
    {
        protected internal MediaSource(MediaPlayer mediaPlayer)
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

        public static ILibVLCMediaSource CreateFromUri(string uri)
        {
            return new LibVLCMediaSource(uri);
        }

        public static ILibVLCMediaSource Create(params string[] cliOptions)
        {
            return new LibVLCMediaSource(cliOptions);
        }

        public static IMediaSource CreateFromMediaPlayer(MediaPlayer mediaPlayer)
        {
            return new MediaSource(mediaPlayer);
        }
    }
}
