using Windows.UI.Xaml.Controls;

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
            Border!.Child = underlyingVideoView;
            underlyingVideoView.Frame = Border.Frame;
            return underlyingVideoView;
        }
    }
}
