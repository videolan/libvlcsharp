using System;

namespace LibVLCSharp.Shared
{
    public interface IMediaSource : ISource, IDisposable
    {
        LibVLC LibVLC { get; }
        MediaPlayer MediaPlayer { get; }
    }
}
