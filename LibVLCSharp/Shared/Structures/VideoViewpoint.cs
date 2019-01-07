using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    /// <summary>Viewpoint for video outputs</summary>
    /// <remarks>allocate using libvlc_video_new_viewpoint()</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct VideoViewpoint
    {
        public readonly float Yaw;
        public readonly float Pitch;
        public readonly float Roll;
        public readonly float Fov;
    }
}
