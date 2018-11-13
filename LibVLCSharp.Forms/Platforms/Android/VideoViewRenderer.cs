using Android.Content;

using LibVLCSharp.Shared;
using LibVLCSharp.Forms.Platforms.Android;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.Android
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.Android.VideoView>
    {
        public VideoViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new LibVLCSharp.Platforms.Android.VideoView(Context));
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
            Control.TriggerLayoutChangeListener();
        }
    }
}