using System.Linq;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Provides utility methods that can be used to traverse object relationships (along child-object or parent-object axes) 
    /// in the visual tree of your app.
    /// </summary>
    internal static class VisualTreeHelper
    {
        /// <summary>
        /// Obtains a child object of a given type of the provided parent element by examining the visual tree.
        /// </summary>
        /// <typeparam name="T">Element type to find.</typeparam>
        /// <param name="parent">The parent element.</param>
        /// <returns>The child element, or null if not found.</returns>
        internal static T FindChild<T>(this Element parent) where T : Element?
        {
            if (parent is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    if (child is T result)
                    {
                        return result;
                    }
                    var childResult = child.FindChild<T>();
                    if (childResult != null)
                    {
                        return childResult;
                    }
                }
            }

            return default!;
        }

        /// <summary>
        /// Using the provided name, obtains a specific child object of the provided parent element by examining the visual tree.
        /// </summary>
        /// <typeparam name="T">The element type to obtain.</typeparam>
        /// <param name="parent">The parent element.</param>
        /// <param name="name">The searched name</param>
        /// <returns>The child element, or null if not found.</returns>
        internal static T FindChild<T>(this Element parent, string name) where T : Element?
        {
            var result = parent.FindByName<T>(name);
            if (result != null)
            {
                return result;
            }

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

            return default!;
        }

        /// <summary>
        /// Gets the ancestor of a given type to which an element belongs.
        /// </summary>
        /// <typeparam name="T">Ancestor element type.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>The ancestor element of the given type, or null if not found.</returns>
        internal static T FindAncestor<T>(this Element element) where T : Element?
        {
            if (element != null)
            {
                element = element.Parent;
                while (element != null)
                {
                    if (element is T ancestor)
                    {
                        return ancestor;
                    }
                    element = element.Parent;
                }
            }
            return default!;
        }
    }
}
