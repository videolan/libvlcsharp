using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Represents the playback controls for a media player element
    /// </summary>
    public class PlaybackControls : PlaybackControlsBase
    {
        /// <summary>
        /// Sets the value of the <see cref="ToolTipService.ToolTipProperty"/> for an object
        /// </summary>
        /// <param name="element">the object to which the attached property is written</param>
        /// <param name="resource">resource string</param>
        /// <param name="args">an object array that contains zero or more objects to format</param>
        protected override void SetToolTip(DependencyObject? element, string? resource, params string[] args)
        {
            if (element == null || resource == null)
            {
                return;
            }
            var resourceLoader = ResourceLoader;
            if (resourceLoader != null)
            {
                ToolTipService.SetToolTip(element, string.Format(resourceLoader.GetString(resource),
                    args.Select(arg => resourceLoader.GetString(arg)).Cast<object>().ToArray()));
            }
        }
    }
}
