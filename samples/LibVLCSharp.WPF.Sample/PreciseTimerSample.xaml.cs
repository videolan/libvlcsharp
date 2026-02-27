using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Helpers;
using System;
using System.Windows;

namespace LibVLCSharp.WPF.Sample
{
    public partial class PreciseTimerSample : Window
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private PreciseMediaTimer? _preciseTimer;

        public PreciseTimerSample()
        {
            InitializeComponent();

            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            VideoView.MediaPlayer = _mediaPlayer;
        }

        private void PreciseTimerClassTest_Click(object sender, RoutedEventArgs e)
        {
            var media = new Media(_libVLC, new Uri("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/dash/ForBiggerEscapesVideo.mp4"));
            _mediaPlayer.Play(media);

            _preciseTimer?.Dispose();
            _preciseTimer = new PreciseMediaTimer(_mediaPlayer, _libVLC);

            _preciseTimer.PrecisePositionChanged += pos =>
            {
                Dispatcher.Invoke(() =>
                {
                    SmoothProgress.Value = pos;
                });
            };

            _preciseTimer.Start();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _preciseTimer?.Dispose();
            _preciseTimer = null;

            _mediaPlayer?.Stop();
            _mediaPlayer?.Dispose();

            _libVLC?.Dispose();
        }

    }
}