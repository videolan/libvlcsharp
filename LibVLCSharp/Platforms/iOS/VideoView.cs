using System;

using LibVLCSharp.Shared;

using UIKit;

namespace LibVLCSharp.Platforms.iOS
{
    public class VideoView : UIView
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

        void Attach()
        {
            if (Source != null)
            {
                Source.NsObject = Handle;
            }
        }

        void Detach()
        {
            if (Source != null)
            {
                Source.NsObject = IntPtr.Zero;
            }
        }
    }
}