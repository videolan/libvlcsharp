using LibVLCSharp.Shared;

using LibVLCSharp.Forms.Platforms.iOS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    /// <summary>
    /// Xamarin.Forms custom renderer for the iOS VideoView
    /// </summary>
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.iOS.VideoView>
    {
        /// <summary>
        /// Native control management during lifecycle events
        /// </summary>
        /// <param name="e">lifecycle event</param>
        protected override void OnElementChanged(ElementChangedEventArgs<LibVLCSharp.Forms.Shared.VideoView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new LibVLCSharp.Platforms.iOS.VideoView());

                    e.NewElement.MediaPlayerChanging += OnMediaPlayerChanging;
                    if (Control.MediaPlayer != e.NewElement.MediaPlayer)
                    {
                        OnMediaPlayerChanging(this, new MediaPlayerChangingEventArgs(Control.MediaPlayer, e.NewElement.MediaPlayer));
                    }
                }
            }

            if (e.OldElement != null)
            {
                e.OldElement.MediaPlayerChanging -= OnMediaPlayerChanging;
            }
        }

        private void OnMediaPlayerChanging(object sender, MediaPlayerChangingEventArgs e)
        {
            Control.MediaPlayer = e.NewMediaPlayer;
        }
    }
}
