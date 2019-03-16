using System.Linq;
using Xamarin.Forms;

namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Provides utility methods that can be used to traverse object relationships (along child-object or parent-object axes) 
    /// in the visual tree of your app.
    /// </summary>
    public static class VisualTreeHelper
    {
        /// <summary>
        /// Using the provided name, obtains a specific child object of the provided parent element by examining the visual tree.
        /// </summary>
        /// <typeparam name="T">The element type to obtain.</typeparam>
        /// <param name="parent">The parent element.</param>
        /// <param name="name">The searched name</param>
        /// <returns>The child element, or null if not found.</returns>
        public static T FindChild<T>(this Element parent, string name) where T : Element
        {
            var result = parent.FindByName<T>(name);
            if (result != null)
            {
                return result;
            }

            if (parent is ContentView contentView)
            {
                result = contentView.Content?.FindChild<T>(name);
                if (result != null)
                {
                    return result;
                }
            }
            else
            {
                if (parent is Layout layout)
                {
                    foreach (var child in layout.Children.OfType<Layout>())
                    {
                        result = child.FindChild<T>(name);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the page to which an element belongs.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The page, or null if not found.</returns>
        public static Page GetParentPage(this VisualElement element)
        {
            if (element != null)
            {
                var parent = element.Parent;
                while (parent != null)
                {
                    if (parent is Page page)
                    {
                        return page;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }
    }
}
