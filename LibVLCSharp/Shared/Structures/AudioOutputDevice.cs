using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AudioOutputDeviceStructure
    {
        public IntPtr Next;
        public IntPtr DeviceIdentifier;
        public IntPtr Description;
    }

    /// <summary>
    /// Description for audio output device
    /// </summary>
    public class AudioOutputDevice
    {
        /// <summary>
        /// Device identifier string.
        /// </summary>
        public string DeviceIdentifier;
        
        /// <summary>
        /// User-friendly device description.
        /// </summary>
        public string Description;
    }
}