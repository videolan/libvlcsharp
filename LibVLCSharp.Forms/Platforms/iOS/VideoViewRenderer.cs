using LibVLCSharp.Shared;

using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.iOS.VideoView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if(Control == null)
            {
                SetNativeControl(new LibVLCSharp.Platforms.iOS.VideoView());
            }

            if (e.OldElement != null)
            {
                e.OldElement.MediaPlayerChanged -= OnMediaPlayerChanged;
            }

            if (e.NewElement != null)
            {
                e.NewElement.MediaPlayerChanged += OnMediaPlayerChanged;
            }    
        }

        private void OnMediaPlayerChanged(object sender, MediaPlayerChangedEventArgs e)
        {
            Control.MediaPlayer = e.NewMediaPlayer;
        }
    }
}