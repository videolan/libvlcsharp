using Microsoft.Maui.Controls;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    /// <summary>
    /// Represnets the Main Page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            MediaPlayerElement.PlaybackControls.VideoView.HandlerChanged += VideoView_HandlerChanged;
        }

        private void VideoView_HandlerChanged(object sender, EventArgs e)
        {
#if WINDOWS
            var windowsView = ((LibVLCSharp.Platforms.Windows.VideoView)MediaPlayerElement.PlaybackControls.VideoView.Handler.PlatformView);

            windowsView.Initialized += (s, e) =>
            {
                ((MainViewModel)BindingContext).OnAppearing(e.SwapChainOptions);
            };
#endif
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            base.OnAppearing();
#if !WINDOWS
            ((MainViewModel)BindingContext).OnAppearing();
#endif
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            base.OnDisappearing();

            // this is a hack to get the mediaplayer/libvlc objects databinding disabled before disposing of them in the viewmodel OnDisappearing function. The MediaElement control and MAUI lifecycle shutdown procedure don't play nice together, so sometimes, on destroy, media element events are being unsubscribed on an already disposed mediaplayer instance, causing a memory violation exception.
            MediaPlayerElement.MediaPlayer = null;
            MediaPlayerElement.LibVLC = null;
            ((MainViewModel)BindingContext).OnDisappearing();
        }
    }
}
