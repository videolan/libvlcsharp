using System;

namespace LibVLCSharp.Shared
{
    public class MediaSource : CustomMediaSource, IMediaSource
    {
        static MediaSource()
        {
            Core.Initialize();
        }

        public MediaSource(params string[] cliOptions) : this(new LibVLC(cliOptions))
        {
        }

        public MediaSource(string uri) : this(new LibVLC())
        {
            MediaPlayer.Media = new Media(LibVLC, uri, Media.FromType.FromLocation);
        }

        private MediaSource(LibVLC libVLC) : base(new MediaPlayer(libVLC))
        {
            LibVLC = libVLC;
        }

        ~MediaSource()
        {
            Dispose();
        }

        public LibVLC LibVLC { get; private set; }

        public void Dispose()
        {
            if (MediaPlayer.NativeReference != IntPtr.Zero)
            {
                MediaPlayer.Media?.Dispose();
            }
            MediaPlayer.Dispose();
            LibVLC.Dispose();
        }
    }
}
