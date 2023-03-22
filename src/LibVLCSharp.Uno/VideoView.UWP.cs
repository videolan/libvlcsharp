using System;
using LibVLCSharp.Platforms.Windows;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// VideoView implementation for the UWP platform
    /// </summary>
    public class VideoView : VideoView<InitializedEventArgs>, IVideoView, IVideoControl
    {
        /// <summary>
        /// Creates args for <see cref="VideoView{TInitializedEventArgs}.Initialized"/> event
        /// </summary>
        /// <returns>args for <see cref="VideoView{TInitializedEventArgs}.Initialized"/> event</returns>
        protected override InitializedEventArgs CreateInitializedEventArgs()
        {
            return new InitializedEventArgs(SwapChainOptions);
        }

        double IVideoControl.Width => ActualWidth;
        double IVideoControl.Height => ActualHeight;

        private EventHandler? _sizeChangedHandler;
        event EventHandler IVideoControl.SizeChanged
        {
            add
            {
                _sizeChangedHandler += value;
                SizeChanged += VideoView_SizeChanged;
            }

            remove
            {
                _sizeChangedHandler -= value;
                SizeChanged -= VideoView_SizeChanged;
            }
        }

        private void VideoView_SizeChanged(object? sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            _sizeChangedHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
