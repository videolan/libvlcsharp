using Uno.UI;
using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Platforms.UWP
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : VideoViewWrapper<Android.VideoView>
    {
        /// <summary>
        /// Creates the underlying video view and set the <see cref="Border.Child"/> property value
        /// </summary>
        /// <returns>the created underlying video view</returns>
        protected override Android.VideoView CreateUnderlyingVideoView()
        {
            var underlyingVideoView = new Android.VideoView(ContextHelper.Current);
            Border!.Child = underlyingVideoView;
            return underlyingVideoView;
        }
    }
}
