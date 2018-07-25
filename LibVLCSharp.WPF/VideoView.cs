using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LibVLCSharp.WPF
{
    [TemplatePart(Name = PART_PlayerView, Type = typeof(System.Windows.Forms.Panel))]
    public class VideoView : ContentControl, IVideoView, IDisposable
    {
        private const string PART_PlayerView = "PART_PlayerView";

        static VideoView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoView), new FrameworkPropertyMetadata(typeof(VideoView)));
        }

        public VideoView()
        {
            var res = (ResourceDictionary)Application.LoadComponent(new Uri("/LibVLCSharp.WPF;component/Styles/VideoView.xaml", UriKind.RelativeOrAbsolute));
            Style = res["VideoViewStyle"] as Style;

            if (!IsDesignMode)
            {
                ForegroundWindow = new ForegroundWindow(this);

                Core.Initialize();

                LibVLC = new LibVLC();
                MediaPlayer = new MediaPlayer(LibVLC);

                Unloaded += VideoView_Unloaded;
            }
        }

        ~VideoView()
        {
            Dispose();
        }

        private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        private ForegroundWindow ForegroundWindow { get; }
        private bool IsContentUpdating { get; set; }

        public MediaPlayer MediaPlayer { get; private set; }
        public LibVLC LibVLC { get; private set; }

        private void VideoView_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!IsDesignMode)
            {
                var hwnd = (Template.FindName(PART_PlayerView, this) as System.Windows.Forms.Panel)?.Handle;
                if (hwnd != null)
                {
                    MediaPlayer.Hwnd = (IntPtr)hwnd;
                }
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (!IsDesignMode)
            {
                if (IsContentUpdating)
                {
                    return;
                }

                IsContentUpdating = true;
                try
                {
                    Content = null;
                }
                finally
                {
                    IsContentUpdating = false;
                }
                ForegroundWindow.Content = newContent as UIElement;
            }

            base.OnContentChanged(oldContent, newContent);
        }

        public void Dispose()
        {
            Unloaded -= VideoView_Unloaded;

            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                if (mediaPlayer.IsPlaying)
                    mediaPlayer.Stop();
                mediaPlayer.Hwnd = IntPtr.Zero;
                mediaPlayer.Dispose();
                MediaPlayer = null;
            }

            var libVLC = LibVLC;
            if (libVLC != null)
            {
                libVLC.Dispose();
                LibVLC = null;
            }
        }
    }
}