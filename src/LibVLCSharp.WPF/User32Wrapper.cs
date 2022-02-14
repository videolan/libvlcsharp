using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.WPF
{
    internal static class User32Wrapper
    {
        public const string LibraryName = "user32.dll";

        /// <summary>
        /// Provide the win32 window stype when call <see cref="CreateWindowEx"/>
        /// <para>See following link: <a href="https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles" /> </para>
        /// </summary>
        [Flags]
        internal enum ExtendedWindow32Styles : int
        {
            /// <summary>
            /// The window should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
            /// The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020
        }

        /// <summary>
        /// Provide the win32 window stype when call <see cref="CreateWindowEx"/>
        /// <para>See following link: <a href="https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles" /> </para>
        /// </summary>
        [Flags]
        internal enum Window32Styles : int
        {
            /// <summary>
            /// The window is a child window. A window with this style cannot have a menu bar.
            /// </summary>
            WS_CHILD = 0x40000000,

            /// <summary>
            /// The window is initially visible.
            /// </summary>
            WS_VISIBLE = 0x10000000
        }


        [DllImport(LibraryName)]
        internal static extern IntPtr CreateWindowEx(ExtendedWindow32Styles dwExStyle,
                string lpszClassName,
                string lpszWindowName,
                Window32Styles style,
                int x, int y, int width, int height,
                IntPtr hwndParent,
                IntPtr hMenu,
                IntPtr hInst,
                IntPtr lpParam);


        [DllImport(LibraryName)]
        internal static extern bool DestroyWindow(IntPtr hwnd);
    }
}
