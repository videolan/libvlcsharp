using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared.Structures
{
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct AudioOutputDescriptionStructure
    {
        public IntPtr Name;
        public IntPtr Description;
        public IntPtr NextAudioOutputDescription;
    }

    /// <summary>
    /// Description for audio output.
    /// </summary>
    public class AudioOutputDescription
    {
        public string Name;
        public string Description;
    }
}
