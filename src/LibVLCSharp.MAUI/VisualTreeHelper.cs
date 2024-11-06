using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace LibVLCSharp.MAUI
{
    internal static class VisualTreeHelper
    {
        internal static T? FindChild<T>(this IElement parent) where T : IElement
        {
            foreach (var child in GetVisualChildren(parent))
            {
                if (child is T result)
                {
                    return result;
                }

                var childResult = FindChild<T>(child);
                if (childResult != null)
                {
                    return childResult;
                }
            }
            return default;
        }

        internal static T? FindChild<T>(this IElement parent, string name) where T : IElement
        {
            foreach (var child in GetVisualChildren(parent))
            {
                if (child is Element element && element.StyleId == name && child is T result)
                {
                    return result;
                }

                var childResult = FindChild<T>(child, name);
                if (childResult != null)
                {
                    return childResult;
                }
            }
            return default;
        }

        internal static T? FindAncestor<T>(this IElement? element) where T : IElement
        {
            while (element != null)
            {
                element = element.Parent;
                if (element is T ancestor)
                {
                    return ancestor;
                }
            }
            return default;
        }

        private static IEnumerable<IElement> GetVisualChildren(IElement parent)
        {
            if (parent is IContentView contentView && contentView.PresentedContent is IElement contentElement)
            {
                yield return contentElement;
            }

            if (parent is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    yield return child;
                }
            }

            if (parent is IVisualTreeElement visualTreeElement)
            {
                foreach (var child in visualTreeElement.GetVisualChildren())
                {
                    if (child is IElement elementChild)
                    {
                        yield return elementChild;
                    }
                }
            }
        }
    }
}
