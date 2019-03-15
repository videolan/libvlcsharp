using System;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Xamarin.Essentials;
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

            ButtonStyle = Resources[nameof(ButtonStyle)] as Style;
            ControlsPanelStyle = Resources[nameof(ControlsPanelStyle)] as Style;
            SeekBarStyle = Resources[nameof(SeekBarStyle)] as Style;
            RemainingTimeLabelStyle = Resources[nameof(RemainingTimeLabelStyle)] as Style;
            BufferingProgressBarStyle = Resources[nameof(BufferingProgressBarStyle)] as Style;

            DeviceDisplay.MainDisplayInfoChanged += (sender, e) => UpdateZoom();
        }

        private VisualElement ControlsPanel { get; set; }
        private Button PlayPauseButton { get; set; }
        private Button ZoomButton { get; set; }
        private Slider SeekBar { get; set; }
        private Label RemainingTimeLabel { get; set; }
        private object LastFadeOutTimer { get; set; }
        private object SeekBarTimer { get; set; }

        /// <summary>
        /// Identifies the <see cref="BufferingProgressBarStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty BufferingProgressBarStyleProperty = BindableProperty.Create(nameof(BufferingProgressBarStyle),
            typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets the controls panel style.
        /// </summary>
        public Style BufferingProgressBarStyle
        {
            get => (Style)GetValue(BufferingProgressBarStyleProperty);
            set => SetValue(BufferingProgressBarStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ControlsPanelStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ControlsPanelStyleProperty = BindableProperty.Create(nameof(ControlsPanelStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets the controls panel style.
        /// </summary>
        public Style ControlsPanelStyle
        {
            get => (Style)GetValue(ControlsPanelStyleProperty);
            set => SetValue(ControlsPanelStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SeekBarStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty SeekBarStyleProperty = BindableProperty.Create(nameof(SeekBarStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets the seek bar style.
        /// </summary>
        public Style SeekBarStyle
        {
            get => (Style)GetValue(SeekBarStyleProperty);
            set => SetValue(SeekBarStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ButtonStyleProperty = BindableProperty.Create(nameof(ButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets the buttons style.
        /// </summary>
        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RemainingTimeLabelStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty RemainingTimeLabelStyleProperty = BindableProperty.Create(nameof(RemainingTimeLabelStyle),
            typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets the remaining time label style.
        /// </summary>
        public Style RemainingTimeLabelStyle
        {
            get => (Style)GetValue(RemainingTimeLabelStyleProperty);
            set => SetValue(RemainingTimeLabelStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), typeof(MediaPlayer),
            typeof(PlaybackControls), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets or sets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => (MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BufferingProgress"/> dependency property.
        /// </summary>
        public static readonly BindableProperty BufferingProgressProperty = BindableProperty.Create(nameof(BufferingProgress), typeof(double),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets a value corresponding to the buffering progress.
        /// </summary>
        public double BufferingProgress
        {
            get => (double)GetValue(BufferingProgressProperty);
            set => SetValue(BufferingProgressProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the playback position within the media.
        /// </summary>
        public TimeSpan Position
        {
            get => (TimeSpan)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Zoom"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(nameof(Zoom), typeof(bool), typeof(PlaybackControls),
            propertyChanged: ZoomPropertyChanged);
        /// <summary>
        /// Gets or sets a value indicating whether the video is zoomed.
        /// </summary>
        public bool Zoom
        {
            get => (bool)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
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
        /// Identifies the <see cref="IsSeekBarVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsSeekBarVisibleProperty = BindableProperty.Create(nameof(IsSeekBarVisible), typeof(bool),
            typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value that indicates whether the seek bar is shown.
        /// </summary>
        public bool IsSeekBarVisible
        {
            get => (bool)GetValue(IsSeekBarVisibleProperty);
            set => SetValue(IsSeekBarVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSeekEnabled"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsSeekEnabledProperty = BindableProperty.Create(nameof(IsSeekEnabled), typeof(bool),
            typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value that indicates whether a user can use the seek bar to find a location in the media.
        /// </summary>
        public bool IsSeekEnabled
        {
            get => (bool)GetValue(IsSeekEnabledProperty);
            set => SetValue(IsSeekEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsZoomButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsZoomButtonVisibleProperty = BindableProperty.Create(nameof(IsZoomButtonVisible),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value indicating whether the zoom button is shown.
        /// </summary>
        public bool IsZoomButtonVisible
        {
            get => (bool)GetValue(IsZoomButtonVisibleProperty);
            set => SetValue(IsZoomButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsZoomEnabled"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsZoomEnabledProperty = BindableProperty.Create(nameof(IsZoomEnabled),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value that indicates whether a user can zoom the media.
        /// </summary>
        public bool IsZoomEnabled
        {
            get => (bool)GetValue(IsZoomEnabledProperty);
            set => SetValue(IsZoomEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowAndHideAutomatically"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ShowAndHideAutomaticallyProperty = BindableProperty.Create(nameof(ShowAndHideAutomatically),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value that indicates whether the controls are shown and hidden automatically.
        /// </summary>
        public bool ShowAndHideAutomatically
        {
            get => (bool)GetValue(ShowAndHideAutomaticallyProperty);
            set => SetValue(ShowAndHideAutomaticallyProperty, value);
        }

        /// <summary>
        /// Called when the <see cref="Element.Parent"/> property has changed.
        /// </summary>
        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent != null)
            {
                PlayPauseButton = SetClickEventHandler(nameof(PlayPauseButton), PlayPauseButton_Clicked);
                ZoomButton = SetClickEventHandler(nameof(ZoomButton), ZoomButton_Clicked);
                ControlsPanel = this.FindChild<VisualElement>(nameof(ControlsPanel));
                SeekBar = this.FindChild<Slider>(nameof(SeekBar));
                RemainingTimeLabel = this.FindChild<Label>(nameof(RemainingTimeLabel));
                UpdateTime();
                if (SeekBar != null)
                {
                    SeekBar.ValueChanged += SeekBar_ValueChanged;
                }
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
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }

            switch (mediaPlayer.State)
            {
                case VLCState.Ended:
                case VLCState.Paused:
                case VLCState.Stopped:
                case VLCState.Error:
                case VLCState.NothingSpecial:
                    mediaPlayer.Play();
                    break;
                default:
                    mediaPlayer.Pause();
                    break;
            }
        }

        private void ZoomButton_Clicked(object sender, EventArgs e)
        {
            _ = FadeInAsync();
            Zoom = !Zoom;
        }

        private static void ZoomPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).UpdateZoom();
        }

        private void UpdateZoom()
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }

            if (Zoom)
            {
                MediaTrack? mediaTrack;
                try
                {
                    mediaTrack = mediaPlayer.Media?.Tracks?.FirstOrDefault(x => x.TrackType == TrackType.Video);
                }
                catch (Exception)
                {
                    mediaTrack = null;
                }
                if (mediaTrack == null)
                {
                    return;
                }

                var videoTrack = mediaTrack.Value.Data.Video;
                var videoWidth = videoTrack.Width;
                var videoHeight = videoTrack.Height;
                if (videoWidth == 0 || videoHeight == 0)
                {
                    return;
                }

                var sarDen = videoTrack.SarDen;
                var sarNum = videoTrack.SarNum;
                if (sarNum != sarDen)
                {
                    videoWidth = videoWidth * sarNum / sarDen;
                }

                var var = (double)videoWidth / videoHeight;
                var displayInfo = DeviceDisplay.MainDisplayInfo;
                var screenWidth = displayInfo.Width;
                var screenHeight = displayInfo.Height;
                var screenar = screenWidth / screenHeight;
                mediaPlayer.Scale = (float)(screenar >= var ? screenWidth / videoWidth : screenHeight / videoHeight);
            }
            else
            {
                mediaPlayer.Scale = 0;
            }
        }

        private void SeekBar_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _ = FadeInAsync();

            UpdateTime();

            var seekBarTimer = new object();
            SeekBarTimer = seekBarTimer;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (SeekBarTimer == seekBarTimer)
                {
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        mediaPlayer.Position = (float)(e.NewValue / ((Slider)sender).Maximum);
                    }
                    SeekBarTimer = null;
                }
                return false;
            });
        }

        private void UpdateTime(double? position = null)
        {
            if (RemainingTimeLabel == null)
            {
                return;
            }

            var length = MediaPlayer?.Length ?? 0;
            var time = position == null ? (SeekBar.Value * length / SeekBar.Maximum) : (double)position;
            var timeRemaining = TimeSpan.FromMilliseconds(length - time).ToShortString();
            if (RemainingTimeLabel.Text != timeRemaining)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    RemainingTimeLabel.Text = timeRemaining;
                });
            }
        }

        private void OnMediaPlayerChanged(MediaPlayer oldMediaPlayer, MediaPlayer newMediaPlayer)
        {
            if (oldMediaPlayer != null)
            {
                oldMediaPlayer.Buffering -= MediaPlayer_Buffering;
                oldMediaPlayer.EndReached -= MediaPlayer_EndReached;
                oldMediaPlayer.LengthChanged -= MediaPlayer_LengthChanged;
                oldMediaPlayer.MediaChanged -= MediaPlayer_MediaChanged;
                oldMediaPlayer.Paused -= MediaPlayer_Paused;
                oldMediaPlayer.Playing -= MediaPlayer_Playing;
                oldMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                oldMediaPlayer.Stopped -= MediaPlayer_Stopped;
                oldMediaPlayer.Vout -= MediaPlayer_VoutChanged;
            }

            if (newMediaPlayer != null)
            {
                newMediaPlayer.Buffering += MediaPlayer_Buffering;
                newMediaPlayer.EndReached += MediaPlayer_EndReached;
                newMediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
                newMediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
                newMediaPlayer.Paused += MediaPlayer_Paused;
                newMediaPlayer.Playing += MediaPlayer_Playing;
                newMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                newMediaPlayer.Stopped += MediaPlayer_Stopped;
                newMediaPlayer.Vout += MediaPlayer_VoutChanged;
            }
        }

        private void MediaPlayer_VoutChanged(object sender, MediaPlayerVoutEventArgs e)
        {
            UpdateZoom();
        }

        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            if (SeekBarTimer != null)
            {
                return;
            }

            var position = TimeSpan.FromMilliseconds((MediaPlayer?.Length ?? 0) * e.Position);
            var elapsedTime = (position - Position).TotalMilliseconds;
            if (elapsedTime > 0 && elapsedTime < 750)
            {
                return;
            }

            UpdateSeekBar(position, e.Position);
        }

        private void UpdateSeekBar(TimeSpan timeSpan, float position = 0)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Position = timeSpan;
                var seekBar = SeekBar;
                if (seekBar != null)
                {
                    seekBar.ValueChanged -= SeekBar_ValueChanged;
                    seekBar.Value = position * seekBar.Maximum;
                    seekBar.ValueChanged += SeekBar_ValueChanged;
                }
            });
            UpdateTime(timeSpan.TotalMilliseconds);
        }

        private void MediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            UpdateTime(MediaPlayer?.Position);
        }

        private void MediaPlayer_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            var value = (int)e.Cache / 100.0d;
            if (BufferingProgress != value)
            {
                Device.BeginInvokeOnMainThread(() => BufferingProgress = value);
            }
        }

        private void MediaPlayer_MediaChanged(object sender, MediaPlayerMediaChangedEventArgs e)
        {
            UpdateSeekBar(TimeSpan.Zero);
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            UpdateState(VLCState.Ended);
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
                case VLCState.Ended:
                case VLCState.Error:
                case VLCState.Paused:
                case VLCState.Stopped:
                    playPauseStateName = "PlayState";
                    break;
                default:
                    playPauseStateName = "PauseState";
                    break;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                _ = FadeInAsync();
                BufferingProgress = 0;
                VisualStateManager.GoToState(playPauseButton, playPauseStateName);
            });
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).OnMediaPlayerChanged((MediaPlayer)oldValue, (MediaPlayer)newValue);
        }

        private void StartFadeOutTimer()
        {
            var fadeOutTimer = new object();
            LastFadeOutTimer = fadeOutTimer;
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                if (LastFadeOutTimer != fadeOutTimer)
                {
                    return false;
                }

                _ = FadeOut(fadeOutTimer);
                return false;
            });
        }

        private async Task FadeOut(object fadeOutTimer)
        {
            if (LastFadeOutTimer == fadeOutTimer)
            {
                await ControlsPanel.FadeTo(0, 1000);
            }
            if (LastFadeOutTimer == fadeOutTimer)
            {
                ControlsPanel.IsVisible = false;
            }
        }

        /// <summary>
        /// Controls fade in.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FadeInAsync()
        {
            if (LastFadeOutTimer != null)
            {
                LastFadeOutTimer = null;
                ControlsPanel.IsVisible = true;
                await ControlsPanel.FadeTo(1);
            }
            if (ShowAndHideAutomatically && MediaPlayer?.State == VLCState.Playing)
            {
                StartFadeOutTimer();
            }
        }
    }
}
