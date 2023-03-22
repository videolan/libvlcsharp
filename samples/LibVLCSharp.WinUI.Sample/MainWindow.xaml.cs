using LibVLCSharp.Platforms.Windows;
using LibVLCSharp.Shared;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibVLCSharp.WinUI.Sample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        LibVLC libvlc;
        MediaPlayer mp;

        public MainWindow()
        {
            InitializeComponent();
            VideoView.Initialized += VideoView_Initialized;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            mp.Stopped += (s, e) =>
            {
                grid.Children.Remove(VideoView);
            };
            mp.Stop();
        }
        private void VideoView_Initialized(object sender, InitializedEventArgs e)
        {
            libvlc = new LibVLC(enableDebugLogs: true, e.SwapChainOptions);
            mp = new MediaPlayer(libvlc);
            using var media = new Media(libvlc, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
            mp.Play(media);
        }
    }
}
