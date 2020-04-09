namespace LibVLCSharp
{
    /// <summary>
    /// VideoView Interface
    /// </summary>
    public interface IVideoView
    {
        /// <summary>
        /// MediaPlayer object connected to the view
        /// </summary>
        MediaPlayer? MediaPlayer { get; set; }
    }
}
