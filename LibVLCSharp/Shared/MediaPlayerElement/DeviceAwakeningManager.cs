#nullable enable
using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Manager to keep the device awake
    /// </summary>
    public class DeviceAwakeningManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DeviceAwakeningManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        /// <param name="displayRequest">display request object</param>
        public DeviceAwakeningManager(IDispatcher dispatcher, IDisplayRequest displayRequest) : base(dispatcher)
        {
            DisplayRequest = displayRequest;
            Initialized += OnStateChangedAsync;
        }

        private IDisplayRequest DisplayRequest { get; }

        private bool DeviceActive { get; set; }

        private bool KeepDeviceAwake { get; set; } = true;

        /// <summary>
        ///Sets a value indicating whether the device should be kept awake
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task KeepDeviceAwakeAsync(bool value)
        {
            if (KeepDeviceAwake != value)
            {
                KeepDeviceAwake = value;
                if (value)
                {
                    await UpdateStateAsync();
                }
                else
                {
                    await SetDeviceActiveAsync(false);
                }
            }
        }

        private async Task SetDeviceActiveAsync(bool value)
        {
            if (DeviceActive != value)
            {
                DeviceActive = value;
                await DispatcherInvokeAsync(() =>
                {
                    if (value)
                    {
                        DisplayRequest.RequestActive();
                    }
                    else
                    {
                        DisplayRequest.RequestRelease();
                    }
                });
            }
        }

        private async Task UpdateStateAsync()
        {
            var state = MediaPlayer?.State;
            await SetDeviceActiveAsync(KeepDeviceAwake && (state == VLCState.Opening || state == VLCState.Playing));
        }

        private async void OnStateChangedAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync();
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
            mediaPlayer.Playing -= OnStateChangedAsync;
            mediaPlayer.Stopped -= OnStateChangedAsync;
        }
    }
}
#nullable restore
