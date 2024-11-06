using Microsoft.Maui.Handlers;

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VideoViewHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public static IPropertyMapper<VideoView, VideoViewHandler> PropertyMapper = new PropertyMapper<VideoView, VideoViewHandler>(ViewMapper)
        {
            [nameof(VideoView.MediaPlayer)] = MapMediaPlayer
        };

        /// <summary>
        /// 
        /// </summary>
        public VideoViewHandler() : base(PropertyMapper)
        {
        }

        /// <summary>
        /// Attach mediaplayer to the native view
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="view"></param>
        public static void MapMediaPlayer(VideoViewHandler handler, VideoView view)
        {
            handler.PlatformView.MediaPlayer = view.MediaPlayer;
        }
    }
}
