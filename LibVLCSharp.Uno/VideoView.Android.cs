using Uno.UI;
using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : VideoViewWrapper<Platforms.Android.VideoView>
    {
        /// <summary>
        /// Creates the underlying video view and set the <see cref="Border.Child"/> property value
        /// </summary>
        /// <returns>the created underlying video view</returns>
        protected override Platforms.Android.VideoView CreateUnderlyingVideoView()
        {
            var underlyingVideoView = new Platforms.Android.VideoView(ContextHelper.Current);
            Border!.Child = underlyingVideoView;
            return underlyingVideoView;
        }
    }
}
