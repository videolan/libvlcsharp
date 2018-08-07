using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Source for VideoView that encapsulates the <see cref="LibVLC"/> and <see cref="MediaPlayer"/> instances />
    /// </summary>
    public class MediaSource : ISource
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MediaSource"/> class
        /// </summary>
        /// <param name="cliOptions">command line options (https://wiki.videolan.org/VLC_command-line_help/)</param>
        protected MediaSource(params string[] cliOptions)
        {
            Core.Initialize();
            LibVLC = new LibVLC(cliOptions);
            MediaPlayer = new MediaPlayer(LibVLC);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MediaSource"/> class
        /// </summary>
        /// <param name="uri">URI of the media to play</param>
        /// <param name="cliOptions">command line options (https://wiki.videolan.org/VLC_command-line_help/)</param>
        protected MediaSource(string uri, params string[] cliOptions) : this(cliOptions)
        {
            MediaPlayer.Media = new Media(LibVLC, uri, Media.FromType.FromLocation);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MediaSource()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the <see cref="MediaPlayer"/> object
        /// </summary>
        public MediaPlayer MediaPlayer { get; }

        /// <summary>
        /// Gets the <see cref="LibVLC"/> object
        /// </summary>
        public LibVLC LibVLC { get; }

#if WINDOWS
        IntPtr ISource.Hwnd
        {
            set { MediaPlayer.Hwnd = value; }
        }
#elif ANDROID
        void ISource.SetAndroidContext(IntPtr aWindow)
        {
            MediaPlayer.SetAndroidContext(aWindow);
        }
#elif COCOA
        IntPtr ISource.NsObject
        {
            set { MediaPlayer.NsObject = value; }
        }
#endif       

        /// <summary>
        /// Creates a new <see cref="MediaSource"/> instance
        /// </summary>
        /// <param name="cliOptions">command line options (https://wiki.videolan.org/VLC_command-line_help/)</param>
        /// <returns>the task representing the asynchronous operation, containing the <see cref="MediaSource"/> of the operation</returns>
        public static Task<MediaSource> CreateAsync(params string[] cliOptions)
        {
            return Task.Run(() => new MediaSource(cliOptions));
        }

        /// <summary>
        /// Creates a new <see cref="MediaSource"/> instance from an URI
        /// </summary>
        /// <param name="uri">URI of the media to play</param>
        /// <param name="cliOptions">command line options (https://wiki.videolan.org/VLC_command-line_help/)</param>
        /// <returns>the task representing the asynchronous operation, containing the <see cref="MediaSource"/> of the operation</returns>
        public static Task<MediaSource> CreateFromUriAsync(string uri, params string[] cliOptions)
        {
            return Task.Run(() => new MediaSource(uri, cliOptions));
        }

        /// <summary>
        /// Creates a new <see cref="MediaSource"/> instance
        /// </summary>
        /// <param name="cliOptions">command line options (https://wiki.videolan.org/VLC_command-line_help/)</param>
        /// <returns>a new <see cref="MediaSource"/> instance</returns>
        /// <remarks>As the instanciation of a MediaSource can be longer than expected (modules loading and native calls when initializing <see cref="LibVLC"/>), 
        /// we recommand to use the asynchronous version of this method (<see cref="CreateAsync(string[])"/>)</remarks>
        public static MediaSource Create(params string[] cliOptions)
        {
            return new MediaSource(cliOptions);
        }

        /// <summary>
        /// Creates a new <see cref="MediaSource"/> instance from an URI
        /// </summary>
        /// <param name="uri">URI of the media to play</param>
        /// <param name="cliOptions">command line options (https://wiki.videolan.org/VLC_command-line_help/)</param>
        /// <returns>a new <see cref="MediaSource"/> instance</returns>
        /// <remarks>As the instanciation of a MediaSource can be longer than expected (modules loading and native calls when initializing <see cref="LibVLC"/>), 
        /// we recommand to use the asynchronous version of this method (<see cref="CreateFromUriAsync(string, string[])"/>)</remarks>
        public static MediaSource CreateFromUri(string uri, params string[] cliOptions)
        {
            return new MediaSource(uri, cliOptions);
        }

        /// <summary>
        /// Releases the unmanaged resources
        /// </summary>
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
