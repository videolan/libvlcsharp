namespace LibVLCSharp.MAUI
{
    /// <summary>
    /// MAUI extensions
    /// </summary>
    public static class AppHostBuilderExtensions
    {
        /// <summary>
        /// MAUI builder helper
        /// </summary>
        /// <param name="builder">MauiAppBuilder</param>
        /// <returns>configured builder for libvlcsharp</returns>
        public static MauiAppBuilder UseLibVLCSharp(this MauiAppBuilder builder)
        {
            // Register LibVLCSharp handlers
            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID || IOS || WINDOWS
                handlers.AddHandler(typeof(VideoView), typeof(VideoViewHandler));
#else
            //.net8 core has no impl, but we need to define it so that the package can be added to a .Net8 x-plat project
            // for linking purposes.  At runtime, the correct platform-specific lib will be loaded, as any Maui-app solution will 
            // also contain a platform-specific project targeting one of the supported targets (Android, iOS, WinUI),
            // or will be a shared-project.
            throw new NotImplementedException($"This exception means the current target plaftorm: {DeviceInfo.Current.Platform} is not supported or correctly initialized. VLC needs platform-specific libs loaded.");
#endif
            });

            // Configure custom fonts
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("FontAwesome5Brands.otf", "FontAwesomeBrands");
                fonts.AddFont("FontAwesome5Solid.otf", "FontAwesomeSolid");
            });

            return builder;
        }   
    }
}
