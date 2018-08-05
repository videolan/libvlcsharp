using System;

namespace LibVLCSharp.Shared
{
    public class MediaSource : IMediaSource
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

        private MediaSource(LibVLC libVLC)
        {
            LibVLC = libVLC;
            MediaPlayer = new MediaPlayer(libVLC);
        }

        ~MediaSource()
        {
            Dispose();
        }

        public MediaPlayer MediaPlayer { get; }
        public LibVLC LibVLC { get; }

#if WINDOWS
        public IntPtr Hwnd
        {
            set { MediaPlayer.Hwnd = value; }
        }
#elif ANDROID
        public void SetAndroidContext(IntPtr aWindow)
        {
            MediaPlayer.SetAndroidContext(aWindow);
        }
#elif COCOA
        public IntPtr NsObject
        {
            set { MediaPlayer.NsObject = value; }
        }
#endif

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
