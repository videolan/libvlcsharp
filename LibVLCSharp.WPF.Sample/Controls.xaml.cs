using LibVLCSharp.Shared;
using System.Windows;
using System.Windows.Controls;

namespace LibVLCSharp.WPF.Sample
{
    public partial class Controls : UserControl
    {
        public Controls()
        {
            InitializeComponent();
        }

        public static DependencyProperty VideoViewProperty = DependencyProperty.Register(nameof(VideoView), typeof(VideoView), typeof(Controls));
        public VideoView VideoView
        {
            get => GetValue(VideoViewProperty) as VideoView;
            set => SetValue(VideoViewProperty, value);
        }

        private LibVLC LibVLC => VideoView.LibVLC;
        private MediaPlayer MediaPlayer => VideoView.MediaPlayer;

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!MediaPlayer.IsPlaying)
            {
                MediaPlayer.Play(new Media(LibVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation));
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.IsPlaying)
            {
                MediaPlayer.Stop();
            }
        }
    }
}