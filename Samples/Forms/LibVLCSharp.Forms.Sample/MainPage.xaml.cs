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
            BindingContext.PropertyChanged += BindingContext_PropertyChanged;
            BindingContext.Play();
        }

        private new MainViewModel BindingContext => (MainViewModel)base.BindingContext;

        private void BindingContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(BindingContext.MediaPlayer)))
                Debug.WriteLine("MediaPlayer change raised from ViewModel.Propertychanged");
        }

        private void VideoView_MediaPlayerChanged(object sender, LibVLCSharp.Shared.MediaPlayerChangedEventArgs e)
        {
            Debug.WriteLine("VideoView_MediaPlayerChanged");
            BindingContext.Play();
        }
    }
}