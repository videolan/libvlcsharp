using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LibVLCSharp.Shared;
using Device3 = SharpDX.DXGI.Device3;
using System.Runtime.InteropServices;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        LibVLC _libVLC;
        MediaPlayer _mp;

        public MainPage()
        {
            Core.Initialize();
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CreateSwapPanel();
            UpdateSize((float)Panel.ActualWidth * Panel.CompositionScaleX, (float)Panel.ActualHeight * Panel.CompositionScaleY);

            this.Panel.SizeChanged += (s, eventArgs) =>
            {
                UpdateSize((float)Panel.ActualWidth * Panel.CompositionScaleX, (float)Panel.ActualHeight * Panel.CompositionScaleY);
            };

            var d3dcontext = $"--winrt-d3dcontext=0x{_d3d11Device.ImmediateContext.NativePointer.ToString("x")}";
            var swapchain = $"--winrt-swapchain=0x{_swapChain.NativePointer.ToString("x")}";
            _libVLC = new LibVLC(
                d3dcontext,
                swapchain);
            _mp = new MediaPlayer(new Media(_libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", FromType.FromLocation));
            _mp.Play();
        }

        readonly Guid SWAPCHAIN_WIDTH = new Guid(0xf1b59347, 0x1643, 0x411a, 0xad, 0x6b, 0xc7, 0x80, 0x17, 0x7a, 0x6, 0xb6);
        readonly Guid SWAPCHAIN_HEIGHT = new Guid(0x6ea976a0, 0x9d60, 0x4bb7, 0xa5, 0xa9, 0x7d, 0xd1, 0x18, 0x7f, 0xc9, 0xbd);

        void UpdateSize(float x, float y)
        {
            IntPtr width = Marshal.AllocHGlobal(sizeof(int));
            IntPtr height = Marshal.AllocHGlobal(sizeof(int));

            var w = (int)x;
            var h = (int)y;

            Marshal.WriteInt32(width, w);
            Marshal.WriteInt32(height, h);

            _swapChain.SetPrivateData(SWAPCHAIN_WIDTH, sizeof(int), width);
            _swapChain.SetPrivateData(SWAPCHAIN_HEIGHT, sizeof(int), height);

            Marshal.FreeHGlobal(width);
            Marshal.FreeHGlobal(height);
        }

        private SharpDX.Direct3D11.Device _d3d11Device;
        private SharpDX.DXGI.Device1 _device;
        private Device3 _device3;
        private SwapChain2 _swapchain2;
        private SwapChain1 _swapChain;

        void CreateSwapPanel()
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
                Width = (int)(this.Panel.ActualWidth * this.Panel.CompositionScaleX),
                Height = (int)(this.Panel.ActualHeight * this.Panel.CompositionScaleY),
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
            var panelNative = ComObject.As<ISwapChainPanelNative>(this.Panel);
            panelNative.SwapChain = _swapChain;
            
            // This is necessary so we can call Trim() on suspend
            this._device3 = _device.QueryInterface<SharpDX.DXGI.Device3>();
            if (_device3 == null)
                throw new Exception();
            this._swapchain2 = _swapChain.QueryInterface<SharpDX.DXGI.SwapChain2>();
            if (_swapchain2 == null)
                throw new Exception();
            
            UpdateScale(this.Panel.CompositionScaleX, this.Panel.CompositionScaleY);
        }

        private void UpdateScale(float panelCompositionScaleX, float panelCompositionScaleY)
        {           
            _swapchain2.MatrixTransform = new RawMatrix3x2 { M11 = 1.0f / panelCompositionScaleX, M22 = 1.0f / panelCompositionScaleY };
        }

        private void Trim()
        {
            _device3?.Trim();
        }
    }
}
