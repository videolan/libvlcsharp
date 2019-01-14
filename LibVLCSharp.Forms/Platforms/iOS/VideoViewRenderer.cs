using LibVLCSharp.Shared;

using LibVLCSharp.Forms.Platforms.iOS;
using LibVLCSharp.Forms.Shared;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Forms.Platforms.iOS
{
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Forms.Shared.VideoView, LibVLCSharp.Platforms.iOS.VideoView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<LibVLCSharp.Forms.Shared.VideoView> e)
        {
            base.OnElementChanged(e);

            if(Control == null)
            {
                SetNativeControl(new LibVLCSharp.Forms.Platforms.iOS.VideoView());
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
        }
    }

    public class VideoView : LibVLCSharp.Platforms.iOS.VideoView, IVisualElementRenderer
    {
        public VisualElement Element { get; private set; }

        public UIView NativeView => this;

        public UIViewController ViewController => ViewController;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint) => this.GetSizeRequest(widthConstraint, heightConstraint);

        public void SetElement(VisualElement element)
        {
            Element = element;

            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(null, Element));
        }

        public void SetElementSize(Size size) => Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
    }
}