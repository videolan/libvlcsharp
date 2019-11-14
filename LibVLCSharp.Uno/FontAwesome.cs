using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Helper class to create Font Awesome icons
    /// </summary>
    internal static class FontAwesome
    {
        private static FontFamily Solid { get; } =
            new FontFamily("ms-appx:///Assets/Fonts/Font Awesome 5 Free-Solid-900.otf#Font Awesome 5 Free");
        private static FontFamily Brands { get; } =
            new FontFamily("ms-appx:///Assets/Fonts/Font Awesome 5 Brands-Regular-400.otf#Font Awesome 5 Brands");

        /// <summary>
        /// Creates a solid font icon
        /// </summary>
        /// <param name="glyph">glyph</param>
        /// <returns><see cref="FontIcon"/></returns>
        public static FontIcon CreateSolidFontIcon(string glyph)
        {
            return new FontIcon { FontFamily = Solid, Glyph = glyph };
        }

        /// <summary>
        /// Creates a brands font icon
        /// </summary>
        /// <param name="glyph">glyph</param>
        /// <returns><see cref="FontIcon"/></returns>
        public static FontIcon CreateBrandsFontIcon(string glyph)
        {
            return new FontIcon { FontFamily = Brands, Glyph = glyph };
        }
    }
}
