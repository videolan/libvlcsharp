using System.Diagnostics;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            videoView.MediaPlayerChanged += VideoView_MediaPlayerChanged;
            ViewModel.PropertyChanged += BindingContext_PropertyChanged;
            ViewModel.Play();
        }

        private MainViewModel ViewModel => (MainViewModel)BindingContext;

        private void BindingContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ViewModel.MediaPlayer)))
                Debug.WriteLine("MediaPlayer change raised from ViewModel.Propertychanged");
        }

        private void VideoView_MediaPlayerChanged(object sender, LibVLCSharp.Shared.MediaPlayerChangedEventArgs e)
        {
            Debug.WriteLine("VideoView_MediaPlayerChanged");
            ViewModel.Play();
        }
    }
}