using System;
using System.IO;
using System.Reflection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LibVLCSharp.MAUI.Shared.Behaviors
{
    public class SvgTintColorBehavior : Behavior<ImageButton>
    {
        public static readonly BindableProperty TintColorProperty =
            BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SvgTintColorBehavior), Colors.Black, propertyChanged: OnTintColorChanged);

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        private static void OnTintColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SvgTintColorBehavior behavior && behavior.AssociatedImageButton != null)
            {
                behavior.ApplyTintColor();
            }
        }

        private ImageButton? AssociatedImageButton { get; set; }

        protected override void OnAttachedTo(ImageButton bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedImageButton = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            ApplyTintColor();
        }

        protected override void OnDetachingFrom(ImageButton bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            AssociatedImageButton = null;
        }

        private void OnBindingContextChanged(object? sender, EventArgs e)
        {
            if (AssociatedImageButton != null)
            {
                BindingContext = AssociatedImageButton.BindingContext;
            }
        }

        private void ApplyTintColor()
        {
            if (AssociatedImageButton == null)
                return;

            if (AssociatedImageButton.Source is FileImageSource fileImageSource)
            {
                var resourceName = fileImageSource.File;
                Console.WriteLine($"Applying tint to resource: {resourceName}");
                var svgContent = GetSvgWithColor(resourceName, TintColor);
                if (svgContent != null)
                {
                    Console.WriteLine($"Modified SVG content: {svgContent}");
                    AssociatedImageButton.Source = GetImageSourceFromSvg(svgContent);
                }
                else
                {
                    Console.WriteLine("SVG content is null.");
                }
            }
        }

        private string? GetSvgWithColor(string resourceName, Color color)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine($"Looking for resource: {resourceName}");
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine("Resource stream is null.");
                return null;
            }

            using var reader = new StreamReader(stream);
            var svgContent = reader.ReadToEnd();

            var colorHex = color.ToHex();
            svgContent = SvgTintColorBehavior.ReplaceOrInsertFill(svgContent, colorHex);

            return svgContent;
        }

        private static string ReplaceOrInsertFill(string svgContent, string colorHex)
        {
            const string fillAttribute = "fill=\"";
            var pathIndex = svgContent.IndexOf("<path", StringComparison.OrdinalIgnoreCase);
            if (pathIndex == -1)
                return svgContent;

            var fillIndex = svgContent.IndexOf(fillAttribute, pathIndex, StringComparison.OrdinalIgnoreCase);
            if (fillIndex != -1)
            {
                var start = fillIndex + fillAttribute.Length;
                var end = svgContent.IndexOf("\"", start, StringComparison.OrdinalIgnoreCase);
                svgContent = svgContent.Substring(0, start) + colorHex + svgContent.Substring(end);
            }
            else
            {
                var endOfPathTagIndex = svgContent.IndexOf(">", pathIndex, StringComparison.OrdinalIgnoreCase);
                if (svgContent[endOfPathTagIndex - 1] == '/')
                {
                    svgContent = svgContent.Insert(endOfPathTagIndex - 1, $" {fillAttribute}{colorHex}\"");
                }
                else
                {
                    svgContent = svgContent.Insert(endOfPathTagIndex, $" {fillAttribute}{colorHex}\"");
                }
            }

            return svgContent;
        }

        private static ImageSource? GetImageSourceFromSvg(string svgContent)
        {
            if (svgContent == null)
                return null;

            var svgStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent));
            return ImageSource.FromStream(() => svgStream);
        }
    }
}
