using Microsoft.Extensions.Logging;

namespace LibVLCSharp.MAUI.Sample.MediaElement
{
    /// <summary>
    /// The MauiProgram class is responsible for creating and configuring the MAUI application.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the MAUI application.
        /// </summary>
        /// <returns>The configured MAUI application.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                 {
                     fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                     fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                 })
                .UseLibVLCSharp(); // Add your custom fonts and handler from your library

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
