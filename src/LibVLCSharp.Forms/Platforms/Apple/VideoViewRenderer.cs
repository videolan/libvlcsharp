using LibVLCSharp.Shared;

#if IOS
using LibVLCSharp.Forms.Platforms.iOS;
using Xamarin.Forms.Platform.iOS;
#elif MAC
using LibVLCSharp.Forms.Platforms.Mac;
using Xamarin.Forms.Platform.MacOS;
#endif

using Xamarin.Forms;
using Foundation;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
#if IOS
namespace LibVLCSharp.Forms.Platforms.iOS
#elif MAC
namespace LibVLCSharp.Forms.Platforms.Mac
#endif
{
    /// <summary>
    /// Xamarin.Forms custom renderer for the Apple VideoView
    /// </summary>
    [Preserve(AllMembers = true)]
#if IOS
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.iOS.VideoView>
#elif MAC
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.Mac.VideoView>
#endif
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
#if IOS
                    SetNativeControl(new LibVLCSharp.Platforms.iOS.VideoView());
#elif MAC
                    SetNativeControl(new LibVLCSharp.Platforms.Mac.VideoView());
#endif
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

        private void OnMediaPlayerChanging(object? sender, MediaPlayerChangingEventArgs e)
        {
            Control.MediaPlayer = e.NewMediaPlayer;
        }
    }
}
