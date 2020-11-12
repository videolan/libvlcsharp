using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared.Structures;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Tracks manager base class
    /// </summary>
    /// <remarks>the <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work</remarks>
    internal abstract class TracksManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Occurs when tracks should be reinitialized
        /// </summary>
        public event EventHandler? TracksCleared;
        /// <summary>
        /// Occurs when a track is selected
        /// </summary>
        public event EventHandler<MediaPlayerESSelectedEventArgs>? TrackSelected;
        /// <summary>
        /// Occurs when a track is added
        /// </summary>
        public event EventHandler<MediaPlayerESAddedEventArgs>? TrackAdded;
        /// <summary>
        /// Occurs when a track is deleted
        /// </summary>
        public event EventHandler<MediaPlayerESDeletedEventArgs>? TrackDeleted;

        /// <summary>
        /// Initializes a new instance of <see cref="TracksManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        /// <param name="trackType">track type</param>
        public TracksManager(IDispatcher? dispatcher, TrackType trackType) : base(dispatcher)
        {
            TrackType = trackType;
            MediaPlayerChanged += async (sender, e) => await InitializeAsync();
        }

        private TrackType TrackType { get; }

        /// <summary>
        /// Gets the tracks descriptions
        /// </summary>
        public abstract IEnumerable<TrackDescription>? Tracks { get; }

        /// <summary>
        /// Gets the current track identifier
        /// </summary>
        /// <remarks>returns -1 if no active input</remarks>
        public abstract int CurrentTrackId { get; set; }

        /// <summary>
        /// Gets the current track identifier
        /// </summary>
        protected int GetCurrentTrackId(int? trackId)
        {
            return trackId ?? -1;
        }

        /// <summary>
        /// Sets the current track identifier
        /// </summary>
        /// <param name="set">setter</param>
        protected void SetCurrentTrackId(Action<MediaPlayer> set)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                set(mediaPlayer);
            }
        }

        /// <summary>
        /// Gets the track description
        /// </summary>
        /// <param name="trackId">track identifier</param>
        /// <returns>the track description</returns>
        public TrackDescription? GetTrackDescription(int trackId)
        {
            return Tracks?.FirstOrDefault(t => t.Id == trackId);
        }

        private Task OnTrackChangedAsync<TEventArgs>(TrackType trackType, EventHandler<TEventArgs>? eventHandler, TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            if (TrackType == trackType)
            {
                return DispatcherInvokeEventHandlerAsync(eventHandler, eventArgs);
            }
            return Task.CompletedTask;
        }

        private async void OnTrackSelectedAsync(object? sender, MediaPlayerESSelectedEventArgs e)
        {
            await OnTrackChangedAsync(e.Type, TrackSelected, e);
        }

        private async void OnTrackAddedAsync(object? sender, MediaPlayerESAddedEventArgs e)
        {
            await OnTrackChangedAsync(e.Type, TrackAdded, e);
        }

        private async void OnTrackDeletedAsync(object? sender, MediaPlayerESDeletedEventArgs e)
        {
            await OnTrackChangedAsync(e.Type, TrackDeleted, e);
        }

        private async void OnStoppedAsync(object? sender, EventArgs e)
        {
            await OnTracksClearedAsync();
        }

        private Task OnTracksClearedAsync()
        {
            return DispatcherInvokeEventHandlerAsync(TracksCleared);
        }

        private Task InitializeAsync()
        {
            return OnTracksClearedAsync();
        }

        /// <summary>
        /// Subscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void SubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.SubscribeEvents(mediaPlayer);
            mediaPlayer.Stopped += OnStoppedAsync;
            mediaPlayer.EndReached += OnStoppedAsync;
            mediaPlayer.ESSelected += OnTrackSelectedAsync;
            mediaPlayer.ESAdded += OnTrackAddedAsync;
            mediaPlayer.ESDeleted += OnTrackDeletedAsync;
        }

        /// <summary>
        /// Unsubscribe media player events
        /// </summary>
        /// <param name="mediaPlayer">media player</param>
        protected override void UnsubscribeEvents(MediaPlayer mediaPlayer)
        {
            base.UnsubscribeEvents(mediaPlayer);
            mediaPlayer.Stopped -= OnStoppedAsync;
            mediaPlayer.EndReached -= OnStoppedAsync;
            mediaPlayer.ESSelected -= OnTrackSelectedAsync;
            mediaPlayer.ESAdded -= OnTrackAddedAsync;
            mediaPlayer.ESDeleted -= OnTrackDeletedAsync;
        }
    }
}
