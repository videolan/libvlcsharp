using LibVLCSharp.Shared;

using System.Windows;
using System.Windows.Controls;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Controls : UserControl
    {
        readonly Example1 parent;

        public Controls(Example1 Parent)
        {
            parent = Parent;

            InitializeComponent();

            PlayButton.Click += PlayButton_Click;
            StopButton.Click += StopButton_Click;
        }

        void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (parent.Player.MediaPlayer.IsPlaying)
            {
                parent.Player.MediaPlayer.Stop();
            }
        }

        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!parent.Player.MediaPlayer.IsPlaying)
            {
                parent.Player.MediaPlayer.Play(new Media(parent.Player.LibVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
            }
        }
    }
}