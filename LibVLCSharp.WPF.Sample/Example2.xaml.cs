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
using System.Windows.Shapes;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Example2 : Window
    {
        public Example2()
        {
            InitializeComponent();

            Label label = new Label();
            label.Content = "TEST";
            label.HorizontalAlignment = HorizontalAlignment.Right;
            label.VerticalAlignment = VerticalAlignment.Bottom;
            label.Foreground = new SolidColorBrush(Colors.Red);
            test.Children.Add(label);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.MediaPlayer.IsPlaying)
            {
                Player.MediaPlayer.Stop();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Player.MediaPlayer.IsPlaying)
            {
                Player.MediaPlayer.Play(new Media(Player.LibVLC,
                    "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
            }
        }
    }
}
