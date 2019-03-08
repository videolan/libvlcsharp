﻿using LibVLCSharp.Shared;
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

        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer),
                typeof(MediaPlayer),
                typeof(VideoView),
                new PropertyMetadata(null, OnMediaPlayerChanged));

        public MediaPlayer MediaPlayer
        {
            get { return GetValue(MediaPlayerProperty) as MediaPlayer; }
            set { SetValue(MediaPlayerProperty, value); }
        }

        private static void OnMediaPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MediaPlayer oldMediaPlayer)
            {
                oldMediaPlayer.Hwnd = IntPtr.Zero;
            }
            if (e.NewValue is MediaPlayer newMediaPlayer)
            {
                newMediaPlayer.Hwnd = ((VideoView)d).Hwnd;
            }
        }

        private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        private ForegroundWindow ForegroundWindow { get; set; }
        private bool IsUpdatingContent { get; set; }
        private UIElement ViewContent { get; set; }
        private IntPtr Hwnd { get; set; }

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

                Hwnd = (Template.FindName(PART_PlayerView, this) as System.Windows.Forms.Panel)?.Handle ?? IntPtr.Zero;
                if (Hwnd == null)
                {
                    Trace.WriteLine("HWND is NULL, aborting...");
                    return;
                }

                if (MediaPlayer == null)
                {
                    Trace.Write("No MediaPlayer is set, aborting...");
                    return;
                }

                MediaPlayer.Hwnd = Hwnd;
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
                    if (MediaPlayer != null)
                    {
                        MediaPlayer.Hwnd = IntPtr.Zero;
                    }
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