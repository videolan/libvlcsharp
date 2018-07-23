namespace LibVLCSharp.Platforms.Gtk
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native references inside the GDK libraries
    /// </summary>
    internal static class NativeReferences
    {
        /// <summary>
        /// Gets the window's HWND
        /// </summary>
        /// <remarks>Window only</remarks>
        /// <param name="gdkWindow">The pointer to the GdkWindow object</param>
        /// <returns>The window's HWND</returns>
        [DllImport("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr gdk_win32_drawable_get_handle(IntPtr gdkWindow);

        /// <summary>
        /// Gets the window's XID
        /// </summary>
        /// <remarks>Linux X11 only</remarks>
        /// <param name="gdkWindow">The pointer to the GdkWindow object</param>
        /// <returns>The window's XID</returns>
        [DllImport("libgdk-x11-2.0.so.0", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint gdk_x11_drawable_get_xid(IntPtr gdkWindow);
    }
}