﻿using LibVLCSharp.Forms.Platforms.GTK;
using LibVLCSharp.Forms.Shared;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.GTK
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.GTK.VideoView>
    {
        LibVLCSharp.GTK.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _videoView = new LibVLCSharp.GTK.VideoView();
                SetNativeControl(_videoView);
            }

            if (e.OldElement != null)
            {
                e.OldElement.MediaPlayerChanging -= OnMediaPlayerChanging;
            }

            if (e.NewElement != null)
            {
                e.NewElement.MediaPlayerChanging += OnMediaPlayerChanging;
                if (Control.MediaPlayer != e.NewElement.MediaPlayer)
                {
                    OnMediaPlayerChanging(this, new MediaPlayerChangingEventArgs(Control.MediaPlayer, e.NewElement.MediaPlayer));
                }
            }
        }

        private void OnMediaPlayerChanging(object sender, MediaPlayerChangingEventArgs e)
        {
            if (Control == null)
            {
                return;
            }
            Control.MediaPlayer = e.NewMediaPlayer;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _videoView?.Dispose();
        }
    }
}