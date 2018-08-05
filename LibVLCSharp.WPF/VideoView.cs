using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace LibVLCSharp.WPF
{
    [TemplatePart(Name = PART_PlayerHost, Type = typeof(WindowsFormsHost))]
    [TemplatePart(Name = PART_PlayerView, Type = typeof(System.Windows.Forms.Panel))]
    public class VideoView : ContentControl, IVideoView, IDisposable
    {
        private const string PART_PlayerHost = "PART_PlayerHost";
        private const string PART_PlayerView = "PART_PlayerView";

        public VideoView()
        {
            DefaultStyleKey = typeof(VideoView);

            if (!IsDesignMode)
            {
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
        private ForegroundWindow ForegroundWindow { get; set; }
        private bool IsUpdatingContent { get; set; }
        private UIElement ViewContent { get; set; }

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
                var windowsFormsHost = Template.FindName(PART_PlayerHost, this) as FrameworkElement;
                if (windowsFormsHost != null)
                {
                    ForegroundWindow = new ForegroundWindow(windowsFormsHost);
                    ForegroundWindow.Content = ViewContent;
                }

                var hwnd = (Template.FindName(PART_PlayerView, this) as System.Windows.Forms.Panel)?.Handle;
                if (hwnd != null)
                {
                    MediaPlayer.Hwnd = (IntPtr)hwnd;
                }
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (IsDesignMode || IsUpdatingContent)
            {
                return;
            }

            IsUpdatingContent = true;
            try
            {
                Content = null;
            }
            finally
            {
                IsUpdatingContent = false;
            }

            ViewContent = newContent as UIElement;
            if (ForegroundWindow != null)
            {
                ForegroundWindow.Content = ViewContent;
            }
        }

        public void Dispose()
        {
            Unloaded -= VideoView_Unloaded;

            if (MediaPlayer != null)
            {
                if (MediaPlayer.IsPlaying)
                    MediaPlayer.Stop();
                MediaPlayer.Hwnd = IntPtr.Zero;
                MediaPlayer.Dispose();
                MediaPlayer = null;
            }

            if (LibVLC != null)
            {
                LibVLC.Dispose();
                LibVLC = null;
            }
        }
    }
}