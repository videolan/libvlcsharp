namespace LibVLCSharp.Gtk.Sample
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native references to system's library
    /// </summary>
    public static class NativeReferences
    {
        /// <summary>
        /// Gets the window's XID
        /// </summary>
        /// <remarks>Linux X11 only</remarks>
        /// <param name="gdkWindow">The pointer to the GdkWindow object</param>
        /// <returns>The window's XID</returns>
        [DllImport("libX11.so.6", CallingConvention = CallingConvention.Cdecl)]
        public static extern int XInitThreads();
    }
}