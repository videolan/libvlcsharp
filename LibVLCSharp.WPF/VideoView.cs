using System;
using System.Windows;
using System.Windows.Controls;

using LibVLCSharp.Shared;

namespace LibVLCSharp.WPF
{
    public class VideoView : UserControl, IVideoView, IDisposable
    {
        double _controlWidth;
        double _controlHeight;
        readonly ForegroundWindow _foreground;

        public VideoView()
        {
            var res = Application.LoadComponent(new Uri("/LibVLCSharp.WPF;component/Styles/VideoView.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
            Style = res["VideoViewStyle"] as Style;

            _foreground = new ForegroundWindow(this);

            Core.Initialize();

            LibVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(LibVLC);

            SizeChanged += OnSizeChanged;

            _controlHeight = Height;
            _controlWidth = Width;

            Loaded += VideoView_Loaded;
            Unloaded += VideoView_Unloaded;
        }

        void VideoView_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Hwnd = ((System.Windows.Forms.Panel)Template.FindName("PART_PlayerView", this)).Handle;

            if (Content != null)
            {
                var content = Content;
                Content = null;
                _foreground.Content = (UIElement)content;
            }
        }

        void VLCResize()
        {
            uint h = 0, w = 0;

            if (MediaPlayer.Size(0, ref w, ref h))
            {
                var scalew = (float)_controlWidth / w;

                var scaleh = (float)_controlHeight / h;

                var scale = scalew < scaleh ? scalew : scaleh;
                MediaPlayer.Scale = scale;
            }
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _controlWidth = e.NewSize.Width;
            _controlHeight = e.NewSize.Height;

            if (MediaPlayer.IsPlaying)
            {
                VLCResize();
            }
        }

        public void Dispose()
        {
            if (MediaPlayer.IsPlaying)
                MediaPlayer.Stop();

            SizeChanged -= OnSizeChanged;
            Loaded -= VideoView_Loaded;
            Unloaded -= VideoView_Unloaded;

            MediaPlayer.Hwnd = IntPtr.Zero;
            MediaPlayer.Dispose();
            LibVLC.Dispose();
        }

        public MediaPlayer MediaPlayer { get; }
        public LibVLC LibVLC { get; }
    }
}