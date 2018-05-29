using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.iOS.VideoView>
    {
        LibVLCSharp.Platforms.iOS.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if(Control == null)
            {
                _videoView = new LibVLCSharp.Platforms.iOS.VideoView(Element.CliOptions);
                SetNativeControl(_videoView);
                
                Element.Instance = Control.Instance;
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