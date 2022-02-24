using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace LibVLCSharp.WPF
{
    /// <summary>
    /// WPF VideoView with databinding for use with LibVLCSharp
    /// </summary>
    [TemplatePart(Name = PART_PlayerHost, Type = typeof(VideoHwndHost))]
    public class VideoView : ContentControl, IVideoView, IDisposable
    {
        private const string PART_PlayerHost = "PART_PlayerHost";
        private VideoHwndHost? _videoHwndHost = null;

        /// <summary>
        /// WPF VideoView constructor
        /// </summary>
        public VideoView()
        {
            DefaultStyleKey = typeof(VideoView);
        }

        /// <summary>
        /// MediaPlayer WPF databinding property
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer),
                typeof(MediaPlayer),
                typeof(VideoView),
                new PropertyMetadata(null, OnMediaPlayerChanged));

        /// <summary>
        /// MediaPlayer property for this VideoView
        /// </summary>
        public MediaPlayer? MediaPlayer
        {
            get => GetValue(MediaPlayerProperty) as MediaPlayer;
            set => SetValue(MediaPlayerProperty, value);
        }

        private static void OnMediaPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MediaPlayer oldMediaPlayer)
            {
                ((VideoView)d).DetachMediaPlayer(oldMediaPlayer);
            }
            if (e.NewValue is MediaPlayer newMediaPlayer)
            {
                ((VideoView)d).AttachMediaPlayer(newMediaPlayer);
            }
        }

        private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        private ForegroundWindow? ForegroundWindow { get; set; }
        private bool IsUpdatingContent { get; set; }
        private UIElement? ViewContent { get; set; }

        /// <summary>
        /// ForegroundWindow management and MediaPlayer setup.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsDesignMode)
            {
                return;
            }

            if (Template.FindName(PART_PlayerHost, this) is not VideoHwndHost controlHost)
            {
                Trace.WriteLine($"Couldn't find {PART_PlayerHost} of type {nameof(VideoHwndHost)}");
                return;
            }

            _videoHwndHost = controlHost;

            ForegroundWindow = new ForegroundWindow(_videoHwndHost)
            {
                OverlayContent = ViewContent
            };


            if (_videoHwndHost.Handle == IntPtr.Zero)
            {
                Trace.WriteLine("HWND is NULL, aborting...");
                return;
            }

            if (MediaPlayer == null)
            {
                Trace.Write("No MediaPlayer is set, aborting...");
                return;
            }
            AttachMediaPlayer(MediaPlayer);
        }

        private void AttachMediaPlayer(MediaPlayer mediaPlayer)
        {
            if (mediaPlayer != null && mediaPlayer.NativeReference != IntPtr.Zero && _videoHwndHost != null)
            {
                mediaPlayer.Hwnd = _videoHwndHost.Handle;
            }
        }

        private void DetachMediaPlayer(MediaPlayer mediaPlayer)
        {
            if (mediaPlayer != null && mediaPlayer.NativeReference != IntPtr.Zero)
            {
                mediaPlayer.Hwnd = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Override to update the foreground window content
        /// </summary>
        /// <param name="oldContent">old content</param>
        /// <param name="newContent">new content</param>
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
                ForegroundWindow.OverlayContent = ViewContent;
            }
        }

        #region IDisposable Support

        bool disposedValue;
        /// <summary>
        /// Unhook mediaplayer and dispose foreground window
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (MediaPlayer != null)
                    {
                        DetachMediaPlayer(MediaPlayer);
                    }

                    _videoHwndHost?.Dispose();
                    _videoHwndHost = null;
                    ForegroundWindow?.Close();
                }

                ViewContent = null;
                ForegroundWindow = null;
                disposedValue = true;
            }
        }

        /// <summary>
        /// Unhook mediaplayer and dispose foreground window
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
