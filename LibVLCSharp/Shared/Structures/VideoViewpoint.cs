using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    /// <summary>Viewpoint for video outputs</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VideoViewpoint
    {
        /// <summary>
        /// view point yaw in degrees  ]-180;180]
        /// </summary>
        public float Yaw { get; internal set; }

        /// <summary>
        /// view point pitch in degrees  ]-90;90]
        /// </summary>
        public float Pitch { get; internal set; }

        /// <summary>
        /// view point roll in degrees ]-180;180]
        /// </summary>
        public float Roll { get; internal set; }

        /// <summary>
        /// field of view in degrees ]0;180[ (default 80.)
        /// </summary>
        public float Fov { get; internal set; }
    }
}