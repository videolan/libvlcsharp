using Android.Content;

using LibVLCSharp.Forms.Platforms.Android;
using LibVLCSharp.Forms.Shared;
using LibVLCSharp.Shared;
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
                e.OldElement.SourceChanged -= OnSourceChanged;
            }

            if (e.NewElement != null)
            {
                e.NewElement.SourceChanged += OnSourceChanged;
            }
        }
        
        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            Control.Source = e.NewSource;
        }
    }
}