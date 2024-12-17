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

        void OnAppearing(object sender, System.EventArgs e)
        {
            base.OnAppearing();
#if !WINDOWS
            ((MainViewModel)BindingContext).OnAppearing();
#endif
        }

        void OnDisappearing(object sender, System.EventArgs e)
        {
            base.OnDisappearing();
            ((MainViewModel)BindingContext).OnDisappearing();
        }
    }
}
