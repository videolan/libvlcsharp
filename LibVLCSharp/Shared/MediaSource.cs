using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared
{
    public class MediaSource : IMediaSource
    {
        static MediaSource()
        {
            Core.Initialize();
        }

        protected MediaSource(params string[] cliOptions)
        {
            LibVLC = new LibVLC(cliOptions);
            MediaPlayer = new MediaPlayer(LibVLC);
        }

        protected MediaSource(string uri) : this()
        {
            MediaPlayer.Media = new Media(LibVLC, uri, Media.FromType.FromLocation);
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

        public static Task<MediaSource> CreateAsync(params string[] cliOptions)
        {
            return Task.Run(() => new MediaSource(cliOptions));
        }

        public static Task<MediaSource> CreateFromUriAsync(string uri)
        {
            return Task.Run(() => new MediaSource(uri));
        }

        public static MediaSource Create(params string[] cliOptions)
        {
            return new MediaSource(cliOptions);
        }

        public static MediaSource CreateFromUri(string uri)
        {
            return new MediaSource(uri);
        }

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
