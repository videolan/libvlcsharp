using Uno.UI;

namespace LibVLCSharp.Platforms.UWP
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView
    {
        private Android.VideoView? UnderlyingVideoView
        {
            get;
            set;
        }

        private Android.VideoView CreateVideoView()
        {
            return new Android.VideoView(ContextHelper.Current);
        }
    }
}
