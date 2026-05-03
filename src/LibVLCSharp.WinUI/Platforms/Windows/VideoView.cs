namespace LibVLCSharp.Platforms.Windows
{
    /// <summary>
    /// VideoView implementation for the WinUI platform
    /// </summary>
    public partial class VideoView : VideoView<InitializedEventArgs>
    {
        /// <summary>
        /// Creates args for <see cref="VideoView{TInitializedEventArgs}.Initialized"/> event
        /// </summary>
        /// <returns>args for <see cref="VideoView{TInitializedEventArgs}.Initialized"/> event</returns>
        protected override InitializedEventArgs CreateInitializedEventArgs()
        {
            return new InitializedEventArgs(SwapChainOptions);
        }
    }
}