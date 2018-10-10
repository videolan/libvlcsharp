using LibVLCSharp.Forms.Platforms.Mac;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.Mac
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.Mac.VideoView>
    {
        LibVLCSharp.Platforms.Mac.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _videoView = new LibVLCSharp.Platforms.Mac.VideoView(Element.CliOptions);
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