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
        public static MauiAppBuilder UseLibVLCSharp(this MauiAppBuilder builder) =>
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(VideoView), typeof(VideoViewHandler));
            });
    }
}
