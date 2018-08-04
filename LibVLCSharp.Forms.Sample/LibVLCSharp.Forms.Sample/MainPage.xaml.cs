using LibVLCSharp.Shared;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private ILibVLCMediaSource Source { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var mediaSource = MediaSource.CreateFromUri("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4");
            Source = mediaSource;
            videoView.Source = mediaSource;
            mediaSource.MediaPlayer.Play();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Source.Dispose();
        }
    }
}