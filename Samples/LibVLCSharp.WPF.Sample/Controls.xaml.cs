using LibVLCSharp.Shared;

using System.Windows;
using System.Windows.Controls;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Controls : UserControl
    {
        readonly Example1 parent;
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        public Controls(Example1 Parent)
        {
            parent = Parent;

            InitializeComponent();

            Loaded += Controls_Loaded;
            PlayButton.Click += PlayButton_Click;
            StopButton.Click += StopButton_Click;
        }

        private void Controls_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            parent.VideoView.MediaPlayer = _mediaPlayer;
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (parent.VideoView.MediaPlayer.IsPlaying)
            {
                parent.VideoView.MediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!parent.VideoView.MediaPlayer.IsPlaying)
            {
                parent.VideoView.MediaPlayer.Play(new Media(_libVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
            }
        }
    }
}