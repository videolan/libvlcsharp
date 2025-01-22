using Microsoft.Maui.Handlers;
#if ANDROID
using VideoViewImpl = LibVLCSharp.Platforms.Android.VideoView;
#elif IOS
using VideoViewImpl = LibVLCSharp.Platforms.iOS.VideoView;
#elif WINUI
using VideoViewImpl = LibVLCSharp.Platforms.Windows.VideoView;
#else //.Net 8 (core/x-plat)
using VideoViewImpl = LibVLCSharp.Shared.IVideoView;
#endif 

namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// MAUI view handler for the VideoView
    /// </summary>
    public partial class VideoViewHandler : ViewHandler<VideoView, VideoViewImpl>
    {
        /// <inheritdoc />
        protected override void ConnectHandler(VideoViewImpl platformView) => base.ConnectHandler(platformView);

        /// <inheritdoc />
        protected override void DisconnectHandler(VideoViewImpl platformView) => base.DisconnectHandler(platformView);

        /// <inheritdoc />
        protected override VideoViewImpl CreatePlatformView()
        {
#if ANDROID
            return new VideoViewImpl(Context);
#elif IOS || WINUI
            return new VideoViewImpl();
#else
            //.net8 core has no impl, but we need to define it so that the package can be added to a .Net8 x-plat project
            // for linking purposes.  At runtime, the correct platform-specific lib will be loaded, as any Maui-app solution will 
            // also contain a platform-specific project targeting one of the supported targets (Android, iOS, WinUI),
            // or will be a shared-project.
            throw new NotImplementedException($"This exception means the current target plaftorm: {DeviceInfo.Current.Platform} is not supported or correctly initialized. VLC needs platform-specific libs loaded.");
#endif
        }
    }
}
