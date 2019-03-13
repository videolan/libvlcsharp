using System;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Represents the playback controls for a <see cref="LibVLCSharp.Shared.MediaPlayer"/>.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaybackControls : TemplatedView
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PlaybackControls"/> class.
        /// </summary>
        public PlaybackControls()
        {
            InitializeComponent();
        }

        private Button PlayPauseButton { get; set; }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), typeof(MediaPlayer),
            typeof(PlaybackControls), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => (MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BufferingProgress"/> dependency property.
        /// </summary>
        public static readonly BindableProperty BufferingProgressProperty = BindableProperty.Create(nameof(BufferingProgress), typeof(double?),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets a value corresponding to the buffering progress
        /// </summary>
        public double? BufferingProgress
        {
            get => (double?)GetValue(BufferingProgressProperty);
            set => SetValue(BufferingProgressProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsPlayPauseButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsPlayPauseButtonVisibleProperty = BindableProperty.Create(nameof(IsPlayPauseButtonVisible),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value indicating whether the play/pause button is shown.
        /// </summary>
        public bool IsPlayPauseButtonVisible
        {
            get => (bool)GetValue(IsPlayPauseButtonVisibleProperty);
            set => SetValue(IsPlayPauseButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsPlayPauseEnabled"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsPlayPauseEnabledProperty = BindableProperty.Create(nameof(IsPlayPauseEnabled),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value indicating whether a user can play/pause the media.
        /// </summary>
        public bool IsPlayPauseEnabled
        {
            get => (bool)GetValue(IsPlayPauseEnabledProperty);
            set => SetValue(IsPlayPauseEnabledProperty, value);
        }

        /// <summary>
        /// Called when the <see cref="Element.Parent"/> property has changed.
        /// </summary>
        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent != null)
            {
                PlayPauseButton = SetClickEventHandler("PlayPauseButton", PlayPauseButton_Clicked);
            }
        }

        private Button SetClickEventHandler(string name, EventHandler eventHandler)
        {
            var button = this.FindChild<Button>(name);
            if (button != null)
            {
                button.Clicked += (sender, e) => OnButtonClicked(sender, e, eventHandler);
            }
            return button;
        }

        private void OnButtonClicked(object sender, EventArgs e, EventHandler eventHandler)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                try
                {
                    eventHandler?.Invoke(sender, e);
                }
                catch (Exception)
                {
                    //TODO
                }
            }
        }

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
        {
            switch (MediaPlayer.State)
            {
                case VLCState.Ended:
                case VLCState.Paused:
                case VLCState.Stopped:
                case VLCState.Error:
                case VLCState.NothingSpecial:
                    MediaPlayer.Play();
                    break;
                default:
                    MediaPlayer.Pause();
                    break;
            }
        }

        private void OnMediaPlayerChanged(MediaPlayer oldMediaPlayer, MediaPlayer newMediaPlayer)
        {
            if (oldMediaPlayer != null)
            {
                oldMediaPlayer.Buffering -= MediaPlayer_Buffering;
            }

            if (newMediaPlayer != null)
            {
                newMediaPlayer.Buffering += MediaPlayer_Buffering;
                newMediaPlayer.Playing += MediaPlayer_Playing;
                newMediaPlayer.Paused += MediaPlayer_Paused;
                newMediaPlayer.Stopped += MediaPlayer_Stopped;
            }
        }

        private void MediaPlayer_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => BufferingProgress = e.Cache == 100 ? (double?)null : e.Cache / 100);
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            UpdateState(VLCState.Playing);
        }

        private void MediaPlayer_Paused(object sender, EventArgs e)
        {
            UpdateState(VLCState.Paused);
        }

        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {
            UpdateState(VLCState.Stopped);
        }

        private void UpdateState(VLCState state, double? bufferingProgress = null)
        {
            var playPauseButton = PlayPauseButton;
            if (playPauseButton == null)
            {
                return;
            }

            string playPauseStateName;
            switch (state)
            {
                case VLCState.Stopped:
                case VLCState.Paused:
                    playPauseStateName = "PlayState";
                    break;
                default:
                    playPauseStateName = "PauseState";
                    break;
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                BufferingProgress = null;
                VisualStateManager.GoToState(playPauseButton, playPauseStateName);
            });
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).OnMediaPlayerChanged((MediaPlayer)oldValue, (MediaPlayer)newValue);
        }
    }
}
