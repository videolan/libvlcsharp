using System;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Buffering progress notifier
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class BufferingProgressNotifier : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs when <see cref="IsBuffering"/> property value changes
        /// </summary>
        public event EventHandler? IsBufferingChanged;
        /// <summary>
        /// Occurs when buffering
        /// </summary>
        public event EventHandler? Buffering;

        /// <summary>
        /// Initializes a new instance of <see cref="BufferingProgressNotifier"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public BufferingProgressNotifier(IDispatcher? dispatcher) : base(dispatcher)
        {
        }

        private bool _isBuffering;
        /// <summary>
        /// Gets a value indicating whether the media is buffering
        /// </summary>
        public bool IsBuffering
        {
            get => _isBuffering;
            private set
            {
                if (_isBuffering != value)
                {
                    _isBuffering = value;
                    IsBufferingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private double _bufferingProgress;
        /// <summary>
        /// Gets the percentage of buffering completed (between 0 and 1)
        /// </summary>
        public double BufferingProgress
        {
            get => _bufferingProgress;
            private set
            {
                if (_bufferingProgress != value)
                {
                    _bufferingProgress = 1 - value < float.Epsilon ? 0 : value;
                    Buffering?.Invoke(this, EventArgs.Empty);
                    IsBuffering = _bufferingProgress > 0;
                }
            }
        }

        private async void MediaPlayer_BufferingAsync(object? sender, MediaPlayerBufferingEventArgs e)
        {
            await DispatcherInvokeAsync(() => BufferingProgress = e.Cache / 100);
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.Buffering += MediaPlayer_BufferingAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.Buffering -= MediaPlayer_BufferingAsync;
        }
    }
}
