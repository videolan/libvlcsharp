namespace LibVLCSharp.Shared
{
    public interface IVideoView
    {
        MediaPlayer MediaPlayer { get; }
        Instance Instance { get; }
    }
}