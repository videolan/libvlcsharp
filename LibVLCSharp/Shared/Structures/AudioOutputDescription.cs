using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared.Structures
{
    /// <summary>
    /// <para>Description for audio output. It contains</para>
    /// <para>name, description and pointer to next record.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AudioOutputDescriptionStructure
    {
        public IntPtr Name;
        public IntPtr Description;
        public IntPtr NextAudioOutputDescription;
    }

    public class AudioOutputDescription
    {
        public string Name;
        public string Description;
    }
}
