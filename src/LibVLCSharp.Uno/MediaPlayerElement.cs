using System;
using LibVLCSharp.Shared;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Represents an object that uses a <see cref="Shared.MediaPlayer"/> to render audio and video to the display
    /// </summary>
    public partial class MediaPlayerElement : ContentControl
    {
        /// <summary>
        /// Occurs when the <see cref="MediaPlayerElement"/> is fully loaded
        /// </summary>
        public event EventHandler<InitializedEventArgs>? Initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerElement"/> class
        /// </summary>
        public MediaPlayerElement()
        {
            DefaultStyleKey = typeof(MediaPlayerElement);
        }

        private VideoView? VideoView { get; set; }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property
        /// </summary>
        public static readonly DependencyProperty LibVLCProperty = DependencyProperty.Register(nameof(LibVLC), typeof(LibVLC),
            typeof(MediaPlayerElement), new PropertyMetadata(null, (d, args) => ((MediaPlayerElement)d).OnLibVLCChanged()));
        /// <summary>
        /// Gets or sets the <see cref="Shared.LibVLC"/> instance
        /// </summary>
        public LibVLC? LibVLC
        {
            get => (LibVLC)GetValue(LibVLCProperty);
            set => SetValue(LibVLCProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer), typeof(Shared.MediaPlayer),
            typeof(MediaPlayerElement), new PropertyMetadata(null, (d, args) => ((MediaPlayerElement)d).OnMediaPlayerChanged()));
        /// <summary>
        /// Gets the <see cref="Shared.MediaPlayer"/> instance
        /// </summary>
        public Shared.MediaPlayer? MediaPlayer
        {
            get => (Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PlaybackControls"/> dependency property
        /// </summary>
        private static readonly DependencyProperty PlaybackControlsProperty = DependencyProperty.Register(nameof(PlaybackControls),
            typeof(PlaybackControls), typeof(MediaPlayerElement),
            new PropertyMetadata(null, (d, args) => ((MediaPlayerElement)d).OnPlaybackControlsChanged((PlaybackControls)args.OldValue)));
        /// <summary>
        /// Gets or sets the playback controls for the media
        /// </summary>
        public PlaybackControls? PlaybackControls
        {
            get => (PlaybackControls)GetValue(PlaybackControlsProperty);
            set => SetValue(PlaybackControlsProperty, value);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="Control.ApplyTemplate"/>.
        /// In simplest terms, this means the method is called just before a UI element displays in your app
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VideoView = (VideoView)GetTemplateChild("VideoView");
            VideoView.Initialized += (sender, e) => Initialized?.Invoke(this, e);
            var playbackControls = PlaybackControls;
            if (playbackControls == null)
            {
                PlaybackControls = new PlaybackControls();
            }
            else
            {
                playbackControls.VideoView = VideoView;
            }
            if (GetTemplateChild("ContentPresenter") is UIElement contentGrid)
            {
                contentGrid.AddHandler(PointerEnteredEvent, new PointerEventHandler(OnPointerMoved), true);
                contentGrid.AddHandler(PointerMovedEvent, new PointerEventHandler(OnPointerMoved), true);
                contentGrid.AddHandler(TappedEvent, new TappedEventHandler(OnPointerMoved), true);
            }
        }

        private void OnPointerMoved(object? sender, RoutedEventArgs e)
        {
            PlaybackControls?.Show();
        }

        /// <summary>
        /// Raises the <see cref="Initialized"/> event
        /// </summary>
        /// <param name="initializedEventArgs">event args</param>
        protected virtual void OnInitialized(InitializedEventArgs initializedEventArgs)
        {
            Initialized?.Invoke(this, initializedEventArgs);
        }

        private void OnLibVLCChanged()
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.LibVLC = LibVLC;
            }
        }

        private void OnMediaPlayerChanged()
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.MediaPlayer = MediaPlayer;
            }
        }

        private void OnPlaybackControlsChanged(PlaybackControls? oldValue = null)
        {
            if (oldValue != null)
            {
                oldValue.VideoView = null;
            }

            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.VideoView = VideoView;
                playbackControls.LibVLC = LibVLC;
                playbackControls.MediaPlayer = MediaPlayer;
            }
        }
    }
}
