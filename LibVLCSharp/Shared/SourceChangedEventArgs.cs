using System;

namespace LibVLCSharp.Shared
{
    public class SourceChangedEventArgs : EventArgs
    {
        public SourceChangedEventArgs(ISource oldSource, ISource newSource)
        {
            OldSource = oldSource;
            NewSource = newSource;
        }

        public ISource OldSource { get; }
        public ISource NewSource { get; }
    }
}
