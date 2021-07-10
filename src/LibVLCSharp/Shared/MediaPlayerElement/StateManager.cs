using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// State manager
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> properties
    /// need to be set in order to work</remarks>
    internal class StateManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs when an error is encountered
        /// </summary>
        public event EventHandler? ErrorOccured;

        /// <summary>
        /// Occurs when the error message should be cleared
        /// </summary>
        public event EventHandler? ErrorCleared;

        /// <summary>
        /// Occurs when the <see cref="PlayPauseAvailable"/> property value changes
        /// </summary>
        public event EventHandler? PlayPauseAvailableChanged;

        /// <summary>
        /// Occurs when the media player started playing a media
        /// </summary>
        public event EventHandler? Playing;

        /// <summary>
        /// Occurs when the media player paused playback
        /// </summary>
        public event EventHandler? Paused;

        /// <summary>
        /// Occurs when the media player paused playback
        /// </summary>
        public event EventHandler? Stopped;

        /// <summary>
        /// Initializes a new instance of <see cref="StateManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public StateManager(IDispatcher? dispatcher) : base(dispatcher)
        {
            MediaPlayerChanged += OnMediaPlayerChangedAsync;
        }

        private bool HasError { get; set; }

        /// <summary>
        /// Gets the media resource locator
        /// </summary>
        public string? MediaResourceLocator 
        { 
            get 
            {
                var mrl = string.Empty;
                var media = MediaPlayer?.Media;
                if(media != null)
                {
                    mrl = media.Mrl;
                    media.Dispose();
                }
                return mrl; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the playback is playing
        /// </summary>
        public bool IsPlaying => MediaPlayer?.State == VLCState.Playing;

        /// <summary>
        /// Gets a value indicating whether the playback is playing or is paused
        /// </summary>
        public bool IsPlayingOrPaused
        {
            get
            {
                var state = MediaPlayer?.State;
                return state == VLCState.Playing || state == VLCState.Paused;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the play/pause button should be shown
        /// </summary>
        public bool PlayPauseAvailable
        {
            get;
            private set;
        }

        /// <summary>
        /// Toggles pause
        /// </summary>
        public void TogglePause()
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }

            var state = mediaPlayer.State;
            switch (state)
            {
                case VLCState.Ended:
                    mediaPlayer.Stop();
                    goto case VLCState.Stopped;
                case VLCState.Paused:
                case VLCState.Stopped:
                case VLCState.Error:
                    mediaPlayer.Play();
                    break;
                default:
                    mediaPlayer.Pause();
                    break;
            }
        }

        /// <summary>
        /// Stops the playback
        /// </summary>
        public void Stop()
        {
            MediaPlayer?.Stop();
        }

        private Task UpdatePlayPauseAvailabilityAsync()
        {
            var mediaPlayer = MediaPlayer;
            var playPauseAvailable = mediaPlayer == null ? false : mediaPlayer.State != VLCState.Playing || mediaPlayer.CanPause;
            if (PlayPauseAvailable != playPauseAvailable)
            {
                PlayPauseAvailable = playPauseAvailable;
                return DispatcherInvokeEventHandlerAsync(PlayPauseAvailableChanged);
            }
            return Task.CompletedTask;
        }

        private async Task PlayingOrPausedAsync(EventHandler? eventHandler)
        {
            if (HasError)
            {
                HasError = false;
                await DispatcherInvokeEventHandlerAsync(ErrorCleared);
            }
            await DispatcherInvokeEventHandlerAsync(eventHandler);
        }

        private Task OnErrorOccuredAsync()
        {
            HasError = true;
            return DispatcherInvokeEventHandlerAsync(ErrorOccured);
        }

        private async void OnMediaPlayerChangedAsync(object? sender, EventArgs e)
        {
            await UpdatePlayPauseAvailabilityAsync();
            switch (MediaPlayer?.State)
            {
                case VLCState.Playing:
                    await PlayingOrPausedAsync(Playing);
                    break;
                case VLCState.Paused:
                    await PlayingOrPausedAsync(Paused);
                    break;
                case VLCState.Error:
                    await OnErrorOccuredAsync();
                    goto default;
                default:
                    await DispatcherInvokeEventHandlerAsync(Stopped);
                    break;
            }
        }

        private async void MediaPlayer_EncounteredErrorAsync(object? sender, EventArgs e)
        {
            await OnErrorOccuredAsync();
        }

        private async void MediaPlayer_PlayingAsync(object? sender, EventArgs e)
        {
            await PlayingOrPausedAsync(Playing);
            await UpdatePlayPauseAvailabilityAsync();
        }

        private async void MediaPlayer_PausedAsync(object? sender, EventArgs e)
        {
            await PlayingOrPausedAsync(Paused);
        }

        private async void MediaPlayer_StoppedAsync(object? sender, EventArgs e)
        {
            await DispatcherInvokeEventHandlerAsync(Stopped);
            await UpdatePlayPauseAvailabilityAsync();
        }

        private async void MediaPlayer_PausableChangedAsync(object? sender, MediaPlayerPausableChangedEventArgs e)
        {
            await UpdatePlayPauseAvailabilityAsync();
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.EncounteredError += MediaPlayer_EncounteredErrorAsync;
            mediaPlayer.Stopped += MediaPlayer_StoppedAsync;
            mediaPlayer.EndReached += MediaPlayer_StoppedAsync;
            mediaPlayer.Paused += MediaPlayer_PausedAsync;
            mediaPlayer.Playing += MediaPlayer_PlayingAsync;
            mediaPlayer.PausableChanged += MediaPlayer_PausableChangedAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.EncounteredError -= MediaPlayer_EncounteredErrorAsync;
            mediaPlayer.Stopped -= MediaPlayer_StoppedAsync;
            mediaPlayer.EndReached -= MediaPlayer_StoppedAsync;
            mediaPlayer.Paused -= MediaPlayer_PausedAsync;
            mediaPlayer.Playing -= MediaPlayer_PlayingAsync;
            mediaPlayer.PausableChanged -= MediaPlayer_PausableChangedAsync;
        }
    }
}
