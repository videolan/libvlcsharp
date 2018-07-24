using LibVLCSharp.Forms.Platforms.Gtk;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.Gtk
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.Gtk.VideoView>
    {
        LibVLCSharp.Platforms.Gtk.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if(Control == null)
            {
                _videoView = new LibVLCSharp.Platforms.Gtk.VideoView(Element.CliOptions);
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