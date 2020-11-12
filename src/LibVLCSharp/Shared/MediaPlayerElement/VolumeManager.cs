using System;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Volume manager
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class VolumeManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs when <see cref="Enabled"/> property value changes
        /// </summary>
        public event EventHandler? EnabledChanged;

        ///// <summary>
        ///// Occurs when <see cref="Volume"/> property value changes
        ///// </summary>
        //public event EventHandler? VolumeChanged;

        /// <summary>
        /// Occurs when <see cref="Mute"/> property value changes
        /// </summary>
        public event EventHandler? MuteChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="VolumeManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public VolumeManager(IDispatcher? dispatcher) : base(dispatcher)
        {
            MediaPlayerChanged += OnMediaPlayerChangedAsync;
        }

        private bool _enabled;
        /// <summary>
        /// Gets a value indicating whether the volume can be updated
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            private set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mute state
        /// </summary>
        public bool Mute
        {
            get => MediaPlayer?.Mute ?? true;
            set
            {
                var mediaPlayer = MediaPlayer;
                if (mediaPlayer != null)
                {
                    mediaPlayer.Mute = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        public int Volume
        {
            get
            {
                var volume = MediaPlayer?.Volume;
                return (volume == null || volume < 0) ? 0 : (int)volume;
            }
            set
            {
                var mediaPlayer = MediaPlayer;
                if (mediaPlayer != null)
                {
                    mediaPlayer.Volume = value;
                    Mute = value == 0;
                }
            }
        }

        private async void OnMediaPlayerChangedAsync(object? sender, EventArgs e)
        {
            await DispatcherInvokeAsync(() =>
            {
                Enabled = MediaPlayer != null;
                MuteChanged?.Invoke(this, EventArgs.Empty);
                //VolumeChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        //private async void MediaPlayer_VolumeChangedAsync(object? sender, EventArgs e)
        //{
        //    await DispatcherInvokeEventHandlerAsync(VolumeChanged);
        //}

        private async void MediaPlayer_MuteChangedAsync(object? sender, EventArgs e)
        {
            await DispatcherInvokeEventHandlerAsync(MuteChanged);
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            // Subscribe to the VolumeChanged event causes a bug when the media player is stopped
            //mediaPlayer.VolumeChanged += MediaPlayer_VolumeChangedAsync;
            mediaPlayer.Muted += MediaPlayer_MuteChangedAsync;
            mediaPlayer.Unmuted += MediaPlayer_MuteChangedAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            //mediaPlayer.VolumeChanged -= MediaPlayer_VolumeChangedAsync;
            mediaPlayer.Muted -= MediaPlayer_MuteChangedAsync;
            mediaPlayer.Unmuted -= MediaPlayer_MuteChangedAsync;
        }
    }
}
