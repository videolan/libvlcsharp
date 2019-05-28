using Windows.UI.Xaml.Controls;
using LibVLCSharp.Shared;

namespace LibVLCSharp.UWP.Sample
{
    /// <summary>
    /// The main page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LibVLC _libVLC;

        private MediaPlayer _mediaPlayer;

        public MainPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                _libVLC = new LibVLC(VideoView.SwapChainOptions);
                _mediaPlayer = new MediaPlayer(_libVLC);
                VideoView.MediaPlayer = _mediaPlayer;
                this._mediaPlayer.Play(new Media(_libVLC, "https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4", FromType.FromLocation));
            };

            Unloaded += (s, e) =>
            {
                VideoView.MediaPlayer = null;
                this._mediaPlayer.Dispose();
                this._libVLC.Dispose();
            };
        }
    }
}
