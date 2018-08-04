using System;

namespace LibVLCSharp.Shared
{
    public interface ILibVLCMediaSource : IMediaSource, IDisposable
    {
        LibVLC LibVLC { get; }
        MediaPlayer MediaPlayer { get; }
    }
}
