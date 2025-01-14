using LibVLCSharp.Shared;

namespace LibVLCSharp.MAUI.Sample
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
#if !WINDOWS
            ((MainViewModel)BindingContext).OnAppearing();
#endif
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((MainViewModel)BindingContext).OnDisappearing();
        }

        private void VideoView_MediaPlayerChanged(object sender, MediaPlayerChangedEventArgs e)
        {
            ((MainViewModel)BindingContext).OnVideoViewInitialized();
        }

        private void VideoView_HandlerChanged(object sender, EventArgs e)
        {
#if WINDOWS
            var windowsView = ((LibVLCSharp.Platforms.Windows.VideoView)VideoView.Handler.PlatformView);

            windowsView.Initialized += (s, e) =>
            {
                ((MainViewModel)BindingContext).Initialize(e.SwapChainOptions);
                ((MainViewModel)BindingContext).OnAppearing();
            };
#endif
        }
    }
}