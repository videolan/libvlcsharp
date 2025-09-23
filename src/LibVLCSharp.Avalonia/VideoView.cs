using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace LibVLCSharp.Avalonia
{
    /// <summary>
    /// Avalonia VideoView for Windows, Linux and Mac.
    /// </summary>
    public class VideoView : NativeControlHost
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        private IPlatformHandle? _platformHandle = null;
        private MediaPlayer? _mediaPlayer = null;
        private Window? _floatingContent = null;
        IDisposable? contentChangedHandler = null;
        IDisposable? isVisibleChangedHandler = null;
        IDisposable? floatingContentChangedHandler = null;

        /// <summary>
        /// MediaPlayer Data Bound property
        /// </summary>
        /// <summary>
        /// Defines the <see cref="MediaPlayer"/> property.
        /// </summary>
        public static readonly DirectProperty<VideoView, MediaPlayer?> MediaPlayerProperty =
            AvaloniaProperty.RegisterDirect<VideoView, MediaPlayer?>(
                nameof(MediaPlayer),
                o => o.MediaPlayer,
                (o, v) => o.MediaPlayer = v,
                defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Defines the <see cref="Content"/> property.
        /// </summary>
        public static readonly StyledProperty<object?> ContentProperty =
            AvaloniaProperty.Register<VideoView, object?>(nameof(Content));

        /// <summary>
        /// Gets or sets the MediaPlayer that will be displayed.
        /// </summary>
        public MediaPlayer? MediaPlayer
        {
            get { return _mediaPlayer; }
            set
            {
                if (ReferenceEquals(_mediaPlayer, value))
                {
                    return;
                }

                Detach();
                _mediaPlayer = value;
                Attach();
            }
        }

        /// <summary>
        /// Gets or sets the content to display.
        /// </summary>
        [Content]
        public object? Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <inheritdoc />
        public VideoView()
        {
            Initialized += (_, _) => { Attach(); };

            contentChangedHandler = ContentProperty.Changed.AddClassHandler<VideoView>((s, _) => s.UpdateOverlayPosition());
            isVisibleChangedHandler = IsVisibleProperty.Changed.AddClassHandler<VideoView>((s, _) => s.ShowNativeOverlay(s.IsVisible));
        }

        private void UpdateOverlayPosition()
        {
            if (_floatingContent == null || !IsVisible)
            {
                return;
            }
            bool forceSetWidth = false, forceSetHeight = false;
            var topLeft = new Point();
            var child = _floatingContent.Presenter?.Child;

            if (child?.IsArrangeValid == true)
            {
                switch (child.HorizontalAlignment)
                {
                    case HorizontalAlignment.Right:
                        topLeft = topLeft.WithX(Bounds.Width - _floatingContent.Bounds.Width);
                        break;
                    case HorizontalAlignment.Center:
                        topLeft = topLeft.WithX((Bounds.Width - _floatingContent.Bounds.Width) / 2);
                        break;
                    case HorizontalAlignment.Stretch:
                        forceSetWidth = true;
                        break;
                    case HorizontalAlignment.Left:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (child.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        topLeft = topLeft.WithY(Bounds.Height - _floatingContent.Bounds.Height);
                        break;
                    case VerticalAlignment.Center:
                        topLeft = topLeft.WithY((Bounds.Height - _floatingContent.Bounds.Height) / 2);
                        break;
                    case VerticalAlignment.Stretch:
                        forceSetHeight = true;
                        break;
                    case VerticalAlignment.Top:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (forceSetWidth && forceSetHeight)
                _floatingContent.SizeToContent = SizeToContent.Manual;
            else if (forceSetHeight)
                _floatingContent.SizeToContent = SizeToContent.Width;
            else if (forceSetWidth)
                _floatingContent.SizeToContent = SizeToContent.Height;
            else
                _floatingContent.SizeToContent = SizeToContent.Manual;

            _floatingContent.Width = forceSetWidth ? Bounds.Width : double.NaN;
            _floatingContent.Height = forceSetHeight ? Bounds.Height : double.NaN;

            _floatingContent.MaxWidth = Bounds.Width;
            _floatingContent.MaxHeight = Bounds.Height;

            var newPosition = this.PointToScreen(topLeft);

            if (newPosition != _floatingContent.Position)
            {
                _floatingContent.Position = newPosition;
            }
            
            if (_floatingContent.Content is Visual content && VisualRoot is Visual root && this is Visual videoView && child != null)
            {
                content.Clip = GetVisibleRegionAsGeometry(root, videoView, child.Margin);
            }
        }
        
        private static RectangleGeometry? GetVisibleRegionAsGeometry(Visual parent, Visual child, Thickness childMargin)
        {
            var childPosition = child.TranslatePoint(new Point(0, 0), parent);

            if (!childPosition.HasValue) return null;
        
            var topDistance = childPosition.Value.Y + childMargin.Top;
            var leftDistance = childPosition.Value.X + childMargin.Left;
            var bottomDistance = parent.Bounds.Height - (childPosition.Value.Y + child.Bounds.Height + childMargin.Bottom);
            var rightDistance = parent.Bounds.Width - (childPosition.Value.X + child.Bounds.Width + childMargin.Right);
        
            var region = new Rect(0, 0, child.Bounds.Width, child.Bounds.Height);
        
            if (topDistance < 0)
            {
                region = new Rect(region.X, region.Y - topDistance, region.Width, region.Height + topDistance);
            }
        
            if (leftDistance < 0)
            {
                region = new Rect(region.X - leftDistance, region.Y, region.Width + leftDistance, region.Height);
            }
        
            if (rightDistance < 0)
            {
                region = region.WithWidth(region.Width + rightDistance);
            }
        
            if (bottomDistance < 0)
            {
                region = region.WithHeight(region.Height + bottomDistance);
            }
        
            return new RectangleGeometry(region);
        }

        private void Attach()
        {
            if (_mediaPlayer == null || _platformHandle == null || !IsInitialized)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mediaPlayer.Hwnd = _platformHandle.Handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _mediaPlayer.XWindow = (uint)_platformHandle.Handle;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _mediaPlayer.NsObject = _platformHandle.Handle;
            }
        }

        private void Detach()
        {
            if (_mediaPlayer == null)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mediaPlayer.Hwnd = IntPtr.Zero;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _mediaPlayer.XWindow = 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _mediaPlayer.NsObject = IntPtr.Zero;
            }
        }

        private void InitializeNativeOverlay()
        {
            if (!this.IsAttachedToVisualTree())
                return;

            if (VisualRoot is not Window visualRoot)
            {
                return;
            }

            if (_floatingContent == null && Content != null)
            {
                _floatingContent = new Window()
                {
                    SystemDecorations = SystemDecorations.None,
                    TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent },
                    Background = Brushes.Transparent,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    CanResize = false,
                    ShowInTaskbar = false,
                    ZIndex = int.MaxValue,
                    Opacity = 1.0,
                    DataContext = DataContext
                };
                floatingContentChangedHandler = _floatingContent.Bind(ContentControl.ContentProperty, this.GetObservable(ContentProperty));
                _floatingContent.PointerEntered += FloatingContentOnPointerEvent;
                _floatingContent.PointerExited += FloatingContentOnPointerEvent;
                _floatingContent.PointerPressed += FloatingContentOnPointerEvent;
                _floatingContent.PointerReleased += FloatingContentOnPointerEvent;

                visualRoot.LayoutUpdated += VisualRoot_UpdateOverlayPosition;
                visualRoot.PositionChanged += VisualRoot_UpdateOverlayPosition;
            }

            ShowNativeOverlay(IsEffectivelyVisible);
        }

        private void VisualRoot_UpdateOverlayPosition(object sender, EventArgs e) => UpdateOverlayPosition();

        private void FloatingContentOnPointerEvent(object? sender, PointerEventArgs e)
        {
            RaiseEvent(e);
        }

        private void ShowNativeOverlay(bool show)
        {
            if (_floatingContent == null || _floatingContent.IsVisible == show || VisualRoot is not Window visualRoot)
                return;

            if (show && this.IsAttachedToVisualTree())
                _floatingContent.Show(visualRoot);
            else
                _floatingContent.Hide();
        }

        /// <inheritdoc />
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            var parent = this.GetVisualParent();
            if(parent != null)
                parent.DetachedFromVisualTree += Parent_DetachedFromVisualTree;
            base.OnAttachedToVisualTree(e);
            InitializeNativeOverlay();
        }

        private void Parent_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (VisualRoot is not Window visualRoot)
            {
                return;
            }

            visualRoot.LayoutUpdated -= VisualRoot_UpdateOverlayPosition;
            visualRoot.PositionChanged -= VisualRoot_UpdateOverlayPosition;
        }

        /// <inheritdoc />
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            var parent = this.GetVisualParent();
            if (parent != null)
                parent.DetachedFromVisualTree -= Parent_DetachedFromVisualTree;
            base.OnDetachedFromVisualTree(e);
            ShowNativeOverlay(false);
        }

        /// <inheritdoc />
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            _platformHandle = base.CreateNativeControlCore(parent);

            if (_platformHandle.Handle != IntPtr.Zero && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                EnsureClipChildren(_platformHandle.Handle);
            }

            return _platformHandle;
        }

        private void EnsureClipChildren(IntPtr hwnd)
        {
            const int GWL_STYLE = -16;
            const uint WS_CLIPCHILDREN = 0x02000000;

            var style = GetWindowLong(hwnd, GWL_STYLE);
            var hasClipChildren = (style & WS_CLIPCHILDREN) != 0;

            if (!hasClipChildren)
            {
                var newStyle = style | WS_CLIPCHILDREN;
                SetWindowLong(hwnd, GWL_STYLE, newStyle);
            }
        }

        /// <inheritdoc />
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            contentChangedHandler?.Dispose();
            isVisibleChangedHandler?.Dispose();
            floatingContentChangedHandler?.Dispose();

            Detach();
            if (_floatingContent != null)
            {
                _floatingContent.PointerEntered -= FloatingContentOnPointerEvent;
                _floatingContent.PointerExited -= FloatingContentOnPointerEvent;
                _floatingContent.PointerPressed -= FloatingContentOnPointerEvent;
                _floatingContent.PointerReleased -= FloatingContentOnPointerEvent;
                _floatingContent.Close();
                _floatingContent = null;
            }

            base.DestroyNativeControlCore(control);

            if (_platformHandle != null)
            {
                _platformHandle = null;
            }
        }
    }
}
