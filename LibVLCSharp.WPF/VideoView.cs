using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Diagnostics;
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
        }

        private void Attach()
        {
            if (!IsDesignMode)
            {
                if (MediaPlayer == null)
                {
                    Trace.Write("No MediaPlayer is set, aborting...");
                    return;
                }

                var hwnd = (Template.FindName(PART_PlayerView, this) as System.Windows.Forms.Panel)?.Handle;
                if (hwnd == null)
                {
                    Trace.WriteLine("HWND is NULL, aborting...");
                    return;
                }

                MediaPlayer.Hwnd = (IntPtr)hwnd;
            }
        }

        private void Detach()
        {
            if (!IsDesignMode)
            {
                if (MediaPlayer != null)
                {
                    MediaPlayer.Hwnd = IntPtr.Zero;
                }
            }
        }

        private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        private ForegroundWindow ForegroundWindow { get; set; }
        private bool IsUpdatingContent { get; set; }
        private UIElement ViewContent { get; set; }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                if (_mediaPlayer != value)
                {
                    Detach();
                    _mediaPlayer = value;

                    if (_mediaPlayer != null)
                    {
                        Attach();
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!IsDesignMode)
            {
                var windowsFormsHost = Template.FindName(PART_PlayerHost, this) as FrameworkElement;
                if (windowsFormsHost != null)
                {
                    ForegroundWindow = new ForegroundWindow(windowsFormsHost)
                    {
                        Content = ViewContent
                    };
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

        #region IDisposable Support

        bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Detach();
                }

                ViewContent = null;
                ForegroundWindow = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}