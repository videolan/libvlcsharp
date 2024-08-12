using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : VideoViewWrapper<Platforms.iOS.VideoView>
    {
        /// <summary>
        /// Creates the underlying video view and set the <see cref="Border.Child"/> property value
        /// </summary>
        /// <returns>the created underlying video view</returns>
        protected override Platforms.iOS.VideoView CreateUnderlyingVideoView()
        {
            var underlyingVideoView = new Platforms.iOS.VideoView();
            Border!.Child = VisualTreeHelper.AdaptNative(underlyingVideoView);
            underlyingVideoView.Frame = Border.Frame;
            return underlyingVideoView;
        }
    }
}
