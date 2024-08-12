using Uno.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : VideoViewWrapper<LibVLCSharp.VideoView>
    {
        /// <summary>
        /// Creates the underlying video view and set the <see cref="Border.Child"/> property value
        /// </summary>
        /// <returns>the created underlying video view</returns>
        protected override LibVLCSharp.VideoView CreateUnderlyingVideoView()
        {
            var underlyingVideoView = new LibVLCSharp.VideoView(ContextHelper.Current);
            Border!.Child = VisualTreeHelper.AdaptNative(underlyingVideoView);
            return underlyingVideoView;
        }
    }
}
