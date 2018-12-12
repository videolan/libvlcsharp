using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ModuleDescriptionStructure
    {
        internal IntPtr Name;
        internal IntPtr ShortName;
        internal IntPtr LongName;
        internal IntPtr Help;
        internal IntPtr Next;
    }

    /// <summary>
    /// Description of a module.
    /// </summary>
    public class ModuleDescription
    {
        public string Name;
        public string ShortName;
        public string LongName;
        public string Help;
    }
}