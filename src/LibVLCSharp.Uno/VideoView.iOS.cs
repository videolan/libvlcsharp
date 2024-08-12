using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : VideoViewWrapper<VideoView>
    {
        /// <summary>
        /// Creates the underlying video view and set the <see cref="Border.Child"/> property value
        /// </summary>
        /// <returns>the created underlying video view</returns>
        protected override VideoView CreateUnderlyingVideoView()
        {
            var underlyingVideoView = new VideoView();
            Border!.Child = VisualTreeHelper.AdaptNative(underlyingVideoView);
            underlyingVideoView.Frame = Border.Frame;
            return underlyingVideoView;
        }
    }
}
