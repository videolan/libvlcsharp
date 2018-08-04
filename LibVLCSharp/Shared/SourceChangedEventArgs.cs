using System;

namespace LibVLCSharp.Shared
{
    public class SourceChangedEventArgs : EventArgs
    {
        public SourceChangedEventArgs(IMediaSource oldSource, IMediaSource newSource)
        {
            OldSource = oldSource;
            NewSource = newSource;
        }

        public IMediaSource OldSource { get; }
        public IMediaSource NewSource { get; }
    }
}
