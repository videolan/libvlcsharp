namespace LibVLCSharp.Shared
{
    public interface IVideoView
    {
        MediaPlayer MediaPlayer { get; }
        LibVLC LibVLC { get; }
    }
}