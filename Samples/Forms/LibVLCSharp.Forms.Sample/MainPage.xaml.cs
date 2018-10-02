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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            videoView.MediaPlayer.Play(new Media(videoView.LibVLC,
                "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
        }
    }
}