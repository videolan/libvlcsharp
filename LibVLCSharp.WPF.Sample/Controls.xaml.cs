using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Controls : UserControl
    {
        Example1 parent;

        public Controls(Example1 Parent)
        {
            parent = Parent;

            InitializeComponent();

            PlayButton.Click += PlayButton_Click;
            StopButton.Click += StopButton_Click;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (parent.Player.MediaPlayer.IsPlaying)
            {
                parent.Player.MediaPlayer.Stop();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!parent.Player.MediaPlayer.IsPlaying)
            {
                parent.Player.MediaPlayer.Play(new Media(parent.Player.LibVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
            }
        }
    }
}
