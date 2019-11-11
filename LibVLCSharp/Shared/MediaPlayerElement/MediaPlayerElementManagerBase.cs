#nullable enable
using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Base class for managers used by MediaPlayerElement or PlaybackControls
    /// </summary>
    public abstract class MediaPlayerElementManagerBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MediaPlayerElementManagerBase"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public MediaPlayerElementManagerBase(IDispatcher? dispatcher = null)
        {
            Dispatcher = dispatcher;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MediaPlayerElementManagerBase()
        {
            Dispose();
        }


        private IDispatcher? Dispatcher { get; }

        private IVideoControl? _videoView;
        /// <summary>
        /// Gets or sets the video view
        /// </summary>
        public IVideoControl? VideoView
        {
            get => _videoView!;
            set
            {
                if (_videoView != value)
                {
                    var oldValue = _videoView;
                    _videoView = value;
                    OnVideoViewChanged(oldValue, value);
                }
            }
        }

        private Shared.MediaPlayer? _mediaPlayer;
        /// <summary>
        /// Gets or sets the media player
        /// </summary>
        public Shared.MediaPlayer? MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                if (_mediaPlayer != value)
                {
                    if (_mediaPlayer != null)
                    {
                        UnsubscribeEvents(_mediaPlayer);
                    }
                    _mediaPlayer = value;
                    if (value != null)
                    {
                        SubscribeEvents(value);
                    }
                    OnMediaPlayerChanged();
                }
            }
        }

        /// <summary>
        /// Called when <see cref="VideoView"/> property value changes
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        protected virtual void OnVideoViewChanged(IVideoControl? oldValue, IVideoControl? newValue)
        {
        }

        /// <summary>
        /// Called when <see cref="MediaPlayer"/> property value changes
        /// </summary>
        protected virtual void OnMediaPlayerChanged()
        {
            Initialize();
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected virtual void SubscribeEvents(Shared.MediaPlayer mediaPlayer)
        {
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected virtual void UnsubscribeEvents(Shared.MediaPlayer mediaPlayer)
        {
        }

        /// <summary>
        /// Initialization method called when <see cref="MediaPlayer"/> property changed or the controls are initialized
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Schedules the provided callback on the UI thread from a worker threa
        /// </summary>
        /// <param name="action">The callback on which the dispatcher returns when the event is dispatched</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        protected Task DispatcherInvokeAsync(Action action)
        {
            if (Dispatcher == null)
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return Dispatcher.InvokeAsync(action);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public virtual void Dispose()
        {
            if (MediaPlayer != null)
            {
                UnsubscribeEvents(MediaPlayer);
            }
            VideoView = null;
            MediaPlayer = null;
        }
    }
}
#nullable restore
