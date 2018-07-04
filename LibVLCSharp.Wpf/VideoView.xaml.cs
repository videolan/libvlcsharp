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

namespace LibVLCSharp.WPF
{
    public partial class VideoView : UserControl, LibVLCSharp.Shared.IVideoView, IDisposable
    {
        private LibVLCSharp.Shared.MediaPlayer mediaPlayer;
        private LibVLCSharp.Shared.LibVLC libVLC;
        private double controlWidth;
        private double controlHeight;

        public VideoView()
        {
            Shared.Core.Initialize();
            libVLC = new Shared.LibVLC();
            mediaPlayer = new Shared.MediaPlayer(libVLC);

            InitializeComponent();
            this.SizeChanged += OnSizeChanged;

            controlHeight = this.Height;
            controlWidth = this.Width;

            mediaPlayer.Hwnd = PlayerView.Handle;
        }

        private void VLCResize()
        {
            uint h = 0, w = 0;
            float scale = 1, scalew = 1, scaleh = 1;

            if (mediaPlayer.Size(0, ref w, ref h))
            {
                scalew = (float)controlWidth / (float)w;
                w = (uint)controlWidth;

                scaleh = (float)controlHeight / (float)h;
                h = (uint)controlHeight;

                scale = scalew < scaleh ? scalew : scaleh;
                mediaPlayer.Scale = scale;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            controlWidth = e.NewSize.Width;
            controlHeight = e.NewSize.Height;

            if (mediaPlayer.IsPlaying)
            {
                VLCResize();
            }
        }

        public void Dispose()
        {
            mediaPlayer.Dispose();
            libVLC.Dispose();
        }

        public LibVLCSharp.Shared.MediaPlayer MediaPlayer { get => mediaPlayer; }
        public LibVLCSharp.Shared.LibVLC LibVLC { get => libVLC; }
    }
}
