using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp
{
    /// <summary>
    /// Device configuration setup for ouput callbacks
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SetupDeviceConfig
    {
        /// <summary>
        /// set to true if D3D11_CREATE_DEVICE_VIDEO_SUPPORT is needed for D3D11
        /// </summary>
        public bool HardwareDecoding { get; }
    }

    /// <summary>
    /// Information about the device
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct SetupDeviceInfo
    {
        /// <summary>
        /// D3D11 info
        /// </summary>
        [FieldOffset(0)]
        public SetupDeviceInfoD3D11 D3D11;

        /// <summary>
        /// D3D9 info
        /// </summary>
        [FieldOffset(0)]
        public SetupDeviceInfoD3D9 D3D9;
    }

    /// <summary>
    /// Struct for d3d11 setup device info.
    /// </summary>
    public unsafe struct SetupDeviceInfoD3D11
    {
        /// <summary>
        /// ID3D11DeviceContext
        /// </summary>
        public void* DeviceContext { get; set; }

        /// <summary>
        /// Windows Mutex HANDLE to protect ID3D11DeviceContext usage
        /// </summary>
        public void* ContextMutex { get; set; }
    }

    /// <summary>
    /// Struct for d3d9 setup device info.
    /// </summary>
    public struct SetupDeviceInfoD3D9
    {
        /// <summary>
        /// IDirect3D9
        /// </summary>
        public IntPtr Device { get; set; }

        /// <summary>
        /// Adapter to use with the IDirect3D9*
        /// </summary>
        public int Adapter { get; set; }
    }

    /// <summary>
    /// Output callback render configuration
    /// </summary>
    public readonly struct RenderConfig
    {
        /// <summary>
        /// rendering video width in pixel
        /// </summary>
        public uint Width { get; }

        /// <summary>
        /// rendering video height in pixel
        /// </summary>
        public uint Height { get; }

        /// <summary>
        /// rendering video bit depth in bits per channel
        /// </summary>
        public uint BitDepth { get; }

        /// <summary>
        /// video is full range or studio/limited range
        /// </summary>
        public bool FullRange { get; }

        /// <summary>
        /// video color space
        /// </summary>
        public ColorSpace ColorSpace { get; }

        /// <summary>
        /// video color primaries
        /// </summary>
        public ColorPrimaries ColorPrimaries { get; }

        /// <summary>
        /// video transfer function
        /// </summary>
        public TransferFunction TransferFunction { get; }

        /// <summary>
        /// device used for rendering, IDirect3DDevice9* for D3D9
        /// </summary>
        public IntPtr Device { get; }
    }

    /// <summary>
    /// Output configuration information
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OutputConfig
    {
        /// <summary>
        /// Union with rendering format info for the various supported backends
        /// </summary>
        public OutputConfigUnion Union;

        /// <summary>
        /// video is full range or studio/limited range
        /// </summary>
        public bool FullRange { get; set; }

        /// <summary>
        /// video color space
        /// </summary>
        public ColorSpace ColorSpace { get; set; }

        /// <summary>
        /// video color primaries
        /// </summary>
        public ColorPrimaries ColorPrimaries { get; set; }

        /// <summary>
        /// video transfer function
        /// </summary>
        public TransferFunction TransferFunction { get; set; }

        /// <summary>
        /// video orientation
        /// </summary>
        public VideoOrientation Orientation { get; set; }
    }

    /// <summary>
    /// Union with rendering format info for the various supported backends
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct OutputConfigUnion
    {
        /// <summary>
        /// the rendering DXGI_FORMAT for d3d11
        /// </summary>
        [FieldOffset(0)]
        public int DxgiFormat;

        /// <summary>
        /// the rendering D3DFORMAT for d3d9
        /// </summary>
        [FieldOffset(0)]
        public uint D3d9Format;

        /// <summary>
        /// the rendering GLint GL_RGBA or GL_RGB for opengl and opengles2
        /// </summary>
        [FieldOffset(0)]
        public int OpenGLFormat;

        /// <summary>
        /// currently unused
        /// </summary>
        [FieldOffset(0)]
        public IntPtr Surface;
    }

    /// <summary>
    /// frame metadata for HDR10 medias
    /// similar to CTA-861-G with ranges from H265, based on SMPTE ST 2086 mastering display color volume
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FrameHDR10Metadata
    {
        /// <summary>
        /// [5,37 000] normalized x / [5,42 000] y chromacity in increments of 0.00002, 0=unknown
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly ushort[] RedPrimary;

        /// <summary>
        /// [5,37 000] normalized x / [5,42 000] y chromacity in increments of 0.00002, 0=unknown
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly ushort[] GreenPrimary;

        /// <summary>
        /// [5,37 000] normalized x / [5,42 000] y chromacity in increments of 0.00002, 0=unknown
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly ushort[] BluePrimary;

        /// <summary>
        /// [5,37 000] normalized x / [5,42 000] y white point in increments of 0.00002, 0=unknown
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly ushort[] WhitePoint;

        /// <summary>
        /// [50 000, 100 000 000] maximum luminance in 0.0001 cd/m², 0=unknown
        /// </summary>
        public uint MaxMasteringLuminance { get; }

        /// <summary>
        /// [1, 50 000] minimum luminance in 0.0001 cd/m², 0=unknown
        /// </summary>
        public uint MinMasteringLuminance { get; }

        /// <summary>
        /// [1, 50 000] Maximum Content Light Level in cd/m², 0=unknown
        /// </summary>
        public ushort MaxContentLightLevel { get; }

        /// <summary>
        /// [1, 50 000] Maximum Frame-Average Light Level in cd/m², 0=unknown
        /// </summary>
        public ushort MaxFrameAverageLightLevel { get; }
    }

    /// <summary>
    /// Enumeration of the Video engine to be used on output.
    /// </summary>
    public enum VideoEngine
    {
        /// <summary>
        /// Disable rendering engine
        /// </summary>
        Disable,

        /// <summary>
        /// OpenGL rendering engine
        /// </summary>
        OpenGL,

        /// <summary>
        /// OpenGLES2 rendering engine
        /// </summary>
        OpenGLES2,

        /// <summary>
        /// Direct3D11 rendering engine
        /// </summary>
        D3D11,

        /// <summary>
        /// Direct3D9 rendering engine
        /// </summary>
        D3D9
    }

    /// <summary>
    /// Enumeration of the Video color spaces.
    /// </summary>
    public enum ColorSpace
    {
        /// <summary>
        /// Rec. 601
        /// </summary>
        BT601 = 1,

        /// <summary>
        /// Rec.709
        /// </summary>
        BT709 = 2,

        /// <summary>
        /// Rec. 2020
        /// </summary>
        BT2020 = 3
    }

    /// <summary>
    /// Enumeration of the Video color primaries.
    /// </summary>
    public enum ColorPrimaries
    {
        /// <summary>
        /// Rec. 601, 525 lines
        /// </summary>
        BT601_525 = 1,

        /// <summary>
        /// Rec. 601, 625 lines
        /// </summary>
        BT601_625 = 2,

        /// <summary>
        /// Rec.709
        /// </summary>
        BT709 = 3,

        /// <summary>
        /// Rec. 2020
        /// </summary>
        BT2020 = 4,

        /// <summary>
        /// DCI/P3
        /// </summary>
        DCI_P3 = 5,

        /// <summary>
        /// BT470m
        /// </summary>
        BT470_M = 6
    }

    /// <summary>
    /// Enumeration of the Video transfer functions.
    /// </summary>
    public enum TransferFunction
    {
        /// <summary>
        /// Linear
        /// </summary>
        LINEAR = 1,

        /// <summary>
        /// SRGB
        /// </summary>
        SRGB = 2,

        /// <summary>
        /// BT470_BG
        /// </summary>
        BT470_BG = 3,

        /// <summary>
        /// BT470_M
        /// </summary>
        BT470_M = 4,

        /// <summary>
        /// BT709
        /// </summary>
        BT709 = 5,

        /// <summary>
        /// PQ
        /// </summary>
        PQ = 6,

        /// <summary>
        /// SMPTE_240
        /// </summary>
        SMPTE_240 = 7,

        /// <summary>
        /// HLG
        /// </summary>
        HLG = 8
    }

    /// <summary>
    /// List of types of frame metadata available
    /// </summary>
    public enum FrameMetadataType
    {
        /// <summary>
        /// HDR10
        /// </summary>
        FrameHDR10Metadata
    }

    /// <summary>
    /// Default shader strings for custom rendering sample code
    /// </summary>
    public static class DefaultShaders
    {
        /// <summary>
        /// Default HLSL shader string used in the official D3D11 custom rendering sample
        /// </summary>
        public const string HLSL = @"
Texture2D shaderTexture;
SamplerState samplerState;
struct PS_INPUT
{
    float4 position     : SV_POSITION;
    float4 textureCoord : TEXCOORD0;
};

float4 PShader(PS_INPUT In) : SV_TARGET
{
    return shaderTexture.Sample(samplerState, In.textureCoord);
}

struct VS_INPUT
{
    float4 position     : POSITION;
    float4 textureCoord : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 position     : SV_POSITION;
    float4 textureCoord : TEXCOORD0;
};

VS_OUTPUT VShader(VS_INPUT In)
{
    return In;
}
";
    }
}
