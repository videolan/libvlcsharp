using System;
using LibVLCSharp.Shared;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example2 : Window
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        public Example2()
        {
            InitializeComponent();

            var label = new Label
            {
                Content = "TEST",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Foreground = new SolidColorBrush(Colors.Red)
            };
            test.Children.Add(label);

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoView.Loaded += (sender, e) => VideoView.MediaPlayer = _mediaPlayer;
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Play(new Media(_libVLC,
                    new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _mediaPlayer.Dispose();// Doing this before disposing the view ensure the video is stopped before the view is destroyed
            VideoView.Dispose();
            _libVLC.Dispose();
        }
    }
}
