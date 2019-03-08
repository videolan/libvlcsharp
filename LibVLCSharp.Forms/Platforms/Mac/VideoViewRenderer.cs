using LibVLCSharp.Shared;

using LibVLCSharp.Forms.Platforms.Mac;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.Mac
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.Mac.VideoView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new LibVLCSharp.Platforms.Mac.VideoView());
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