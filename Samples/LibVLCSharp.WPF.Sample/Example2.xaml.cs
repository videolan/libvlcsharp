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

            Loaded += Controls_Loaded;
        }

        private void Controls_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            VideoView.MediaPlayer = _mediaPlayer;
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoView.MediaPlayer.IsPlaying)
            {
                VideoView.MediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoView.MediaPlayer.IsPlaying)
            {
                VideoView.MediaPlayer.Play(new Media(_libVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
            }
        }
    }
}