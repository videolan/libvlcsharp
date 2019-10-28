using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Platforms.UWP
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : VideoViewWrapper<iOS.VideoView>
    {
        /// <summary>
        /// Creates the underlying video view and set the <see cref="Border.Child"/> property value
        /// </summary>
        /// <returns>the created underlying video view</returns>
        protected override iOS.VideoView CreateUnderlyingVideoView()
        {
            var underlyingVideoView = new iOS.VideoView();
            Border!.Child = underlyingVideoView;
            return underlyingVideoView;
        }
    }
}
