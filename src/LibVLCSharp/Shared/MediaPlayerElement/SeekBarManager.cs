using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Seek bar manager
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class SeekBarManager : MediaPlayerElementManagerBase
    {
        private bool isDragging = false;

        /// <summary>
        /// Gets or sets a value indicating whether the seek bar is being dragged by the user.
        /// </summary>
        public bool IsDragging
        {
            get => isDragging;
            set => isDragging = value;
        }

        /// <summary>
        /// Occurs when the media position changes
        /// </summary>
        public event EventHandler? PositionChanged;

        /// <summary>
        /// Occurs when the <see cref="Seekable"/> property value changes
        /// </summary>
        public event EventHandler? SeekableChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="SeekBarManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public SeekBarManager(IDispatcher? dispatcher) : base(dispatcher)
        {
            MediaPlayerChanged += async (sender, e) => await UpdateSeekableAndPositionAsync();
        }

        private bool _seekable;
        /// <summary>
        /// Gets a value indicating whether the media is seekable
        /// </summary>
        public bool Seekable
        {
            get => _seekable;
            private set
            {
                if (_seekable != value)
                {
                    _seekable = value;
                    SeekableChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the seek bar highest possible value
        /// </summary>
        public double SeekBarMaximum { get; set; }

        private long Length
        {
            get
            {
                var mediaPlayer = MediaPlayer;
                return mediaPlayer == null ? 0 : mediaPlayer.Length;
            }
        }

        private bool ErrorOrEnded
        {
            get
            {
                var mediaPlayer = MediaPlayer;
                return mediaPlayer == null ? true : mediaPlayer.State == VLCState.Error || mediaPlayer.State == VLCState.Ended;
            }
        }

        /// <summary>
        /// Gets elapsed and remaining time
        /// </summary>
        /// <returns>a <see cref="MediaPosition"/> instance containing the position, elapsed and remaining time</returns>
        public MediaPosition Position
        {
            get
            {
                var mediaPlayerPosition = ErrorOrEnded ? 0 : MediaPlayer!.Position;
                return new MediaPosition(mediaPlayerPosition, mediaPlayerPosition * SeekBarMaximum, Length);
            }
        }

        /// <summary>
        /// Sets media position as percentage, between 0.0 and 1.0
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(float position)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                mediaPlayer.Position = position;
            }
        }

        /// <summary>
        /// Sets the seek bar position
        /// </summary>
        /// <param name="position">seek bar position</param>
        public void SetSeekBarPosition(double position)
        {
            SetPosition((float)(position / SeekBarMaximum));
        }

        private Task UpdateSeekableAsync()
        {
            return DispatcherInvokeAsync(() =>
            {
                Seekable = !ErrorOrEnded && MediaPlayer!.IsSeekable == true;
            });
        }

        private async Task OnPositionChangedAsync()
        {
            // Fire event only when not seeking manually
            if (!isDragging)
            {
                await DispatcherInvokeEventHandlerAsync(PositionChanged);
            }
        }

        private async Task UpdateSeekableAndPositionAsync()
        {
            await UpdateSeekableAsync();
            await OnPositionChangedAsync();
        }

        private async void MediaPlayer_SeekableChangedAsync(object? sender, EventArgs e)
        {
            await UpdateSeekableAsync();
        }

        private async void MediaPlayer_PositionChangedAsync(object? sender, EventArgs e)
        {
            await OnPositionChangedAsync();
        }

        private async void MediaPlayer_StoppedAsync(object? sender, EventArgs e)
        {
            await UpdateSeekableAndPositionAsync();
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.EncounteredError += MediaPlayer_StoppedAsync;
            mediaPlayer.Stopped += MediaPlayer_StoppedAsync;
            mediaPlayer.EndReached += MediaPlayer_StoppedAsync;
            mediaPlayer.SeekableChanged += MediaPlayer_SeekableChangedAsync;
            mediaPlayer.LengthChanged += MediaPlayer_PositionChangedAsync;
            mediaPlayer.PositionChanged += MediaPlayer_PositionChangedAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.EncounteredError -= MediaPlayer_StoppedAsync;
            mediaPlayer.Stopped -= MediaPlayer_StoppedAsync;
            mediaPlayer.EndReached -= MediaPlayer_StoppedAsync;
            mediaPlayer.SeekableChanged -= MediaPlayer_SeekableChangedAsync;
            mediaPlayer.LengthChanged -= MediaPlayer_PositionChangedAsync;
            mediaPlayer.PositionChanged -= MediaPlayer_PositionChangedAsync;
        }
    }
}
