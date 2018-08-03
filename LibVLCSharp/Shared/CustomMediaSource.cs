using System;

namespace LibVLCSharp.Shared
{
    public class CustomMediaSource : ICustomMediaSource
    {
        public CustomMediaSource(MediaPlayer mediaPlayer)
        {
            MediaPlayer = mediaPlayer;
        }

        public MediaPlayer MediaPlayer { get; protected set; }

        public void SetWindowHandle(IntPtr handle)
        {
            MediaPlayer.SetWindowHandle(handle);
        }
    }
}
