using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace LibVLCSharp.WPF
{
    /// <summary>
    /// Provide the UserControl with Handle Pointer in WPF
    /// <para>This class will create instance of win32 window and hosted it in WPF Framework element</para>
    /// <remark>As WPF only render the whole window, so all controls over the <see cref="VideoHwndHost"/> will not reneder</remark>
    /// </summary>
    internal class VideoHwndHost : HwndHost
    {
        /// <inheritdoc/>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var windowHandle = User32Wrapper.CreateWindowEx(User32Wrapper.ExtendedWindow32Styles.WS_EX_TRANSPARENT, "static", string.Empty,
                                                       User32Wrapper.Window32Styles.WS_CHILD | User32Wrapper.Window32Styles.WS_VISIBLE, 
                                                       0, 0, 0, 0, 
                                                       hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return new HandleRef(this, windowHandle);
        }

        /// <inheritdoc/>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            User32Wrapper.DestroyWindow(hwnd.Handle);
        }
    }
}
