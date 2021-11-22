using LibVLCSharp.Forms.Platforms.WPF;
using LibVLCSharp.Forms.Shared;
using LibVLCSharp.Shared;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.WPF
{
    /// <summary>
    /// Xamarin.Forms renderer for the VideoView on WPF
    /// </summary>
    public class VideoViewRenderer : ViewRenderer<VideoView, LibVLCSharp.WPF.VideoView>
    {
        /// <summary>
        /// Native control management during lifecycle events
        /// </summary>
        /// <param name="e">lifecycle event</param>
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new LibVLCSharp.WPF.VideoView());
            }

            if (e.OldElement != null)
            {
                e.OldElement.MediaPlayerChanging -= OnMediaPlayerChanging;
            }

            if (e.NewElement != null)
            {
                e.NewElement.MediaPlayerChanging += OnMediaPlayerChanging;
                if (Control!.MediaPlayer != e.NewElement.MediaPlayer)
                {
                    OnMediaPlayerChanging(this, new MediaPlayerChangingEventArgs(Control.MediaPlayer, e.NewElement.MediaPlayer));
                }
            }
        }

        private void OnMediaPlayerChanging(object? sender, MediaPlayerChangingEventArgs e)
        {
            Control.MediaPlayer = e.NewMediaPlayer;
        }
    }
}
