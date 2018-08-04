using System;

namespace LibVLCSharp.Shared
{
    public class LibVLCMediaSource : MediaSource, ILibVLCMediaSource
    {
        static LibVLCMediaSource()
        {
            Core.Initialize();
        }

        protected internal LibVLCMediaSource(params string[] cliOptions) : this(new LibVLC(cliOptions))
        {
        }

        protected internal LibVLCMediaSource(string uri) : this(new LibVLC())
        {
            MediaPlayer.Media = new Media(LibVLC, uri, Media.FromType.FromLocation);
        }

        private LibVLCMediaSource(LibVLC libVLC) : base(new MediaPlayer(libVLC))
        {
            LibVLC = libVLC;
        }

        ~LibVLCMediaSource()
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
            if (LibVLC.NativeReference != IntPtr.Zero)
            {
                LibVLC.Dispose();
            }
        }
    }
}
