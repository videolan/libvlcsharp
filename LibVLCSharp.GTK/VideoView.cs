using System;
using System.Runtime.InteropServices;
using Gdk;
using Gtk;
using LibVLCSharp.Shared;

namespace LibVLCSharp.GTK
{
    /// <summary>
    /// GTK VideoView for Windows, Linux and Mac.
    /// Mac is currently unsupported (see https://github.com/mono/gtk-sharp/issues/257)
    /// </summary>
    public class VideoView : DrawingArea, IVideoView
    {
        struct Native
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

            /// <summary>
            /// Gets the nsview's handle
            /// </summary>
            /// <remarks>Mac only</remarks>
            /// <param name="gdkWindow">The pointer to the GdkWindow object</param>
            /// <returns>The nsview's handle</returns>
            [DllImport("libgdk-quartz-2.0.0.dylib")]
            internal static extern IntPtr gdk_quartz_window_get_nsview(IntPtr gdkWindow);
        }

        private MediaPlayer _mediaPlayer;

        public VideoView(MediaPlayer mediaPlayer = null)
        {
            _mediaPlayer = mediaPlayer;
            Color black = Color.Zero;
            Color.Parse("black", ref black);
            ModifyBg(StateType.Normal, black);

            Realized += (s, e) => Attach();
        }

        public MediaPlayer MediaPlayer
        {
            get
            {
                return _mediaPlayer;
            }
            set
            {
                if (ReferenceEquals(_mediaPlayer, value))
                {
                    return;
                }

                Detach();
                _mediaPlayer = value;
                Attach();
            }
        }

        void Attach()
        {
            if (!IsRealized || _mediaPlayer == null)
            {
                return;
            }

            if (PlatformHelper.IsWindows)
            {
                MediaPlayer.Hwnd = Native.gdk_win32_drawable_get_handle(GdkWindow.Handle);
            }
            else if (PlatformHelper.IsLinux)
            {
                MediaPlayer.XWindow = Native.gdk_x11_drawable_get_xid(GdkWindow.Handle);
            }
            else if (PlatformHelper.IsMac)
            {
                MediaPlayer.NsObject = Native.gdk_quartz_window_get_nsview(GdkWindow.Handle);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        void Detach()
        {
            if (!IsRealized || _mediaPlayer == null)
            {
                return;
            }

            if (PlatformHelper.IsWindows)
            {
                MediaPlayer.Hwnd = IntPtr.Zero;
            }
            else if (PlatformHelper.IsLinux)
            {
                MediaPlayer.XWindow = 0;
            }
            else if (PlatformHelper.IsMac)
            {
                MediaPlayer.NsObject = IntPtr.Zero;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override void Dispose()
        {
            Detach();
            base.Dispose();
        }
    }
}
