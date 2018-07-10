using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibVLCSharp.WPF
{
    internal partial class ForegroundWindow : Window
    {
        private Window _wndhost;
        private ContentControl _bckgnd;
        private UIElement _content;
        private readonly Point _zeroPoint = new Point(0, 0);

        internal new UIElement Content
        {
            get { return _content; }
            set
            {
                _content = value;
                if (_content != null)
                {
                    PART_Content.Children.Clear();
                    PART_Content.Children.Add(_content);
                }
            }
        }

        internal ForegroundWindow(ContentControl Background)
        {
            InitializeComponent();
            _bckgnd = Background;
            _bckgnd.Loaded += Bckgnd_Loaded;
            _bckgnd.Unloaded += Bckgnd_Unloaded;
        }

        private void Bckgnd_Unloaded(object sender, RoutedEventArgs e)
        {
            _wndhost.Closing -= Wndhost_Closing;
            _wndhost.SizeChanged -= Wndhost_SizeChanged;
            _wndhost.LocationChanged -= Wndhost_LocationChanged;
            this.Hide();
        }

        private void Bckgnd_Loaded(object sender, RoutedEventArgs e)
        {
            _wndhost = Window.GetWindow(_bckgnd);
            this.Owner = _wndhost;
            _wndhost.Closing += Wndhost_Closing;
            _wndhost.SizeChanged += Wndhost_SizeChanged;
            _wndhost.LocationChanged += Wndhost_LocationChanged;
            try
            {
                Point locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
                PresentationSource source = PresentationSource.FromVisual(_wndhost);
                System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
                this.Left = targetPoints.X;
                this.Top = targetPoints.Y;
                Vector size = _bckgnd.PointToScreen(new Point(_bckgnd.ActualWidth, _bckgnd.ActualHeight)) - _bckgnd.PointToScreen(_zeroPoint);
                this.Height = size.Y;
                this.Width = size.X;
                this.Show();
                _wndhost.Focus();
            }
            catch
            {
                this.Hide();
                throw new LibVLCSharp.Shared.VLCException("Unable to create WPF Window in VideoView.");
            }
        }

        private void Wndhost_LocationChanged(object sender, EventArgs e)
        {
            Point locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
            PresentationSource source = PresentationSource.FromVisual(_wndhost);
            System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            this.Left = targetPoints.X;
            this.Top = targetPoints.Y;
        }

        private void Wndhost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Point locationFromScreen = _bckgnd.PointToScreen(_zeroPoint);
            PresentationSource source = PresentationSource.FromVisual(_wndhost);
            System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            this.Left = targetPoints.X;
            this.Top = targetPoints.Y;
            Vector size = _bckgnd.PointToScreen(new Point(_bckgnd.ActualWidth, _bckgnd.ActualHeight)) - _bckgnd.PointToScreen(_zeroPoint);
            this.Height = size.Y;
            this.Width = size.X;
        }

        private void Wndhost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Close();
        }
    }
}
