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
    [TemplatePart(Name = PART_Content, Type = typeof(UIElement))]
    public class VideoView : ContentControl, IVideoView, IDisposable
    {
        private const string PART_PlayerHost = "PART_PlayerHost";
        private const string PART_PlayerView = "PART_PlayerView";
        private const string PART_Content = "PART_Content";

        public VideoView()
        {
            DefaultStyleKey = typeof(VideoView);

            if (!IsInDesignMode)
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

        private bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        public MediaPlayer MediaPlayer { get; private set; }
        public LibVLC LibVLC { get; private set; }

        private void VideoView_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!IsInDesignMode)
            {
                if (Template.FindName(PART_PlayerHost, this) is FrameworkElement windowsFormsHost &&
                    Template.FindName(PART_Content, this) is UIElement content &&
                    LogicalTreeHelper.GetParent(content) is Panel parent)
                {
                    parent.Children.Remove(content);
                    new ForegroundWindow(windowsFormsHost) { Content = content };
                }

                var hwnd = (Template.FindName(PART_PlayerView, this) as System.Windows.Forms.Panel)?.Handle;
                if (hwnd != null)
                {
                    MediaPlayer.Hwnd = (IntPtr)hwnd;
                }
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