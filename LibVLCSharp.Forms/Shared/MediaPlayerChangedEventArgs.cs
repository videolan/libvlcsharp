using System;

namespace LibVLCSharp.Forms.Shared
{
    public class MediaPlayerChangedEventArgs : EventArgs
    {
        public MediaPlayerChangedEventArgs(LibVLCSharp.Shared.MediaPlayer oldMediaPlayer, LibVLCSharp.Shared.MediaPlayer newMediaPlayer)
        {
            OldMediaPlayer = oldMediaPlayer;
            NewMediaPlayer = newMediaPlayer;
        }

        public LibVLCSharp.Shared.MediaPlayer OldMediaPlayer { get; }
        public LibVLCSharp.Shared.MediaPlayer NewMediaPlayer { get; }
    }
}