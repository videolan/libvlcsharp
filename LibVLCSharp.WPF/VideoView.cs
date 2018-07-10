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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibVLCSharp.WPF
{
    public partial class VideoView : UserControl, LibVLCSharp.Shared.IVideoView, IDisposable
    {
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;
        private LibVLCSharp.Shared.LibVLC _libVLC;
        private double _controlWidth;
        private double _controlHeight;
        private ForegroundWindow _foreground;

        public VideoView()
        {
            ResourceDictionary res = Application.LoadComponent(new Uri("/LibVLCSharp.WPF;component/Styles/VideoView.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
            this.Style = res["VideoViewStyle"] as Style;

            _foreground = new ForegroundWindow(this);

            Shared.Core.Initialize();
            _libVLC = new Shared.LibVLC();
            _mediaPlayer = new Shared.MediaPlayer(_libVLC);

            this.SizeChanged += OnSizeChanged;

            _controlHeight = this.Height;
            _controlWidth = this.Width;

            this.Loaded += VideoView_Loaded;
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Hwnd = ((System.Windows.Forms.Panel)this.Template.FindName("PART_PlayerView", this)).Handle;

            if (this.Content != null)
            {
                object content = Content;
                Content = null;
                _foreground.Content = (UIElement)content;
            }
        }

        private void VLCResize()
        {
            uint h = 0, w = 0;
            float scale = 1, scalew = 1, scaleh = 1;

            if (_mediaPlayer.Size(0, ref w, ref h))
            {
                scalew = (float)_controlWidth / (float)w;
                w = (uint)_controlWidth;

                scaleh = (float)_controlHeight / (float)h;
                h = (uint)_controlHeight;

                scale = scalew < scaleh ? scalew : scaleh;
                _mediaPlayer.Scale = scale;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _controlWidth = e.NewSize.Width;
            _controlHeight = e.NewSize.Height;

            if (_mediaPlayer.IsPlaying)
            {
                VLCResize();
            }
        }

        public void Dispose()
        {
            if (_mediaPlayer.IsPlaying)
                _mediaPlayer.Stop();

            this.SizeChanged -= OnSizeChanged;
            this.Loaded -= VideoView_Loaded;

            _mediaPlayer.Hwnd = IntPtr.Zero;
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }

        public LibVLCSharp.Shared.MediaPlayer MediaPlayer { get => _mediaPlayer; }
        public LibVLCSharp.Shared.LibVLC LibVLC { get => _libVLC; }
    }
}
