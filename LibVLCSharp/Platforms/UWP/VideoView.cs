namespace LibVLCSharp.Platforms.UWP
{
    using LibVLCSharp.Shared;
    using SharpDX;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;
    using SharpDX.Mathematics.Interop;
    using System;
    using System.Runtime.InteropServices;
    using Windows.UI.Xaml.Controls;
    using Device3 = SharpDX.DXGI.Device3;

    /// <summary>
    /// The VideoView implementation for UWP applications
    /// </summary>
    public class VideoView : UserControl, IVideoView
    {
        SwapChainPanel _panel;
        SharpDX.Direct3D11.Device _d3d11Device;
        SharpDX.DXGI.Device1 _device;
        Device3 _device3;
        SwapChain2 _swapchain2;
        SwapChain1 _swapChain;

        bool _loaded;

        /// <summary>
        /// The constructor
        /// </summary>
        public VideoView()
        {
            this._panel = new SwapChainPanel();
            this.Content = this._panel;
            this.Loaded += (s, e) => this.OnLoad();
            this.Unloaded += (s, e) => this.OnUnload();

            _panel.SizeChanged += (s, eventArgs) =>
            {
                if (_loaded)
                {
                    UpdateSize();
                }
            };

            _panel.CompositionScaleChanged += (s, eventArgs) =>
            {
                if (_loaded)
                {
                    UpdateScale();
                }
            };
        }

        /// <summary>
        /// Gets the swapchain parameters to pass to the <see cref="LibVLC"/> constructor.
        ///
        /// If you don't pass them to the <see cref="LibVLC"/> constructor, the video won't
        /// be displayed in your application.
        /// </summary>
        /// <returns>The list of arguments to be given to the <see cref="LibVLC"/> constructor.</returns>
        public string[] GetSwapChainOptions()
        {
            if (!_loaded)
            {
                throw new InvalidOperationException("You must wait for the VideoView to be loaded before calling GetSwapChainOptions()");
            }

            return new string[]
            {
                $"--winrt-d3dcontext=0x{_d3d11Device.ImmediateContext.NativePointer.ToString("x")}",
                $"--winrt-swapchain=0x{_swapChain.NativePointer.ToString("x")}"
            };
        }

        void OnLoad()
        {
            SharpDX.DXGI.Factory2 dxgiFactory;
            DeviceCreationFlags deviceCreationFlags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport;

#if DEBUG
            deviceCreationFlags |= DeviceCreationFlags.Debug;
            try
            {
                dxgiFactory = new SharpDX.DXGI.Factory2(true);
            }
            catch (SharpDXException e)
            {
                dxgiFactory = new SharpDX.DXGI.Factory2(false);
            }
#else
            dxgiFactory = new SharpDX.DXGI.Factory2(false);
#endif
            _d3d11Device = null;
            foreach (var adapter in dxgiFactory.Adapters)
            {
                try
                {
                    _d3d11Device = new SharpDX.Direct3D11.Device(adapter, deviceCreationFlags);
                    break;
                }
                catch (SharpDXException e)
                {
                }
            }

            if (_d3d11Device is null)
            {
                throw new Exception("Could not D3D11CreateDevice");
            }

            _device = _d3d11Device.QueryInterface<SharpDX.DXGI.Device1>();

            //Create the swapchain
            var swapChainDescription = new SharpDX.DXGI.SwapChainDescription1
            {
                Width = (int)(this._panel.ActualWidth * this._panel.CompositionScaleX),
                Height = (int)(this._panel.ActualHeight * this._panel.CompositionScaleY),
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription =
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = Usage.RenderTargetOutput,
                BufferCount = 2,
                SwapEffect = SwapEffect.FlipSequential,
                Flags = SwapChainFlags.None,
                AlphaMode = AlphaMode.Unspecified
            };

            _swapChain = new SharpDX.DXGI.SwapChain1(dxgiFactory, _d3d11Device, ref swapChainDescription);

            _device.MaximumFrameLatency = 1;

            //TODO: perform the next 2 calls on the UI thread
            var panelNative = ComObject.As<ISwapChainPanelNative>(this._panel);
            panelNative.SwapChain = _swapChain;

            // This is necessary so we can call Trim() on suspend
            this._device3 = _device.QueryInterface<SharpDX.DXGI.Device3>();
            if (_device3 == null)
                throw new Exception();
            this._swapchain2 = _swapChain.QueryInterface<SharpDX.DXGI.SwapChain2>();
            if (_swapchain2 == null)
                throw new Exception();

            UpdateScale();
            UpdateSize();
            this._loaded = true;
        }

        void OnUnload()
        {
            this._swapchain2?.Dispose();
            this._swapchain2 = null;

            this._device3?.Dispose();
            this._device3 = null;

            this._swapChain?.Dispose();
            this._swapChain = null;

            this._device?.Dispose();
            this._device = null;

            this._d3d11Device?.Dispose();
            this._d3d11Device = null;
        }

        readonly Guid SWAPCHAIN_WIDTH = new Guid(0xf1b59347, 0x1643, 0x411a, 0xad, 0x6b, 0xc7, 0x80, 0x17, 0x7a, 0x6, 0xb6);
        readonly Guid SWAPCHAIN_HEIGHT = new Guid(0x6ea976a0, 0x9d60, 0x4bb7, 0xa5, 0xa9, 0x7d, 0xd1, 0x18, 0x7f, 0xc9, 0xbd);

        void UpdateSize()
        {
            IntPtr width = Marshal.AllocHGlobal(sizeof(int));
            IntPtr height = Marshal.AllocHGlobal(sizeof(int));
            var w = (int)(this._panel.ActualWidth * this._panel.CompositionScaleX);
            var h = (int)(this._panel.ActualHeight * this._panel.CompositionScaleY);

            Marshal.WriteInt32(width, w);
            Marshal.WriteInt32(height, h);

            _swapChain.SetPrivateData(SWAPCHAIN_WIDTH, sizeof(int), width);
            _swapChain.SetPrivateData(SWAPCHAIN_HEIGHT, sizeof(int), height);

            Marshal.FreeHGlobal(width);
            Marshal.FreeHGlobal(height);
        }

        void UpdateScale()
        {
            _swapchain2.MatrixTransform = new RawMatrix3x2 { M11 = 1.0f / this._panel.CompositionScaleX, M22 = 1.0f / this._panel.CompositionScaleY };
        }

        void Trim()
        {
            _device3?.Trim();
        }

        void Attach()
        {
        }

        void Detach()
        {
        }

        MediaPlayer _mediaPlayer;

        /// <summary>
        /// The MediaPlayer object attached to this VideoView. Use this to manage playback and more
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                if (_mediaPlayer != value)
                {
                    Detach();
                    _mediaPlayer = value;

                    if (_mediaPlayer != null)
                    {
                        Attach();
                    }
                }
            }
        }

    }
}
