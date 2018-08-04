using LibVLCSharp.Shared;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example2 : Window
    {
        readonly ILibVLCMediaSource _mediaSource;

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
            _mediaSource = MediaSource.CreateFromUri("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4");
            Player.Source = _mediaSource;
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaSource.MediaPlayer.IsPlaying)
            {
                _mediaSource.MediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_mediaSource.MediaPlayer.IsPlaying)
            {
                _mediaSource.MediaPlayer.Play();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _mediaSource.Dispose();
        }
    }
}