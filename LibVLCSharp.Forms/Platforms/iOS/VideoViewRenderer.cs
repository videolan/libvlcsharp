using LibVLCSharp.Shared;

using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.iOS.VideoView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<LibVLCSharp.Forms.Shared.VideoView> e)
        {
            base.OnElementChanged(e);

            if(Control == null)
            {
                SetNativeControl(new LibVLCSharp.Forms.Platforms.iOS.VideoView());
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
            Control.MediaPlayer = e.NewMediaPlayer;
        }
    }
}