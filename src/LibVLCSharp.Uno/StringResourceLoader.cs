namespace LibVLCSharp.Uno
{
    internal static class StringResourceLoader
    {
        private const string ResourceMap = "LibVLCSharp.Uno/Resources";

#if WINDOWS
        private static readonly Microsoft.Windows.ApplicationModel.Resources.ResourceLoader ResourceLoader =
            new Microsoft.Windows.ApplicationModel.Resources.ResourceLoader(
                Microsoft.Windows.ApplicationModel.Resources.ResourceLoader.GetDefaultResourceFilePath(),
                ResourceMap);

        internal static string GetString(string resource) => ResourceLoader.GetString(resource);
#else
        internal static string GetString(string resource) =>
            Windows.ApplicationModel.Resources.ResourceLoader
                .GetForCurrentView(ResourceMap)
                .GetString(resource) ?? string.Empty;
#endif
    }
}
