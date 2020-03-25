using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace LibVLCSharp.WPF
{
    internal partial class ForegroundWindow : Window
    {
        Window? _wndhost;
        readonly FrameworkElement _bckgnd;
        UIElement? _content;
        readonly Point _zeroPoint = new Point(0, 0);

        internal new UIElement? Content
        {
            get => _content;
            set
            {
                _content = value;
                PART_Content.Children.Clear();
                if (_content != null)
                {
                    PART_Content.Children.Add(_content);
                }
            }
        }

        internal ForegroundWindow(FrameworkElement background)
        {
            InitializeComponent();

            DataContext = background.DataContext;

            _bckgnd = background;
            _bckgnd.DataContextChanged += Background_DataContextChanged;
            _bckgnd.Loaded += Background_Loaded;
            _bckgnd.Unloaded += Background_Unloaded;
        }

        void Background_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = e.NewValue;
        }

        void Background_Unloaded(object sender, RoutedEventArgs e)
        {
            _bckgnd.SizeChanged -= Wndhost_SizeChanged;
            if (_wndhost != null)
            {
                _wndhost.Closing -= Wndhost_Closing;
                _wndhost.LocationChanged -= Wndhost_LocationChanged;
            }

            Hide();
        }

        void Background_Loaded(object sender, RoutedEventArgs e)
        {
            _wndhost = GetWindow(_bckgnd);
            Trace.Assert(_wndhost != null);
            if (_wndhost == null)
            {
                return;
            }

            Owner = _wndhost;

            _wndhost.Closing += Wndhost_Closing;
            _bckgnd.SizeChanged += Wndhost_SizeChanged;
            _wndhost.LocationChanged += Wndhost_LocationChanged;

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
            catch
            {
                Hide();
                throw new VLCException("Unable to create WPF Window in VideoView.");
            }
        }

        void Wndhost_LocationChanged(object? sender, EventArgs e)
        {
            var locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
            var source = PresentationSource.FromVisual(_wndhost);
            var targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            Left = targetPoints.X;
            Top = targetPoints.Y;
        }

        void Wndhost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
            var source = PresentationSource.FromVisual(_wndhost);
            var targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            Left = targetPoints.X;
            Top = targetPoints.Y;
            var size = new Point(_bckgnd.ActualWidth, _bckgnd.ActualHeight);
            Height = size.Y;
            Width = size.X;
        }

        void Wndhost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close();
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
