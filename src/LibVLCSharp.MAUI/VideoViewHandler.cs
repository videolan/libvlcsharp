using Microsoft.Maui.Handlers;
#if ANDROID
using PlatformView = LibVLCSharp.Platforms.Android.VideoView;
#elif IOS
using PlatformView = LibVLCSharp.Platforms.iOS.VideoView;
#endif 

namespace LibVLCSharp.MAUI
{
    public partial class VideoViewHandler : ViewHandler<VideoView, PlatformView>
    {
        public static IPropertyMapper<VideoView, VideoViewHandler> PropertyMapper = new PropertyMapper<VideoView, VideoViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(VideoView.MediaPlayer)] = MapMediaPlayer
        };

        public VideoViewHandler() : base(PropertyMapper)
        {
        }

        protected override PlatformView CreatePlatformView()
        {
#if ANDROID
            return new PlatformView(Context);
#elif IOS
            return new PlatformView();
#else
            throw new NotImplementedException();
#endif
        }

        protected override void ConnectHandler(PlatformView platformView) //JKN check if the override is not a dupp from the other part of the class implementation
        {
            base.ConnectHandler(platformView);
            // Perform any control setup here
        }

        protected override void DisconnectHandler(PlatformView platformView)
        {
            base.DisconnectHandler(platformView);
            // Perform any native view cleanup here
        }

        public static void MapMediaPlayer(VideoViewHandler handler, VideoView view)
        {
            handler.PlatformView.MediaPlayer = view.MediaPlayer;
        }
    }
}
