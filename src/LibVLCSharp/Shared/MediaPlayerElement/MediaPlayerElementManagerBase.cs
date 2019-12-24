using System;
using System.Threading.Tasks;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Base class for managers used by MediaPlayerElement or PlaybackControls
    /// </summary>
    internal abstract class MediaPlayerElementManagerBase : IDisposable
    {
        /// <summary>
        /// Occurs when <see cref="VideoView"/> property changed
        /// </summary>
        protected EventHandler? VideoViewChanged;

        /// <summary>
        /// Occurs when <see cref="LibVLC"/> property changed
        /// </summary>
        protected EventHandler? LibVLCChanged;

        /// <summary>
        /// Occurs when <see cref="MediaPlayer"/> property changed
        /// </summary>
        protected EventHandler? MediaPlayerChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="MediaPlayerElementManagerBase"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        public MediaPlayerElementManagerBase(IDispatcher? dispatcher)
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
            get => _videoView;
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

        private LibVLC? _libVLC;
        /// <summary>
        /// Gets or sets the <see cref="LibVLC"/> instance
        /// </summary>
        public LibVLC? LibVLC
        {
            get => _libVLC;
            set
            {
                if (_libVLC != value)
                {
                    var oldValue = _libVLC;
                    _libVLC = value;
                    OnLibVLCChanged(oldValue, value);
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
                    var oldValue = _mediaPlayer;
                    _mediaPlayer = value;
                    OnMediaPlayerChanged(oldValue, value);
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
            VideoViewChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when <see cref="LibVLC"/> property value changes
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        protected virtual void OnLibVLCChanged(LibVLC? oldValue, LibVLC? newValue)
        {
            LibVLCChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when <see cref="MediaPlayer"/> property value changes
        /// </summary>
        protected virtual void OnMediaPlayerChanged(MediaPlayer? oldValue, MediaPlayer? newValue)
        {
            if (oldValue != null)
            {
                UnsubscribeEvents(oldValue);
            }
            if (newValue != null)
            {
                SubscribeEvents(newValue);
            }
            MediaPlayerChanged?.Invoke(this, EventArgs.Empty);
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
        /// Fires an event using dispatcher
        /// </summary>
        /// <param name="eventHandler">event handler</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        protected Task DispatcherInvokeEventHandlerAsync(EventHandler? eventHandler)
        {
            if (eventHandler == null)
            {
                return Task.CompletedTask;
            }
            return DispatcherInvokeAsync(() => eventHandler(this, EventArgs.Empty));
        }

        /// <summary>
        /// Fires an event using dispatcher
        /// </summary>
        /// <typeparam name="TEventArgs">event args type</typeparam>
        /// <param name="eventHandler">event handler</param>
        /// <param name="eventArgs">event args</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        protected Task DispatcherInvokeEventHandlerAsync<TEventArgs>(EventHandler<TEventArgs>? eventHandler, TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            if (eventHandler == null)
            {
                return Task.CompletedTask;
            }

            return DispatcherInvokeAsync(() => eventHandler(this, eventArgs));
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
            LibVLC = null;
        }
    }
}
