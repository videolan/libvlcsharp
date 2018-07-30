using System;
using LibVLCSharp.Shared;
using AppKit;

namespace LibVLCSharp.Platforms.Mac
{
    public class VideoView : NSView, IVideoView
    {
        public VideoView(string[] cliOptions = default(string[]))
        {
            LibVLC = new LibVLC(cliOptions);
            MediaPlayer = new Shared.MediaPlayer(LibVLC);

            Attach();
        }

        public Shared.MediaPlayer MediaPlayer { get; }
        public LibVLC LibVLC { get; }

        void Attach() => MediaPlayer.NsObject = Handle;

        void Detach() => MediaPlayer.NsObject = IntPtr.Zero;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Detach();

            MediaPlayer.Media?.Dispose();
            MediaPlayer.Dispose();
            LibVLC.Dispose();
        }
    }
}
