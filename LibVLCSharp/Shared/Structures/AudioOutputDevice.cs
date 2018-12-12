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

    public class AudioOutputDevice
    {
        public string DeviceIdentifier;
        public string Description;
    }
}