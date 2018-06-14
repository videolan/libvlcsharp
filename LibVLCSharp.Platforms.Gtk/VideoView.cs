using System;
using System.Runtime.InteropServices;
using Gdk;
using Gtk;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Platforms.Gtk
{
    public class VideoView : DrawingArea, IVideoView
    {
        public VideoView(string[] cliOptions = default(string[]))
        {
            LibVLC = new LibVLC(cliOptions);
            MediaPlayer = new Shared.MediaPlayer(LibVLC);

            Color black = Color.Zero;
            Color.Parse("black", ref black);
            this.ModifyBg(StateType.Normal, black);

            this.Realized += (s, e) => { this.Attach(); };
        }

        public Shared.MediaPlayer MediaPlayer { get; }
        public LibVLC LibVLC { get; }

        void Attach()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MediaPlayer.Hwnd = NativeReferences.gdk_win32_drawable_get_handle(this.GdkWindow.Handle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                MediaPlayer.XWindow = NativeReferences.gdk_x11_drawable_get_xid(this.GdkWindow.Handle);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        void Detach()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MediaPlayer.Hwnd = IntPtr.Zero;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                MediaPlayer.XWindow = 0;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override void Dispose()
        {
            Detach();

            MediaPlayer.Media?.Dispose();
            MediaPlayer.Dispose();
            LibVLC.Dispose();
            base.Dispose();
        }
    }
}
