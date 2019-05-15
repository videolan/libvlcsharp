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

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new LibVLCSharp.Platforms.Android.VideoView(Context));

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
            Control.TriggerLayoutChangeListener();
        }
    }
}