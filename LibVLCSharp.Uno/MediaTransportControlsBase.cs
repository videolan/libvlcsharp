using System;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

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
            ResourceLoader = ResourceLoader.GetForCurrentView();
            Timer.Tick += Timer_Tick;
        }

        private DispatcherTimer Timer { get; set; } = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };

        /// <summary>
        /// Gets the <see cref="ResourceLoader"/>
        /// </summary>
        protected ResourceLoader? ResourceLoader { get; }

        private FrameworkElement? LeftSeparator { get; set; }
        private FrameworkElement? RightSeparator { get; set; }
        private CommandBar? CommandBar { get; set; }
        private Slider? ProgressSlider { get; set; }
        private TextBlock? TimeElapsedElement { get; set; }
        private TextBlock? TimeRemainingElement { get; set; }
        private FrameworkElement? TimeTextGrid { get; set; }
        private Control? PlayPauseButton { get; set; }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer), typeof(Shared.MediaPlayer),
            typeof(MediaTransportControlsBase), new PropertyMetadata(null, (d, args) =>
            ((MediaTransportControlsBase)d).OnMediaPlayerChanged((Shared.MediaPlayer)args.OldValue, (Shared.MediaPlayer)args.NewValue)));
        /// <summary>
        /// Gets the <see cref="Shared.MediaPlayer"/> instance
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
            new PropertyMetadata(true, (d, args) => ((MediaTransportControlsBase)d).UpdatePauseAvailability()));
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
            typeof(Style), typeof(MediaTransportControlsBase),
            new PropertyMetadata(null, (d, args) => UpdateStyle(d, args.NewValue, d => d.PlayPauseButton)));
        /// <summary>
        /// Gets or sets the play/pause button style
        /// </summary>
        public Style? PlayPauseButtonStyle
        {
            get => (Style)GetValue(PlayPauseButtonStyleProperty);
            set => SetValue(PlayPauseButtonStyleProperty, value);
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
            if (GetTemplateChild("RootGrid") is FrameworkElement rootGrid)
            {
                rootGrid.DoubleTapped += Grid_DoubleTapped;
                rootGrid.PointerEntered += OnPointerMoved;
                rootGrid.PointerMoved += OnPointerMoved;
                rootGrid.Tapped += OnPointerMoved;
                rootGrid.PointerExited += (sender, e) => Show();
            }

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

            PlayPauseButton = GetTemplateChild("PlayPauseButton") as Control;
            SetButtonClick(PlayPauseButton, PlayPauseButtons_Click);
            SetToolTip(PlayPauseButton, "Play");

            Reset();
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

        private void PlayPauseButtons_Click(object sender, RoutedEventArgs e)
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

        private void OnMediaPlayerChanged(Shared.MediaPlayer oldValue, Shared.MediaPlayer newValue)
        {
            if (oldValue != null)
            {
                oldValue.Buffering -= MediaPlayer_BufferingAsync;
                oldValue.EncounteredError -= MediaPlayer_EncounteredErrorAsync;
                oldValue.EndReached -= MediaPlayer_EndReachedAsync;
                oldValue.ESAdded -= MediaPlayer_TracksChanged;
                oldValue.ESDeleted -= MediaPlayer_TracksChanged;
                oldValue.LengthChanged -= MediaPlayer_LengthChangedAsync;
                oldValue.MediaChanged -= MediaPlayer_MediaChangedAsync;
                oldValue.NothingSpecial -= MediaPlayer_NothingSpecialAsync;
                oldValue.PausableChanged -= MediaPlayer_PausableChangedAsync;
                oldValue.Paused -= MediaPlayer_PausedAsync;
                oldValue.Playing -= MediaPlayer_PlayingAsync;
                oldValue.PositionChanged -= MediaPlayer_PositionChangedAsync;
                oldValue.SeekableChanged -= MediaPlayer_SeekableChangedAsync;
                oldValue.Stopped -= MediaPlayer_StoppedAsync;
            }

            if (newValue != null)
            {
                newValue.Buffering += MediaPlayer_BufferingAsync;
                newValue.EncounteredError += MediaPlayer_EncounteredErrorAsync;
                newValue.ESAdded += MediaPlayer_TracksChanged;
                newValue.ESDeleted += MediaPlayer_TracksChanged;
                newValue.EndReached += MediaPlayer_EndReachedAsync;
                newValue.LengthChanged += MediaPlayer_LengthChangedAsync;
                newValue.MediaChanged += MediaPlayer_MediaChangedAsync;
                newValue.NothingSpecial += MediaPlayer_NothingSpecialAsync;
                newValue.PausableChanged += MediaPlayer_PausableChangedAsync;
                newValue.Paused += MediaPlayer_PausedAsync;
                newValue.Playing += MediaPlayer_PlayingAsync;
                newValue.PositionChanged += MediaPlayer_PositionChangedAsync;
                newValue.SeekableChanged += MediaPlayer_SeekableChangedAsync;
                newValue.Stopped += MediaPlayer_StoppedAsync;
            }

            Reset();
        }

        private async void MediaPlayer_BufferingAsync(object sender, MediaPlayerBufferingEventArgs e)
        {
            await UpdateStateAsync(e.Cache == 100 ? (VLCState?)null : VLCState.Buffering);
        }

        private async void MediaPlayer_EncounteredErrorAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync(VLCState.Error);
        }

        private async void MediaPlayer_EndReachedAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync(VLCState.Ended);
        }

        private async void MediaPlayer_LengthChangedAsync(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateTime(MediaPlayer!.Position));
        }

        private async void MediaPlayer_MediaChangedAsync(object sender, MediaPlayerMediaChangedEventArgs e)
        {
            await DispatcherRunAsync(Reset);
        }

        private async void MediaPlayer_NothingSpecialAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync(VLCState.NothingSpecial);
        }

        private async void MediaPlayer_PausableChangedAsync(object sender, MediaPlayerPausableChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdatePauseAvailability(e.Pausable == 1));
        }

        private async void MediaPlayer_PausedAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync(VLCState.Paused);
        }

        private async void MediaPlayer_PlayingAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync(VLCState.Playing);
        }

        private async void MediaPlayer_PositionChangedAsync(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateSeekBarPosition(e.Position));
        }

        private async void MediaPlayer_SeekableChangedAsync(object sender, MediaPlayerSeekableChangedEventArgs e)
        {
            await DispatcherRunAsync(() => UpdateSeekAvailability(e.Seekable == 1));
        }

        private async void MediaPlayer_StoppedAsync(object sender, EventArgs e)
        {
            await UpdateStateAsync(VLCState.Stopped);
        }

        private void MediaPlayer_TracksChanged(object sender, EventArgs e)
        {
        }

        private void Reset()
        {
            UpdateState();
            UpdateSeekBarPosition();
            UpdatePauseAvailability();
            UpdateSeekAvailability();
        }

        private async Task DispatcherRunAsync(DispatchedHandler handler)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
        }

        private async Task UpdateStateAsync(VLCState? state = null)
        {
            await DispatcherRunAsync(() => UpdateState(state));
        }

        private void UpdateState(VLCState? state = null)
        {
            state ??= MediaPlayer?.State ?? VLCState.NothingSpecial;
            string statusState;
            string? playPauseState;
            switch (state)
            {
                case VLCState.Error:
                    //UpdateErrorMessage(state);
                    goto case VLCState.Stopped;
                case VLCState.Stopped:
                case VLCState.Ended:
                case VLCState.NothingSpecial:
                    UpdatePauseAvailability(true);
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
                VisualStateManager.GoToState(this, playPauseState, true);
            }
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

        private void StartTimer()
        {
            if (ShowAndHideAutomatically && MediaPlayer?.State == VLCState.Playing)
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

        private void UpdatePauseAvailability(bool? pausable = null)
        {
            var mediaPlayer = MediaPlayer;
            var state = mediaPlayer?.State;
            UpdateControl(PlayPauseButton,
                IsPlayPauseButtonVisible && mediaPlayer?.Media != null && ((pausable ?? mediaPlayer?.CanPause) != false ||
                state != VLCState.Opening && state != VLCState.Playing && state != VLCState.Buffering));
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

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (e.OriginalSource == sender)
            {
                //ToggleFullscreen();
            }
        }
    }
}
