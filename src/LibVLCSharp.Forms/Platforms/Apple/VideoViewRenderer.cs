using LibVLCSharp.Forms;

#if IOS
using Xamarin.Forms.Platform.iOS;
#elif MAC
using Xamarin.Forms.Platform.MacOS;
#endif

using Xamarin.Forms;
using Foundation;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Xamarin.Forms custom renderer for the Apple VideoView
    /// </summary>
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.VideoView, LibVLCSharp.VideoView>
    {
        /// <summary>
        /// Native control management during lifecycle events
        /// </summary>
        /// <param name="e">lifecycle event</param>
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new LibVLCSharp.VideoView());

                    e.NewElement.MediaPlayerChanging += OnMediaPlayerChanging;
                    if (Control!.MediaPlayer != e.NewElement.MediaPlayer)
                    {
                        OnMediaPlayerChanging(this, new MediaPlayerChangingEventArgs(Control!.MediaPlayer, e.NewElement.MediaPlayer));
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
