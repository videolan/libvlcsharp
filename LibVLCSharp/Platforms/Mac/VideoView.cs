using System;
using LibVLCSharp.Shared;
using AppKit;

namespace LibVLCSharp.Platforms.Mac
{
    public class VideoView : NSView
    {
        public VideoView()
        {
        }

        private ISource _source;
        public ISource Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    Detach();
                    _source = value;
                    Attach();
                }
            }
        }

        void Attach() => Source?.SetWindowHandle(Handle);

        void Detach() => Source?.SetWindowHandle(IntPtr.Zero);
    }
}
