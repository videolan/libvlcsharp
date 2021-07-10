using System;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Manager to keep the device awake
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class DeviceAwakeningManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DeviceAwakeningManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        /// <param name="displayRequest">display request object</param>
        public DeviceAwakeningManager(IDispatcher? dispatcher, IDisplayRequest displayRequest) : base(dispatcher)
        {
            DisplayRequest = displayRequest;
            MediaPlayerChanged += OnStateChangedAsync;
        }

        private IDisplayRequest DisplayRequest { get; }

        private bool DeviceActive { get; set; }

        private bool _keepDeviceAwake = true;
        /// <summary>
        /// Gets or sets a value indicating whether the device should be kept awake
        /// </summary>
        public bool KeepDeviceAwake
        {
            get => _keepDeviceAwake;

            set
            {
                if (_keepDeviceAwake != value)
                {
                    _keepDeviceAwake = value;
                    if (value)
                    {
                        UpdateState();
                    }
                    else
                    {
                        SetDeviceActive(false);
                    }
                }
            }
        }

        private void SetDeviceActive(bool value)
        {
            if (DeviceActive != value)
            {
                DeviceActive = value;
                if (value)
                {
                    DisplayRequest.RequestActive();
                }
                else
                {
                    DisplayRequest.RequestRelease();
                }
            }
        }

        private void UpdateState()
        {
            var mediaPlayer = MediaPlayer;
            SetDeviceActive(KeepDeviceAwake && mediaPlayer != null &&
                (mediaPlayer.State == VLCState.Playing || mediaPlayer.State == VLCState.Opening));
        }

        private async void OnStateChangedAsync(object? sender, EventArgs e)
        {
            await DispatcherInvokeAsync(UpdateState);
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.EncounteredError += OnStateChangedAsync;
            mediaPlayer.EndReached += OnStateChangedAsync;
            mediaPlayer.NothingSpecial += OnStateChangedAsync;
            mediaPlayer.Paused += OnStateChangedAsync;
            mediaPlayer.Opening += OnStateChangedAsync;
            mediaPlayer.Playing += OnStateChangedAsync;
            mediaPlayer.Stopped += OnStateChangedAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.EncounteredError -= OnStateChangedAsync;
            mediaPlayer.EndReached -= OnStateChangedAsync;
            mediaPlayer.NothingSpecial -= OnStateChangedAsync;
            mediaPlayer.Paused -= OnStateChangedAsync;
            mediaPlayer.Opening += OnStateChangedAsync;
            mediaPlayer.Playing -= OnStateChangedAsync;
            mediaPlayer.Stopped -= OnStateChangedAsync;
        }
    }
}
