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

        private ImageButton AssociatedImageButton { get; set; }

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

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            BindingContext = AssociatedImageButton.BindingContext;
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
        
        private string GetSvgWithColor(string resourceName, Color color)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;

            // Ensure this matches your namespace and resource path
            //var fullyQualifiedResourceName = $"{assemblyName}.{resourceName}";
            Console.WriteLine($"Looking for resource: {resourceName}");
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine("Resource stream is null.");
                return null;
            }

            using var reader = new StreamReader(stream);
            var svgContent = reader.ReadToEnd();

            // Replace or insert the fill attribute
            var colorHex = color.ToHex();
            svgContent = ReplaceOrInsertFill(svgContent, colorHex);

            return svgContent;
        }

        private string ReplaceOrInsertFill(string svgContent, string colorHex)
        {
            const string fillAttribute = "fill=\"";
            var pathIndex = svgContent.IndexOf("<path", StringComparison.OrdinalIgnoreCase);
            if (pathIndex == -1)
                return svgContent;

            var fillIndex = svgContent.IndexOf(fillAttribute, pathIndex, StringComparison.OrdinalIgnoreCase);
            if (fillIndex != -1)
            {
                // Replace existing fill attribute
                var start = fillIndex + fillAttribute.Length;
                var end = svgContent.IndexOf("\"", start, StringComparison.OrdinalIgnoreCase);
                svgContent = svgContent.Substring(0, start) + colorHex + svgContent.Substring(end);
            }
            else
            {
                // Insert fill attribute
                var endOfPathTagIndex = svgContent.IndexOf(">", pathIndex, StringComparison.OrdinalIgnoreCase);
                if (svgContent[endOfPathTagIndex - 1] == '/')
                {
                    // Self-closing tag
                    svgContent = svgContent.Insert(endOfPathTagIndex - 1, $" {fillAttribute}{colorHex}\"");
                }
                else
                {
                    // Normal closing tag
                    svgContent = svgContent.Insert(endOfPathTagIndex, $" {fillAttribute}{colorHex}\"");
                }
            }

            return svgContent;
        }

        private ImageSource GetImageSourceFromSvg(string svgContent)
        {
            if (svgContent == null)
                return null;

            var svgStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent));
            return ImageSource.FromStream(() => svgStream);
        }
    }
}
