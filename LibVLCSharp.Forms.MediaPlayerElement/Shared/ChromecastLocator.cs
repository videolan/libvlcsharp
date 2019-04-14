using System.Collections.Generic;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Helper class to manage the cast.
    /// </summary>
    public static class ChromecastLocator
    {
        /// <summary>
        /// Finds the Chromecast renderers.
        /// </summary>
        /// <param name="libVLC">The <see cref="LibVLC"/> instance.</param>
        /// <returns>An enumerable collection of <see cref="RendererItem"/> representing the Chromecast renderers.</returns>
        public static async Task<IEnumerable<RendererItem>> FindRenderersAsync(this LibVLC libVLC)
        {
            var renderers = new List<RendererItem>();
            using (var rendererDiscover = new RendererDiscoverer(libVLC,
                Device.RuntimePlatform == Device.iOS ? "Bonjour_renderer" : "microdns_renderer"))
            {
                rendererDiscover.ItemAdded += (sender, e) => renderers.Add(e.RendererItem);
                rendererDiscover.Start();
                await Task.Delay(3000);
            }
            return renderers;
        }
    }
}
