using System;

namespace LibVLCSharp.Shared
{
    public class MediaPlayerChangingEventArgs : EventArgs
    {
        public MediaPlayerChangingEventArgs(LibVLCSharp.Shared.MediaPlayer oldMediaPlayer, LibVLCSharp.Shared.MediaPlayer newMediaPlayer)
        {
            OldMediaPlayer = oldMediaPlayer;
            NewMediaPlayer = newMediaPlayer;
        }

        public LibVLCSharp.Shared.MediaPlayer OldMediaPlayer { get; }
        public LibVLCSharp.Shared.MediaPlayer NewMediaPlayer { get; }
    }
}
