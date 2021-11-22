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
        readonly FrameworkElement _bckgnd;
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

        internal ForegroundWindow(FrameworkElement background)
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

            DataContext = background.DataContext;

            _bckgnd = background;
            _bckgnd.DataContextChanged += Background_DataContextChanged;
            _bckgnd.Loaded += Background_Loaded;
            _bckgnd.Unloaded += Background_Unloaded;
        }

        void Background_DataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = e.NewValue;
        }

        void Background_Unloaded(object? sender, RoutedEventArgs e)
        {
            _bckgnd.SizeChanged -= Wndhost_SizeChanged;
            _bckgnd.LayoutUpdated -= RefreshOverlayPosition;
            if (_wndhost != null)
            {
                _wndhost.Closing -= Wndhost_Closing;
                _wndhost.LocationChanged -= RefreshOverlayPosition;
            }

            Hide();
        }

        void Background_Loaded(object? sender, RoutedEventArgs e)
        {
            if (_wndhost != null && IsVisible)
            {
                return;
            }

            _wndhost = GetWindow(_bckgnd);
            Trace.Assert(_wndhost != null);
            if (_wndhost == null)
            {
                return;
            }

            Owner = _wndhost;

            _wndhost.Closing += Wndhost_Closing;
            _wndhost.LocationChanged += RefreshOverlayPosition;
            _bckgnd.LayoutUpdated += RefreshOverlayPosition;
            _bckgnd.SizeChanged += Wndhost_SizeChanged;

            try
            {
                var locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
                var source = PresentationSource.FromVisual(_wndhost);
                var targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
                Left = targetPoints.X;
                Top = targetPoints.Y;
                var size = new Point(_bckgnd.ActualWidth, _bckgnd.ActualHeight);
                Height = size.Y;
                Width = size.X;
                Show();
                _wndhost.Focus();
            }
            catch (Exception ex)
            {
                Hide();
                throw new VLCException("Unable to create WPF Window in VideoView.", ex);
            }
        }

        void RefreshOverlayPosition(object? sender, EventArgs e)
        {
            if (PresentationSource.FromVisual(_bckgnd) == null)
            {
                return;
            }

            var locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
            var source = PresentationSource.FromVisual(_wndhost);
            var targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            Left = targetPoints.X;
            Top = targetPoints.Y;
        }

        void Wndhost_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            var source = PresentationSource.FromVisual(_wndhost);
            if (source == null)
            {
                return;
            }

            var locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
            var targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            Left = targetPoints.X;
            Top = targetPoints.Y;
            var size = new Point(_bckgnd.ActualWidth, _bckgnd.ActualHeight);
            Height = size.Y;
            Width = size.X;
        }

        void Wndhost_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Close();

            _bckgnd.DataContextChanged -= Background_DataContextChanged;
            _bckgnd.Loaded -= Background_Loaded;
            _bckgnd.Unloaded -= Background_Unloaded;
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
