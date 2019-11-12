#nullable enable
using System;
using System.Threading;

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
        public AutoHideNotifier(IDispatcher dispatcher) : base(dispatcher)
        {
            Timer = new Timer(obj => Hide(), null, Timeout.Infinite, Timeout.Infinite);
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
        /// Shows the playback controls if they're hidden
        /// </summary>
        public void Show()
        {
            OnShown();
            StartTimer();
        }

        private void OnShown()
        {
            StopTimer();
            var shown = Shown;
            if (shown != null)
            {
                DispatcherInvokeAsync(() => shown(this, EventArgs.Empty));
            }
        }

        /// <summary>
        /// Hides the playback controls if they're shown
        /// </summary>
        public void Hide()
        {
            StopTimer();
            var hidden = Hidden;
            if (hidden != null)
            {
                DispatcherInvokeAsync(() => hidden(this, EventArgs.Empty));
            }
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

        private void OnStateChanged(object sender, EventArgs e)
        {
            Show();
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.Buffering += OnStateChanged;
            mediaPlayer.EncounteredError += OnStateChanged;
            mediaPlayer.EndReached += OnStateChanged;
            mediaPlayer.NothingSpecial += OnStateChanged;
            mediaPlayer.Paused += OnStateChanged;
            mediaPlayer.Playing += OnStateChanged;
            mediaPlayer.Stopped += OnStateChanged;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.Buffering -= OnStateChanged;
            mediaPlayer.EncounteredError -= OnStateChanged;
            mediaPlayer.EndReached -= OnStateChanged;
            mediaPlayer.NothingSpecial -= OnStateChanged;
            mediaPlayer.Paused -= OnStateChanged;
            mediaPlayer.Playing -= OnStateChanged;
            mediaPlayer.Stopped -= OnStateChanged;
        }

        /// <summary>
        /// Initialization method called when <see cref="MediaPlayer"/> property changed or the controls are initialized
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            Show();
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
#nullable restore
