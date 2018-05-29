using Android.Content;

using LibVLCSharp.Forms.Platforms.Android;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.Android
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.Android.VideoView>
    {
        LibVLCSharp.Platforms.Android.VideoView _videoView;

        public VideoViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _videoView = new LibVLCSharp.Platforms.Android.VideoView(Context, Element.CliOptions);
                SetNativeControl(_videoView);

                Element.LibVLC = Control.LibVLC;
                Element.MediaPlayer = Control.MediaPlayer;
            }

            if (e.OldElement != null)
            {
            }

            if (e.NewElement != null)
            {
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _videoView.Dispose();
        }
    }
}