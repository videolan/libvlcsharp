using System;
using System.Threading.Tasks;
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

            ButtonStyle = Resources[nameof(ButtonStyle)] as Style;
            ControlsPanelStyle = Resources[nameof(ControlsPanelStyle)] as Style;
            PositionSliderStyle = Resources[nameof(PositionSliderStyle)] as Style;
            BufferingProgressBarStyle = Resources[nameof(BufferingProgressBarStyle)] as Style;
        }

        private VisualElement ControlsPanel { get; set; }
        private Button PlayPauseButton { get; set; }
        private Slider PositionSlider { get; set; }
        private object LastFadeOutTimer { get; set; }
        private object PositionSliderTimer { get; set; }

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
        /// Identifies the <see cref="PositionSliderStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty PositionSliderStyleProperty = BindableProperty.Create(nameof(PositionSliderStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets the controls panel style.
        /// </summary>
        public Style PositionSliderStyle
        {
            get => (Style)GetValue(PositionSliderStyleProperty);
            set => SetValue(PositionSliderStyleProperty, value);
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
                PlayPauseButton = SetClickEventHandler(nameof(PlayPauseButton), PlayPauseButton_Clicked);
                ControlsPanel = this.FindChild<VisualElement>(nameof(ControlsPanel));
                PositionSlider = this.FindChild<Slider>(nameof(PositionSlider));
                if (PositionSlider != null)
                {
                    PositionSlider.ValueChanged += PositionSlider_ValueChanged;
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

        private void PositionSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _ = FadeInAsync();

            var positionSliderTimer = new object();
            PositionSliderTimer = positionSliderTimer;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (PositionSliderTimer == positionSliderTimer)
                {
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        mediaPlayer.Position = (float)(e.NewValue / ((Slider)sender).Maximum);
                    }
                    PositionSliderTimer = null;
                }
                return false;
            });
        }

        private void OnMediaPlayerChanged(MediaPlayer oldMediaPlayer, MediaPlayer newMediaPlayer)
        {
            if (oldMediaPlayer != null)
            {
                oldMediaPlayer.Buffering -= MediaPlayer_Buffering;
                oldMediaPlayer.EndReached -= MediaPlayer_EndReached;
                oldMediaPlayer.Paused -= MediaPlayer_Paused;
                oldMediaPlayer.Playing -= MediaPlayer_Playing;
                oldMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                oldMediaPlayer.Stopped -= MediaPlayer_Stopped;
            }

            if (newMediaPlayer != null)
            {
                newMediaPlayer.Buffering += MediaPlayer_Buffering;
                newMediaPlayer.EndReached += MediaPlayer_EndReached;
                newMediaPlayer.Paused += MediaPlayer_Paused;
                newMediaPlayer.Playing += MediaPlayer_Playing;
                newMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                newMediaPlayer.Stopped += MediaPlayer_Stopped;
            }
        }

        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Position = new TimeSpan((long)((MediaPlayer?.Length ?? 0) * 1000 * e.Position));
                var positionSlider = PositionSlider;
                if (positionSlider != null && PositionSliderTimer == null)
                {
                    positionSlider.ValueChanged -= PositionSlider_ValueChanged;
                    positionSlider.Value = e.Position * positionSlider.Maximum;
                    positionSlider.ValueChanged += PositionSlider_ValueChanged;
                }
            });
        }

        private void MediaPlayer_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            var value = (int)e.Cache / 100.0d;
            if (BufferingProgress != value)
            {
                Device.BeginInvokeOnMainThread(() => BufferingProgress = value);
            }
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
            if (MediaPlayer?.State == VLCState.Playing)
            {
                var fadeOutTimer = new object();
                LastFadeOutTimer = fadeOutTimer;
                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    if (LastFadeOutTimer != fadeOutTimer)
                    {
                        return false;
                    }

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (LastFadeOutTimer == fadeOutTimer)
                        {
                            await ControlsPanel.FadeTo(0, 1000);
                        }
                        if (LastFadeOutTimer == fadeOutTimer)
                        {
                            ControlsPanel.IsVisible = false;
                        }
                    });
                    return false;
                });
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
            StartFadeOutTimer();
        }
    }
}
