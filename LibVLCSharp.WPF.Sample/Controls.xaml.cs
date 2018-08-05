using LibVLCSharp.Shared;

using System.Windows;
using System.Windows.Controls;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Controls : UserControl
    {
        readonly IMediaSource _mediaSource;

        public Controls(Example1 Parent)
        {
            InitializeComponent();

            PlayButton.Click += PlayButton_Click;
            StopButton.Click += StopButton_Click;

            _mediaSource = new MediaSource("http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4");
            Parent.Player.Source = _mediaSource;
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
    }
}