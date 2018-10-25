using LibVLCSharp.Shared;
using LibVLCSharp.Forms.Platforms.WPF;
using LibVLCSharp.Forms.Shared;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.WPF
{
    public class VideoViewRenderer : ViewRenderer<VideoView, LibVLCSharp.WPF.VideoView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new LibVLCSharp.WPF.VideoView());
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