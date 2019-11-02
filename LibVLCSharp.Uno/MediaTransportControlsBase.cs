using System;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace LibVLCSharp.Uno
{
    /// <summary>
    /// Represents the playback controls for a media player element
    /// </summary>
    public abstract partial class MediaTransportControlsBase : Control
    {
        private const string PlayState = "PlayState";
        private const string PauseState = "PauseState";
        private const string DisabledState = "Disabled";
        private const string NormalState = "Normal";
        private const string BufferingState = "Buffering";
        private const string ControlPanelFadeInState = "ControlPanelFadeIn";
        private const string ControlPanelFadeOutState = "ControlPanelFadeOut";

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTransportControlsBase"/> class
        /// </summary>
        public MediaTransportControlsBase()
        {
            DefaultStyleKey = typeof(MediaTransportControls);
            AspectRatioManager = new AspectRatioManager();
            Timer.Tick += Timer_Tick;
        }

        private AspectRatioManager AspectRatioManager { get; }
        private DispatcherTimer Timer { get; set; } = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };
        private bool IsVolumeFlyoutOpen { get; set; }

        /// <summary>
        /// Gets the <see cref="ResourceLoader"/>
        /// </summary>
        protected ResourceLoader? ResourceLoader => ResourceLoader.GetForCurrentView("LibVLCSharp.Uno/Resources");

        private FrameworkElement? LeftSeparator { get; set; }
        private FrameworkElement? RightSeparator { get; set; }
        private CommandBar? CommandBar { get; set; }
        private Slider? VolumeSlider { get; set; }
        private Slider? ProgressSlider { get; set; }
        private TextBlock? TimeElapsedElement { get; set; }
        private TextBlock? TimeRemainingElement { get; set; }
        private FrameworkElement? TimeTextGrid { get; set; }
        private FrameworkElement? PlayPauseButton { get; set; }
        private FrameworkElement? PlayPauseButtonOnLeft { get; set; }
        private FrameworkElement? StopButton { get; set; }
        private FrameworkElement? CastButton { get; set; }
        private FrameworkElement? ZoomButton { get; set; }
        private FrameworkElement? FullWindowButton { get; set; }

        /// <summary>
        /// Identifies the <see cref="VideoView"/> dependency property
        /// </summary>
        public static readonly DependencyProperty VideoViewProperty = DependencyProperty.Register(nameof(VideoView), typeof(FrameworkElement),
            typeof(MediaTransportControlsBase), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the video view
        /// </summary>
        public FrameworkElement? VideoView
        {
            get => (FrameworkElement)GetValue(VideoViewProperty);
            set => SetValue(VideoViewProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer), typeof(Shared.MediaPlayer),
            typeof(MediaTransportControlsBase), new PropertyMetadata(null, (d, args) =>
            ((MediaTransportControlsBase)d).OnMediaPlayerChanged((Shared.MediaPlayer)args.OldValue, (Shared.MediaPlayer)args.NewValue)));
        /// <summary>
        /// Gets or sets the <see cref="Shared.MediaPlayer"/> instance
        /// </summary>
        public Shared.MediaPlayer? MediaPlayer
        {
            get => (Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowAndHideAutomatically"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ShowAndHideAutomaticallyProperty = DependencyProperty.Register(nameof(ShowAndHideAutomatically),
            typeof(bool), typeof(MediaTransportControlsBase),
            new PropertyMetadata(true, (d, args) => ((MediaTransportControlsBase)d).OnShowAndHideAutomaticallyPropertyChanged()));
        /// <summary>
        /// Gets or sets a value that indicates whether the controls are shown and hidden automatically
        /// </summary>
        public bool ShowAndHideAutomatically
        {
            get => (bool)GetValue(ShowAndHideAutomaticallyProperty);
            set => SetValue(ShowAndHideAutomaticallyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsPlayPauseButtonVisible"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsPlayPauseButtonVisibleProperty = DependencyProperty.Register(nameof(IsPlayPauseButtonVisible),
            typeof(bool), typeof(MediaTransportControlsBase),
            new PropertyMetadata(true, (d, args) => ((MediaTransportControlsBase)d).UpdatePlayPauseAvailability()));
        /// <summary>
        /// Gets or sets a value indicating whether the play/pause button is shown
        /// </summary>
        public bool IsPlayPauseButtonVisible
        {
            get => (bool)GetValue(IsPlayPauseButtonVisibleProperty);
            set => SetValue(IsPlayPauseButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PlayPauseButtonStyle"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PlayPauseButtonStyleProperty = DependencyProperty.Register(nameof(PlayPauseButtonStyle),
            typeof(Style), typeof(MediaTransportControlsBase), new PropertyMetadata(null, (d, args) =>
            {
                UpdateStyle(d, args.NewValue, d => d.PlayPauseButton);
                UpdateStyle(d, args.NewValue, d => d.PlayPauseButtonOnLeft);
            }));
        /// <summary>
        /// Gets or sets the play/pause button style
        /// </summary>
        public Style? PlayPauseButtonStyle
        {
            get => (Style)GetValue(PlayPauseButtonStyleProperty);
            set => SetValue(PlayPauseButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsStopButtonVisible"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStopButtonVisibleProperty { get; } = DependencyProperty.Register(nameof(IsStopButtonVisible), typeof(bool),
            typeof(MediaTransportControlsBase), new PropertyMetadata(false, (d, e) => ((MediaTransportControlsBase)d).UpdateStopButton()));
        /// <summary>
        /// Gets or sets a value indicating whether the stop button is shown.
        /// </summary>
        public bool IsStopButtonVisible
        {
            get => (bool)GetValue(IsStopButtonVisibleProperty);
            set => SetValue(IsStopButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsStopEnabled"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStopEnabledProperty { get; } = DependencyProperty.Register(nameof(IsStopEnabled), typeof(bool),
            typeof(MediaTransportControlsBase), new PropertyMetadata(true, (d, e) => ((MediaTransportControlsBase)d).UpdateStopButton()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can stop the media playback.
        /// </summary>
        public bool IsStopEnabled
        {
            get => (bool)GetValue(IsStopEnabledProperty);
            set => SetValue(IsStopEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StopButtonStyle"/> dependency property
        /// </summary>
        public static readonly DependencyProperty StopButtonStyleProperty = DependencyProperty.Register(nameof(StopButtonStyle),
            typeof(Style), typeof(MediaTransportControlsBase),
            new PropertyMetadata(null, (d, args) => UpdateStyle(d, args.NewValue, d => d.StopButton)));
        /// <summary>
        /// Gets or sets the stop button style
        /// </summary>
        public Style? StopButtonStyle
        {
            get => (Style)GetValue(StopButtonStyleProperty);
            set => SetValue(StopButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsZoomButtonVisible"/> dependency property.
        /// </summary>
        public static DependencyProperty IsZoomButtonVisibleProperty { get; } = DependencyProperty.Register(nameof(IsZoomButtonVisible), typeof(bool),
            typeof(MediaTransportControlsBase), new PropertyMetadata(true, (d, e) => ((MediaTransportControlsBase)d).UpdateZoomButton()));
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
        public static DependencyProperty IsZoomEnabledProperty { get; } = DependencyProperty.Register(nameof(IsZoomEnabled), typeof(bool),
            typeof(MediaTransportControlsBase), new PropertyMetadata(true, (d, e) => ((MediaTransportControlsBase)d).UpdateZoomButton()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can zoom the media.
        /// </summary>
        public bool IsZoomEnabled
        {
            get => (bool)GetValue(IsZoomEnabledProperty);
            set => SetValue(IsZoomEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ZoomButtonStyle"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ZoomButtonStyleProperty = DependencyProperty.Register(nameof(ZoomButtonStyle),
            typeof(Style), typeof(MediaTransportControlsBase),
            new PropertyMetadata(null, (d, args) => UpdateStyle(d, args.NewValue, d => d.ZoomButton)));
        /// <summary>
        /// Gets or sets the zoom button style
        /// </summary>
        public Style? ZoomButtonStyle
        {
            get => (Style)GetValue(ZoomButtonStyleProperty);
            set => SetValue(ZoomButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSeekBarVisible"/> dependency property
        /// </summary>
        public static DependencyProperty IsSeekBarVisibleProperty { get; } = DependencyProperty.Register(nameof(IsSeekBarVisible), typeof(bool), typeof(MediaTransportControls),
            new PropertyMetadata(true, (d, e) => ((MediaTransportControls)d).UpdateSeekBarVisibility()));
        /// <summary>
        /// Gets or sets a value indicating whether the seek bar is shown
        /// </summary>
        public bool IsSeekBarVisible
        {
            get => (bool)GetValue(IsSeekBarVisibleProperty);
            set => SetValue(IsSeekBarVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSeekBarEnabled"/> dependency property
        /// </summary>
        public static DependencyProperty IsSeekBarEnabledProperty { get; } = DependencyProperty.Register(nameof(IsSeekBarEnabled), typeof(bool), typeof(MediaTransportControls),
            new PropertyMetadata(true, (d, e) => ((MediaTransportControls)d).UpdateSeekAvailability()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can use the seek bar to find a location in the media
        /// </summary>
        public bool IsSeekBarEnabled
        {
            get => (bool)GetValue(IsSeekBarEnabledProperty);
            set => SetValue(IsSeekBarEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SeekBarStyle"/> dependency property
        /// </summary>
        public static readonly DependencyProperty SeekBarStyleProperty = DependencyProperty.Register(nameof(SeekBarStyle), typeof(Style),
            typeof(MediaTransportControlsBase), new PropertyMetadata(null, (d, args) => UpdateStyle(d, args.NewValue, d => d.ProgressSlider)));
        /// <summary>
        /// Gets or sets the seek bar style
        /// </summary>
        public Style? SeekBarStyle
        {
            get => (Style)GetValue(SeekBarStyleProperty);
            set => SetValue(SeekBarStyleProperty, value);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. 
        /// In simplest terms, this means the method is called just before a UI element displays in your app.
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PointerEntered += OnPointerMoved;
            PointerMoved += OnPointerMoved;
            Tapped += OnPointerMoved;
            PointerExited += (sender, e) => Show();

            LeftSeparator = GetTemplateChild("LeftSeparator") as FrameworkElement;
            RightSeparator = GetTemplateChild("RightSeparator") as FrameworkElement;
            CommandBar = GetTemplateChild("MediaControlsCommandBar") as CommandBar;
            if (CommandBar != null)
            {
                CommandBar.LayoutUpdated += CommandBar_LayoutUpdated;
            }

            ProgressSlider = GetTemplateChild("ProgressSlider") as Slider;
            if (ProgressSlider != null)
            {
                ProgressSlider.Minimum = 0;
                ProgressSlider.Maximum = 100;
                ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;
            }
            TimeElapsedElement = GetTemplateChild("TimeElapsedElement") as TextBlock;
            TimeRemainingElement = GetTemplateChild("TimeRemainingElement") as TextBlock;
            TimeTextGrid = GetTemplateChild("TimeTextGrid") as FrameworkElement;

            PlayPauseButton = GetTemplateChild("PlayPauseButton") as FrameworkElement;
            PlayPauseButtonOnLeft = GetTemplateChild("PlayPauseButtonOnLeft") as FrameworkElement;
            StopButton = GetTemplateChild("StopButton") as FrameworkElement;
            CastButton = GetTemplateChild("CastButton") as FrameworkElement;
            ZoomButton = GetTemplateChild("ZoomButton") as FrameworkElement;
            FullWindowButton = GetTemplateChild("FullWindowButton") as FrameworkElement;
            var audioMuteButton = GetTemplateChild("AudioMuteButton");
            VolumeSlider = GetTemplateChild("VolumeSlider") as Slider;
            if (VolumeSlider != null)
            {
                VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            }
            if (GetTemplateChild("VolumeFlyout") is Flyout volumeFlyout)
            {
                volumeFlyout.Opened += VolumeFlyout_Opened;
                volumeFlyout.Closed += VolumeFlyout_Closed;
            }

            UpdateZoomButton();
            UpdateControl(CastButton, false);
            UpdateControl(FullWindowButton, false);

            SetButtonClick(PlayPauseButton, PlayPauseButton_Click);
            SetButtonClick(PlayPauseButtonOnLeft, PlayPauseButton_Click);
            SetButtonClick(StopButton, StopButton_Click);
            SetButtonClick(ZoomButton, ZoomButton_Click);
            SetButtonClick(audioMuteButton, AudioMuteButton_Click);

            SetToolTip(PlayPauseButton, "Play");
            SetToolTip(PlayPauseButtonOnLeft, "Play");
            SetToolTip(StopButton, "Stop");
            SetToolTip(ZoomButton, "AspectRatio");
            SetToolTip(audioMuteButton, "Mute");

            Reset();
        }

        private void VolumeFlyout_Closed(object sender, object e)
        {
            IsVolumeFlyoutOpen = false;
            Show(true);
        }

        private void VolumeFlyout_Opened(object sender, object e)
        {
            IsVolumeFlyoutOpen = true;
            Show(false);
        }

        /// <summary>
        /// Sets the value of the <see cref="ToolTipService.ToolTipProperty"/> for an object
        /// </summary>
        /// <param name="element">the object to which the attached property is written</param>
        /// <param name="resource">resource string</param>
        /// <param name="args">an object array that contains zero or more objects to format</param>
        protected abstract void SetToolTip(DependencyObject? element, string resource, params string[] args);

        private static void UpdateStyle(DependencyObject dependencyObject, object newValue,
            Func<MediaTransportControlsBase, FrameworkElement?> getFrameworkElement)
        {
            var mediaTransportControls = (MediaTransportControlsBase)dependencyObject;
            var frameworkElement = getFrameworkElement(mediaTransportControls);
            if (frameworkElement != null)
            {
                frameworkElement.Style = (Style)newValue;
            }
        }

        private void UpdateControl(FrameworkElement? control, bool visible, bool enabled = true)
        {
            if (control != null)
            {
                control.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                if (control is Control)
                {
                    ((Control)control).IsEnabled = enabled;
                }
            }
        }

        private void CommandBar_LayoutUpdated(object sender, object e)
        {
            var leftSeparator = LeftSeparator;
            var rightSeparator = RightSeparator;
            if (leftSeparator == null || rightSeparator == null)
            {
                return;
            }

            var commandBar = CommandBar!;
            var width = commandBar.PrimaryCommands
                .Where(el => !(el is AppBarSeparator) && ((FrameworkElement)el).Visibility == Visibility.Visible)
                .Sum(el => ((FrameworkElement)el).Width);
            width = (commandBar.ActualWidth - width) / 2;
            if (width >= 0 && leftSeparator.Width != width)
            {
                leftSeparator.Width = width;
                rightSeparator.Width = width;
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var progressSlider = (Slider)sender;
            var position = (float)(progressSlider.Value / progressSlider.Maximum);
            UpdateTime(position);
            MediaPlayer!.Position = position;
        }

        private void UpdateSeekBarPosition(float position = 0)
        {
            UpdateTime(position);
            if (ProgressSlider != null)
            {
                ProgressSlider.ValueChanged -= ProgressSlider_ValueChanged;
                try
                {
                    ProgressSlider.Value = position * ProgressSlider.Maximum;
                }
                finally
                {
                    ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;
                }
            }
        }

        private void UpdateTime(float position = 0)
        {
            if (TimeRemainingElement == null && TimeElapsedElement == null)
            {
                return;
            }

            var mediaPlayer = MediaPlayer;
            var state = mediaPlayer?.State;
            var length = mediaPlayer == null || state == VLCState.Ended || state == VLCState.Error || state == VLCState.NothingSpecial ||
                state == VLCState.Stopped ? 0 : mediaPlayer.Length;
            var time = position * length;
            var timeRemaining = TimeSpan.FromMilliseconds(length - time).ToShortString();
            var timeElapsed = TimeSpan.FromMilliseconds(time).ToShortString();
            if (TimeRemainingElement?.Text != timeRemaining)
            {
                TimeRemainingElement!.Text = timeRemaining;
            }
            if (TimeElapsedElement?.Text != timeElapsed)
            {
                TimeElapsedElement!.Text = timeElapsed;
            }
        }

        private void SetButtonClick(DependencyObject? dependencyObject, RoutedEventHandler eventHandler)
        {
            if (dependencyObject is ButtonBase button)
            {
                button.Click += eventHandler;
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            var mediaPlayer = MediaPlayer!;
            var state = mediaPlayer.State;
            string playPauseState;
            switch (state)
            {
                case VLCState.Ended:
                    mediaPlayer.Stop();
                    goto case VLCState.Stopped;
                case VLCState.Paused:
                case VLCState.Stopped:
                case VLCState.Error:
                case VLCState.NothingSpecial:
                    playPauseState = PauseState;
                    break;
                default:
                    playPauseState = PlayState;
                    break;
            }
            VisualStateManager.GoToState(this, playPauseState, true);
            if (playPauseState == PauseState)
            {
                mediaPlayer.Play();
            }
            else
            {
                mediaPlayer.Pause();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer?.Stop();
        }

        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            AspectRatioManager.ToggleZoom(VideoView, MediaPlayer);
        }

        private void AudioMuteButton_Click(object sender, RoutedEventArgs e)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                mediaPlayer.ToggleMute();
            }
        }

        private void OnMediaPlayerChanged(Shared.MediaPlayer oldValue, Shared.MediaPlayer newValue)
        {
            if (oldValue != null)
            {
                oldValue.Buffering -= MediaPlayer_BufferingAsync;
                oldValue.EncounteredError -= MediaPlayer_UpdateStateAsync;
                oldValue.EndReached -= MediaPlayer_UpdateStateAsync;
                oldValue.ESAdded -= MediaPlayer_TracksChanged;
                oldValue.ESDeleted -= MediaPlayer_TracksChanged;
                oldValue.LengthChanged -= MediaPlayer_LengthChangedAsync;
                oldValue.MediaChanged -= MediaPlayer_MediaChangedAsync;
                oldValue.NothingSpecial -= MediaPlayer_UpdateStateAsync;
                oldValue.PausableChanged -= MediaPlayer_PausableChangedAsync;
                oldValue.Paused -= MediaPlayer_UpdateStateAsync;
                oldValue.Playing -= MediaPlayer_UpdateStateAsync;
                oldValue.PositionChanged -= MediaPlayer_PositionChangedAsync;
                oldValue.SeekableChanged -= MediaPlayer_SeekableChangedAsync;
                oldValue.Stopped -= MediaPlayer_UpdateStateAsync;
                oldValue.VolumeChanged -= MediaPlayer_VolumeChanged;
                oldValue.Muted -= MediaPlayer_MuteChanged;
                oldValue.Unmuted -= MediaPlayer_MuteChanged;
            }

            if (newValue != null)
            {
                newValue.Buffering += MediaPlayer_BufferingAsync;
                newValue.EncounteredError += MediaPlayer_UpdateStateAsync;
                newValue.ESAdded += MediaPlayer_TracksChanged;
                newValue.ESDeleted += MediaPlayer_TracksChanged;
                newValue.EndReached += MediaPlayer_UpdateStateAsync;
                newValue.LengthChanged += MediaPlayer_LengthChangedAsync;
                newValue.MediaChanged += MediaPlayer_MediaChangedAsync;
                newValue.NothingSpecial += MediaPlayer_UpdateStateAsync;
                newValue.PausableChanged += MediaPlayer_PausableChangedAsync;
                newValue.Paused += MediaPlayer_UpdateStateAsync;
                newValue.Playing += MediaPlayer_UpdateStateAsync;
                newValue.PositionChanged += MediaPlayer_PositionChangedAsync;
                newValue.SeekableChanged += MediaPlayer_SeekableChangedAsync;
                newValue.Stopped += MediaPlayer_UpdateStateAsync;
                newValue.VolumeChanged += MediaPlayer_VolumeChanged;
                newValue.Muted += MediaPlayer_MuteChanged;
                newValue.Unmuted += MediaPlayer_MuteChanged;
            }

            Reset();
        }

        private void MediaPlayer_MuteChanged(object sender, EventArgs e)
        {
            UpdateMuteState();
        }

        private void MediaPlayer_VolumeChanged(object sender, MediaPlayerVolumeChangedEventArgs e)
        {
            UpdateVolume();
        }

        private async void MediaPlayer_BufferingAsync(object sender, MediaPlayerBufferingEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateState(e.Cache == 100 ? (VLCState?)null : VLCState.Buffering));
        }

        private async void MediaPlayer_LengthChangedAsync(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateTime(MediaPlayer!.Position));
        }

        private async void MediaPlayer_MediaChangedAsync(object sender, MediaPlayerMediaChangedEventArgs e)
        {
            await DispatcherRunAsync(Reset);
        }

        private async void MediaPlayer_PausableChangedAsync(object sender, MediaPlayerPausableChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdatePlayPauseAvailability(e.Pausable == 1));
        }

        private async void MediaPlayer_PositionChangedAsync(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateSeekBarPosition(e.Position));
        }

        private async void MediaPlayer_SeekableChangedAsync(object sender, MediaPlayerSeekableChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateSeekAvailability(e.Seekable == 1));
        }

        private async void MediaPlayer_UpdateStateAsync(object sender, EventArgs e)
        {
            await DispatcherRunAsync(() => UpdateState());
        }

        private void MediaPlayer_TracksChanged(object sender, EventArgs e)
        {
        }

        private void Reset()
        {
            UpdateState();
            UpdateSeekBarPosition();
            UpdatePlayPauseAvailability();
            UpdateStopButton();
            UpdateSeekAvailability();
            UpdateMuteState();
            UpdateVolume();
        }

        private async Task DispatcherRunAsync(DispatchedHandler handler)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
        }

        private void UpdateState(VLCState? state = null)
        {
            string statusState;
            string? playPauseState;
            state ??= MediaPlayer?.State ?? VLCState.NothingSpecial;
            switch (state)
            {
                case VLCState.Error:
                    //UpdateErrorMessage(state);
                    goto case VLCState.Stopped;
                case VLCState.Stopped:
                case VLCState.Ended:
                case VLCState.NothingSpecial:
                    UpdatePlayPauseAvailability(true);
                    UpdateSeekBarPosition();
                    UpdateSeekAvailability(false);
                    //UpdateTracksSelectionButtonAvailability(AudioTracksSelectionButton, AudioSelectionUnavailableState);
                    //UpdateTracksSelectionButtonAvailability(ClosedCaptionsSelectionButton, ClosedCaptionsSelectionUnavailableState);
                    goto case VLCState.Paused;
                case VLCState.Paused:
                    statusState = DisabledState;
                    playPauseState = PlayState;
                    break;
                case VLCState.Buffering:
                    statusState = BufferingState;
                    playPauseState = null;
                    break;
                default:
                    //ShowError(null);
                    statusState = NormalState;
                    playPauseState = PauseState;
                    break;
            }

            Show();
            VisualStateManager.GoToState(this, statusState, true);
            if (playPauseState != null)
            {
                var playPauseToolTip = playPauseState == PlayState ? "Play" : "Pause";
                SetToolTip(PlayPauseButton, playPauseToolTip);
                SetToolTip(PlayPauseButtonOnLeft, playPauseToolTip);
                VisualStateManager.GoToState(this, playPauseState, true);
            }
            UpdateStopButton();
            //UpdateKeepScreenOn();
        }

        private void UpdateSeekBarVisibility()
        {
            UpdateControl(ProgressSlider, IsSeekBarVisible);
            UpdateControl(TimeTextGrid, IsSeekBarVisible);
            UpdateSeekAvailability();
        }

        private void UpdateSeekAvailability(bool? seekable = null)
        {
            if (ProgressSlider != null)
            {
                ProgressSlider.IsEnabled = IsSeekBarEnabled && (seekable ?? MediaPlayer?.IsSeekable == true);
            }
        }

        private void UpdateMuteState()
        {
            VisualStateManager.GoToState(this, MediaPlayer?.Mute == true ? "MuteState" : "VolumeState", true);
        }

        private void UpdateVolume()
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }
            var volumeSlider = VolumeSlider;
            if (volumeSlider == null)
            {
                return;
            }
            volumeSlider.ValueChanged -= VolumeSlider_ValueChanged;
            try
            {
                volumeSlider.Value = mediaPlayer.Volume;
            }
            finally
            {
                volumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            }
        }

        private void UpdateStopButton()
        {
            var state = MediaPlayer?.State;
            UpdateControl(StopButton, IsStopButtonVisible && MediaPlayer?.Media != null,
                IsStopEnabled && state != null && state != VLCState.Ended && state != VLCState.Stopped);
        }

        private void UpdateZoomButton()
        {
            UpdateControl(ZoomButton, IsZoomButtonVisible, IsZoomEnabled);
        }

        private void StartTimer()
        {
            if (ShowAndHideAutomatically && !IsVolumeFlyoutOpen && MediaPlayer?.State == VLCState.Playing)
            {
                Timer.Start();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            Timer.Stop();
            VisualStateManager.GoToState(this, ControlPanelFadeOutState, true);
        }

        /// <summary>
        /// Shows the tranport controls if they're hidden
        /// </summary>
        public void Show()
        {
            Show(true);
        }

        private void Show(bool startTimer)
        {
            Timer.Stop();
            VisualStateManager.GoToState(this, ControlPanelFadeInState, true);
            if (startTimer)
            {
                StartTimer();
            }
        }

        private void UpdatePlayPauseAvailability(bool? pausable = null)
        {
            var mediaPlayer = MediaPlayer;
            var state = mediaPlayer?.State;
            var playPauseButtonVisible = IsPlayPauseButtonVisible && mediaPlayer != null && mediaPlayer.Media != null &&
                ((pausable ?? mediaPlayer?.CanPause) != false ||
                state != VLCState.Opening && state != VLCState.Playing && state != VLCState.Buffering);
            UpdateControl(PlayPauseButton, playPauseButtonVisible);
            UpdateControl(PlayPauseButtonOnLeft, playPauseButtonVisible);
        }

        private void OnShowAndHideAutomaticallyPropertyChanged()
        {
            if (ShowAndHideAutomatically)
            {
                StartTimer();
            }
            else
            {
                Show(false);
            }
        }

        private void OnPointerMoved(object sender, RoutedEventArgs e)
        {
            Show(false);
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (MediaPlayer != null)
            {
                var mute = MediaPlayer.Mute;
                MediaPlayer.Volume = (int)e.NewValue;
                if (mute)
                {
                    MediaPlayer.Mute = mute;
                }
            }
        }
    }
}
