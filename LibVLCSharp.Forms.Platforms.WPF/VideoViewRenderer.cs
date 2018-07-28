using LibVLCSharp.Forms.Platforms.WPF;
using LibVLCSharp.Forms.Shared;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.WPF
{
    public class VideoViewRenderer : ViewRenderer<VideoView, LibVLCSharp.WPF.VideoView>
    {
        LibVLCSharp.WPF.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            if (Control == null)
            {
                _videoView = new LibVLCSharp.WPF.VideoView();
                SetNativeControl(_videoView);
            }

            if (e.OldElement != null)
            {
            }

            if (e.NewElement != null)
            {
                UpdateMediaPlayer();
                UpdateLibVLC();
            }

            base.OnElementChanged(e);
        }

        void UpdateMediaPlayer()
        {
            Element.MediaPlayer = Control.MediaPlayer;
        }

        void UpdateLibVLC()
        {
            Element.LibVLC = Control.LibVLC;
        }
    }
}