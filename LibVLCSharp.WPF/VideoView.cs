using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace LibVLCSharp.WPF
{
    [TemplatePart(Name = PART_PlayerHost, Type = typeof(WindowsFormsHost))]
    [TemplatePart(Name = PART_PlayerView, Type = typeof(System.Windows.Forms.Panel))]
    public class VideoView : ContentControl
    {
        private const string PART_PlayerHost = "PART_PlayerHost";
        private const string PART_PlayerView = "PART_PlayerView";

        public VideoView()
        {
            DefaultStyleKey = typeof(VideoView);
        }

        private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        private ForegroundWindow ForegroundWindow { get; set; }
        private IntPtr Hwnd { get; set; }
        private bool IsUpdatingContent { get; set; }
        private UIElement ViewContent { get; set; }

        public static DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(ISource), typeof(VideoView),
            new PropertyMetadata(null, OnSourceChanged));
        public ISource Source
        {
            get { return GetValue(SourceProperty) as ISource; }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ISource oldSource)
            {
                oldSource.SetWindowHandle(IntPtr.Zero);
            }
            if (e.NewValue is ISource newSource)
            {
                newSource.SetWindowHandle(((VideoView)d).Hwnd);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!IsDesignMode)
            {
                var windowsFormsHost = Template.FindName(PART_PlayerHost, this) as FrameworkElement;
                if (windowsFormsHost != null)
                {
                    ForegroundWindow = new ForegroundWindow(windowsFormsHost);
                    ForegroundWindow.Content = ViewContent;
                }

                Hwnd = (Template.FindName(PART_PlayerView, this) as System.Windows.Forms.Panel)?.Handle ?? IntPtr.Zero;
                Source?.SetWindowHandle(Hwnd);
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (IsDesignMode || IsUpdatingContent)
            {
                return;
            }

            IsUpdatingContent = true;
            try
            {
                Content = null;
            }
            finally
            {
                IsUpdatingContent = false;
            }

            ViewContent = newContent as UIElement;
            if (ForegroundWindow != null)
            {
                ForegroundWindow.Content = ViewContent;
            }
        }
    }
}