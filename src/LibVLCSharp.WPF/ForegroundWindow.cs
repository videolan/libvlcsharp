using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LibVLCSharp.WPF
{
    internal class ForegroundWindow : Window
    {
        Window? _wndhost;
        readonly FrameworkElement _videoHost;
        readonly FrameworkElement _backgroundElement;
        readonly Point _zeroPoint = new Point(0, 0);
        private readonly Grid _grid = new Grid();

        UIElement? _overlayContent;
        internal UIElement? OverlayContent
        {
            get => _overlayContent;
            set
            {
                _overlayContent = value;
                _grid.Children.Clear();
                if (_overlayContent != null)
                {
                    _grid.Children.Add(_overlayContent);
                }
            }
        }

        internal ForegroundWindow(FrameworkElement videoHost, FrameworkElement? background)
        {
            Title = "LibVLCSharp.WPF";
            Height = 300;
            Width = 300;
            WindowStyle = WindowStyle.None;
            Background = Brushes.Transparent;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            Content = _grid;

            DataContext = videoHost.DataContext;

            if (background == null)
                background = videoHost;

            _backgroundElement = background;
            _videoHost = videoHost;
            _videoHost.DataContextChanged += VideoHost_DataContextChanged;
            _videoHost.Loaded += VideoHost_Loaded;
            _videoHost.Unloaded += VideoHost_Unloaded;
        }

        void VideoHost_DataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = e.NewValue;
        }

        void VideoHost_Unloaded(object? sender, RoutedEventArgs e)
        {
            _backgroundElement.SizeChanged -= Bckgnd_SizeChanged;
            _backgroundElement.LayoutUpdated -= Bckgnd_LayoutUpdated;
            if (_wndhost != null)
            {
                _wndhost.Closing -= Wndhost_Closing;
                _wndhost.LocationChanged -= Wndhost_LocationChanged;
            }

            Hide();
        }

        void VideoHost_Loaded(object? sender, RoutedEventArgs e)
        {
            if (_wndhost != null && IsVisible)
            {
                return;
            }

            _wndhost = GetWindow(_videoHost);
            Trace.Assert(_wndhost != null);
            if (_wndhost == null)
            {
                return;
            }

            Owner = _wndhost;

            _wndhost.Closing += Wndhost_Closing;
            _wndhost.LocationChanged += Wndhost_LocationChanged;
            _backgroundElement.LayoutUpdated += Bckgnd_LayoutUpdated;
            _backgroundElement.SizeChanged += Bckgnd_SizeChanged;

            try
            {
                AlignWithBackground();
                Show();
                _wndhost.Focus();
            }
            catch (Exception ex)
            {
                Hide();
                throw new VLCException("Unable to create WPF Window in VideoView.", ex);
            }
        }

        void Bckgnd_LayoutUpdated(object? sender, EventArgs e)
        {
            AlignWithBackground();
        }

        void Wndhost_LocationChanged(object? sender, EventArgs e)
        {
            AlignWithBackground();
        }

        void Bckgnd_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            AlignWithBackground();
        }

        void AlignWithBackground()
        {
            if (_wndhost == null)
            {
                return;
            }

            var source = PresentationSource.FromVisual(_wndhost);

            if (source == null)
            {
                return;
            }

            if (PresentationSource.FromVisual(_backgroundElement) == null)
            {
                return;
            }

            if (double.IsNaN(_backgroundElement.ActualWidth) || double.IsNaN(_backgroundElement.ActualHeight) ||
                _backgroundElement.ActualWidth == 0 || _backgroundElement.ActualHeight == 0)
            {
                return;
            }

            /*
             * Note that _bckgnd.ActualWidth and _bckgnd.ActualWidth are in local coordinates
             * and not in screen coordinates.
             *
             * Sizes in local and screen coordinates differ when a scale transformation has
             * been applied to _bckgnd or one of its ancestors up to the parent window.
             * This is also the case when the video or one of its ancestors is contained
             * in a Viewbox.
             *
             * To correctly position and size the ForegroundWindow, we transform the extent
             * of _bckgnd (top-left and bottom-right points) from _bckgnd coordinates
             * to screen coordinates and use them as the extents of ForegroundWindow.
             *
             * In case a scaling is detected, we should also apply that scaling to the
             * content of ForegroundWindow, so it naturally scales up and down with the
             * rest of the parent controls.
             *
             * The video view itself natively supports scaling.
             */

            var startLocationFromScreen = _backgroundElement.PointToScreen(_zeroPoint);
            var startLocationPoint = source.CompositionTarget.TransformFromDevice.Transform(startLocationFromScreen);
            var endLocationFromScreen = _backgroundElement.PointToScreen(new Point(_backgroundElement.ActualWidth, _backgroundElement.ActualHeight));
            var endLocationPoint = source.CompositionTarget.TransformFromDevice.Transform(endLocationFromScreen);

            Left = Math.Min(startLocationPoint.X, endLocationPoint.X);
            Top = Math.Min(startLocationPoint.Y, endLocationPoint.Y);
            Width = Math.Abs(endLocationPoint.X - startLocationPoint.X);
            Height = Math.Abs(endLocationPoint.Y - startLocationPoint.Y);

            if (Math.Abs(Width - _backgroundElement.ActualWidth) + Math.Abs(Height - _backgroundElement.ActualHeight) > 0.5)
            {
                ScaleWindowContent(Width / _backgroundElement.ActualWidth, Height / _backgroundElement.ActualHeight);
            }
        }

        void ScaleWindowContent(double scaleX, double scaleY)
        {
            if (VisualChildrenCount <= 0)
            {
                return;
            }

            var child = (FrameworkElement)GetVisualChild(0)!;

            // Do not re-create and apply the ScaleTransform if already scaled
            // That would lead to an infinite layout update cycle
            if (child.LayoutTransform is ScaleTransform scaleTransform && 
                Math.Abs(scaleTransform.ScaleX - scaleX) < 0.01 &&
                Math.Abs(scaleTransform.ScaleY - scaleY) < 0.01)
            {
                return;
            }

            child.LayoutTransform = new ScaleTransform(scaleX, scaleY);
        }

        void Wndhost_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            Close();

            _videoHost.DataContextChanged -= VideoHost_DataContextChanged;
            _videoHost.Loaded -= VideoHost_Loaded;
            _videoHost.Unloaded -= VideoHost_Unloaded;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                _wndhost?.Focus();
            }
        }
    }
}
