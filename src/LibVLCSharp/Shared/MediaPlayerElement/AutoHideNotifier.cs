using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Notifies when the playback controls should be shown or hidden
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal class AutoHideNotifier : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs whenever the playback controls should be shown
        /// </summary>
        public event EventHandler? Shown;
        /// <summary>
        /// Occurs whenever the playback controls should be hidden
        /// </summary>
        public event EventHandler? Hidden;

        /// <summary>
        /// Initializes a new instance of <see cref="AutoHideNotifier"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public AutoHideNotifier(IDispatcher? dispatcher) : base(dispatcher)
        {
            MediaPlayerChanged += async (sender, e) => await ShowAsync();
            Timer = new Timer(async obj => await HideAsync(), null, Timeout.Infinite, Timeout.Infinite);
        }

        private bool _enabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether the auto hide feature should be enabled or not
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (value)
                    {
                        StartTimer();
                    }
                    else
                    {
                        OnShown();
                    }
                }
            }
        }

        private Timer Timer { get; }

        /// <summary>
        ///  Shows the playback controls if they're hidden
        /// </summary>
        /// <param name="pointerIsOnPlaybackControls">true if the pointer is on playback controls, false otherwise</param>
        public void Show(bool pointerIsOnPlaybackControls = false)
        {
            OnShown();
            if (!pointerIsOnPlaybackControls)
            {
                StartTimer();
            }
        }

        private void OnShown()
        {
            StopTimer();
            Shown?.Invoke(this, EventArgs.Empty);
        }

        private async Task ShowAsync()
        {
            StopTimer();
            await DispatcherInvokeEventHandlerAsync(Shown);
            StartTimer();
        }

        /// <summary>
        /// Hides the playback controls if they're shown
        /// </summary>
        public void Hide()
        {
            StopTimer();
            Hidden?.Invoke(this, EventArgs.Empty);
        }

        private Task HideAsync()
        {
            StopTimer();
            return DispatcherInvokeEventHandlerAsync(Hidden);
        }

        private void StartTimer()
        {
            if (Enabled && MediaPlayer?.State == VLCState.Playing)
            {
                Timer.Change(TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(Timeout.Infinite));
            }
        }

        private void StopTimer()
        {
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async void OnStateChangedAsync(object? sender, EventArgs e)
        {
            await ShowAsync();
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.MediaChanged += OnStateChangedAsync;
            mediaPlayer.Buffering += OnStateChangedAsync;
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
            mediaPlayer.MediaChanged -= OnStateChangedAsync;
            mediaPlayer.Buffering -= OnStateChangedAsync;
            mediaPlayer.EncounteredError -= OnStateChangedAsync;
            mediaPlayer.EndReached -= OnStateChangedAsync;
            mediaPlayer.NothingSpecial -= OnStateChangedAsync;
            mediaPlayer.Paused -= OnStateChangedAsync;
            mediaPlayer.Opening += OnStateChangedAsync;
            mediaPlayer.Playing -= OnStateChangedAsync;
            mediaPlayer.Stopped -= OnStateChangedAsync;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            Timer.Dispose();
        }
    }
}
