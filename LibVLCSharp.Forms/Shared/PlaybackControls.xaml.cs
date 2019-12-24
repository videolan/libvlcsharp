﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using LibVLCSharp.Forms.Shared.Resources;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.MediaPlayerElement;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Represents the playback controls for a <see cref="LibVLCSharp.Shared.MediaPlayer"/>.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaybackControls : TemplatedView
    {
        private const string AudioSelectionAvailableState = "AudioSelectionAvailable";
        private const string AudioSelectionUnavailableState = "AudioSelectionUnavailable";
        private const string ClosedCaptionsSelectionAvailableState = "ClosedCaptionsSelectionAvailable";
        private const string ClosedCaptionsSelectionUnavailableState = "ClosedCaptionsSelectionUnavailable";
        private const string PlayState = "PlayState";
        private const string PauseState = "PauseState";
        private const string PauseAvailableState = "PauseAvailable";
        private const string PauseUnavailableState = "PauseUnavailable";
        private const string SeekAvailableState = "SeekAvailable";
        private const string SeekUnavailableState = "SeekUnavailable";
        private const string CastAvailableState = "CastAvailable";
        private const string CastUnavailableState = "CastUnavailable";

        /// <summary>
        /// Initializes a new instance of <see cref="PlaybackControls"/> class.
        /// </summary>
        public PlaybackControls()
        {
            InitializeComponent();

            ButtonColor = (Color)(Resources[nameof(ButtonColor)] ?? Color.Transparent);
            Foreground = (Color)(Resources[nameof(Foreground)] ?? Color.White);
            MainColor = (Color)(Resources[nameof(MainColor)] ?? Color.Transparent);
            AudioTracksSelectionButtonStyle = Resources[nameof(AudioTracksSelectionButtonStyle)] as Style;
            BufferingProgressBarStyle = Resources[nameof(BufferingProgressBarStyle)] as Style;
            ButtonBarStyle = Resources[nameof(ButtonBarStyle)] as Style;
            CastButtonStyle = Resources[nameof(CastButtonStyle)] as Style;
            ClosedCaptionsSelectionButtonStyle = Resources[nameof(ClosedCaptionsSelectionButtonStyle)] as Style;
            ControlsPanelStyle = Resources[nameof(ControlsPanelStyle)] as Style;
            MessageStyle = Resources[nameof(MessageStyle)] as Style;
            PlayPauseButtonStyle = Resources[nameof(PlayPauseButtonStyle)] as Style;
            RemainingTimeLabelStyle = Resources[nameof(RemainingTimeLabelStyle)] as Style;
            ElapsedTimeLabelStyle = Resources[nameof(ElapsedTimeLabelStyle)] as Style;
            SeekBarStyle = Resources[nameof(SeekBarStyle)] as Style;
            StopButtonStyle = Resources[nameof(StopButtonStyle)] as Style;
            AspectRatioButtonStyle = Resources[nameof(AspectRatioButtonStyle)] as Style;
            RewindButtonStyle = Resources[nameof(RewindButtonStyle)] as Style;
            SeekButtonStyle = Resources[nameof(SeekButtonStyle)] as Style;

            Manager = new MediaPlayerElementManager(new Dispatcher(), new DisplayInformation(), new DisplayRequest());
            var autoHideManager = Manager.Get<AutoHideNotifier>();
            autoHideManager.Shown += async (sender, e) => await FadeInAsync();
            autoHideManager.Hidden += async (sender, e) => await FadeOutAsync();
            autoHideManager.Enabled = ShowAndHideAutomatically;
            var audioTrackManager = Manager.Get<AudioTracksManager>();
            audioTrackManager.TrackAdded += OnAudioTracksChanged;
            audioTrackManager.TrackDeleted += OnAudioTracksChanged;
            audioTrackManager.TracksCleared += OnAudioTracksChanged;
            var subTitlesTrackManager = Manager.Get<SubtitlesTracksManager>();
            subTitlesTrackManager.TrackAdded += OnSubtitlesTracksChanged;
            subTitlesTrackManager.TrackDeleted += OnSubtitlesTracksChanged;
            subTitlesTrackManager.TracksCleared += OnSubtitlesTracksChanged;
            var castRenderersDiscoverer = Manager.Get<CastRenderersDiscoverer>();
            castRenderersDiscoverer.CastAvailableChanged += (sender, e) => UpdateCastAvailability();
            castRenderersDiscoverer.Enabled = IsCastButtonVisible;
            var seekBarManager = Manager.Get<SeekBarManager>();
            seekBarManager.PositionChanged += (sender, e) => UpdateTime();
            seekBarManager.SeekableChanged += (sender, e) => UpdateSeekAvailability();
            var bufferingProgressNotifier = Manager.Get<BufferingProgressNotifier>();
            bufferingProgressNotifier.Buffering += (sender, e) => OnBuffering();
            var stateManager = Manager.Get<StateManager>();
            stateManager.ErrorOccured += (sender, e) => ShowError();
            stateManager.ErrorCleared += (sender, e) => ErrorMessage = null;
            stateManager.Playing += (sender, e) => OnPlaying();
            stateManager.Paused += (sender, e) => OnStoppedOrPaused();
            stateManager.Stopped += (sender, e) => OnStoppedOrPaused();
            stateManager.PlayPauseAvailableChanged += (sender, e) => UpdatePauseAvailability();
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PlaybackControls()
        {
            Manager.Dispose();
        }

        private MediaPlayerElementManager Manager { get; }
        private Button AudioTracksSelectionButton { get; set; }
        private Button CastButton { get; set; }
        private Button ClosedCaptionsSelectionButton { get; set; }
        private VisualElement ControlsPanel { get; set; }
        private Button PlayPauseButton { get; set; }
        private Label RemainingTimeLabel { get; set; }
        private Label ElapsedTimeLabel { get; set; }
        private Label AspectRatioLabel { get; set; }

        private Slider SeekBar { get; set; }

        private bool Initialized { get; set; }
        private ISystemUI SystemUI => DependencyService.Get<ISystemUI>();

        private const int SEEK_OFFSET = 2000;
        private bool RemoteRendering { get; set; } = false;
        private const string Disconnect = "Disconnect";
        private const string Cancel = "Cancel";

        /// <summary>
        /// Identifies the <see cref="ButtonColor"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ButtonColorProperty = BindableProperty.Create(nameof(ButtonColor), typeof(Color),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the button color.
        /// </summary>
        public Color ButtonColor
        {
            get => (Color)GetValue(ButtonColorProperty);
            set => SetValue(ButtonColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ForegroundProperty = BindableProperty.Create(nameof(Foreground), typeof(Color),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the button color.
        /// </summary>
        public Color Foreground
        {
            get => (Color)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MainColor"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MainColorProperty = BindableProperty.Create(nameof(MainColor), typeof(Color),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the main color.
        /// </summary>
        public Color MainColor
        {
            get => (Color)GetValue(MainColorProperty);
            set => SetValue(MainColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AudioTracksSelectionButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty AudioTracksSelectionButtonStyleProperty = BindableProperty.Create(
            nameof(AudioTracksSelectionButtonStyle), typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the audio tracks selection button style.
        /// </summary>
        public Style AudioTracksSelectionButtonStyle
        {
            get => (Style)GetValue(AudioTracksSelectionButtonStyleProperty);
            set => SetValue(AudioTracksSelectionButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BufferingProgressBarStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty BufferingProgressBarStyleProperty = BindableProperty.Create(nameof(BufferingProgressBarStyle),
            typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the controls panel style.
        /// </summary>
        public Style BufferingProgressBarStyle
        {
            get => (Style)GetValue(BufferingProgressBarStyleProperty);
            set => SetValue(BufferingProgressBarStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonBarStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ButtonBarStyleProperty = BindableProperty.Create(nameof(ButtonBarStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the button bar style.
        /// </summary>
        public Style ButtonBarStyle
        {
            get => (Style)GetValue(ButtonBarStyleProperty);
            set => SetValue(ButtonBarStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CastButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty CastButtonStyleProperty = BindableProperty.Create(nameof(CastButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the cast button style.
        /// </summary>
        public Style CastButtonStyle
        {
            get => (Style)GetValue(CastButtonStyleProperty);
            set => SetValue(CastButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ClosedCaptionsSelectionButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ClosedCaptionsSelectionButtonStyleProperty = BindableProperty.Create(
            nameof(ClosedCaptionsSelectionButtonStyle), typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the closed captions selection button style.
        /// </summary>
        public Style ClosedCaptionsSelectionButtonStyle
        {
            get => (Style)GetValue(ClosedCaptionsSelectionButtonStyleProperty);
            set => SetValue(ClosedCaptionsSelectionButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ControlsPanelStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ControlsPanelStyleProperty = BindableProperty.Create(nameof(ControlsPanelStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the controls panel style.
        /// </summary>
        public Style ControlsPanelStyle
        {
            get => (Style)GetValue(ControlsPanelStyleProperty);
            set => SetValue(ControlsPanelStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MessageStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MessageStyleProperty = BindableProperty.Create(nameof(MessageStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the message style.
        /// </summary>
        public Style MessageStyle
        {
            get => (Style)GetValue(MessageStyleProperty);
            set => SetValue(MessageStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PlayPauseButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty PlayPauseButtonStyleProperty = BindableProperty.Create(nameof(PlayPauseButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the play/pause button style.
        /// </summary>
        public Style PlayPauseButtonStyle
        {
            get => (Style)GetValue(PlayPauseButtonStyleProperty);
            set => SetValue(PlayPauseButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RemainingTimeLabelStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty RemainingTimeLabelStyleProperty = BindableProperty.Create(nameof(RemainingTimeLabelStyle),
            typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the remaining time label style.
        /// </summary>
        public Style RemainingTimeLabelStyle
        {
            get => (Style)GetValue(RemainingTimeLabelStyleProperty);
            set => SetValue(RemainingTimeLabelStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ElapsedTimeLabelStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ElapsedTimeLabelStyleProperty = BindableProperty.Create(nameof(ElapsedTimeLabelStyle),
            typeof(Style), typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the elapsed time label style.
        /// </summary>
        public Style ElapsedTimeLabelStyle
        {
            get => (Style)GetValue(ElapsedTimeLabelStyleProperty);
            set => SetValue(ElapsedTimeLabelStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SeekBarStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty SeekBarStyleProperty = BindableProperty.Create(nameof(SeekBarStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the seek bar style.
        /// </summary>
        public Style SeekBarStyle
        {
            get => (Style)GetValue(SeekBarStyleProperty);
            set => SetValue(SeekBarStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StopButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty StopButtonStyleProperty = BindableProperty.Create(nameof(StopButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the stop button style.
        /// </summary>
        public Style StopButtonStyle
        {
            get => (Style)GetValue(StopButtonStyleProperty);
            set => SetValue(StopButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VideoView"/> dependency property.
        /// </summary>
        public static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
            typeof(PlaybackControls),
            propertyChanged: (bindable, oldValue, newValue) => ((PlaybackControls)bindable).Manager.VideoView = (VideoView)newValue);

        /// <summary>
        /// Gets or sets the associated <see cref="VideoView"/>.
        /// </summary>
        /// <remarks>It is only useful to set this property for the aspect ratio feature.</remarks>
        public VideoView VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            set => SetValue(VideoViewProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AspectRatioButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty AspectRatioButtonStyleProperty = BindableProperty.Create(nameof(AspectRatioButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the aspect ratio button style.
        /// </summary>
        public Style AspectRatioButtonStyle
        {
            get => (Style)GetValue(AspectRatioButtonStyleProperty);
            set => SetValue(AspectRatioButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RewindButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty RewindButtonStyleProperty = BindableProperty.Create(nameof(RewindButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the rewind button style.
        /// </summary>
        public Style RewindButtonStyle
        {
            get => (Style)GetValue(RewindButtonStyleProperty);
            set => SetValue(RewindButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SeekButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty SeekButtonStyleProperty = BindableProperty.Create(nameof(SeekButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the rewind button style.
        /// </summary>
        public Style SeekButtonStyle
        {
            get => (Style)GetValue(SeekButtonStyleProperty);
            set => SetValue(SeekButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonBarStartArea"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ButtonBarStartAreaProperty = BindableProperty.Create(nameof(ButtonBarStartArea), typeof(View),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the view in the button bar start area.
        /// </summary>
        public View ButtonBarStartArea
        {
            get => (View)GetValue(ButtonBarStartAreaProperty);
            set => SetValue(ButtonBarStartAreaProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonBarEndArea"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ButtonBarEndAreaProperty = BindableProperty.Create(nameof(ButtonBarEndArea), typeof(View),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the view in the button bar end area.
        /// </summary>
        public View ButtonBarEndArea
        {
            get => (View)GetValue(ButtonBarEndAreaProperty);
            set => SetValue(ButtonBarEndAreaProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LibVLCProperty = BindableProperty.Create(nameof(LibVLC), typeof(LibVLC), typeof(PlaybackControls),
            propertyChanged: LibVLCPropertyChanged);
        /// <summary>
        /// Gets or sets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => (LibVLC)GetValue(LibVLCProperty);
            set => SetValue(LibVLCProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
            typeof(LibVLCSharp.Shared.MediaPlayer), typeof(PlaybackControls), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets or sets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
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
        /// Identifies the <see cref="ErrorMessage"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(nameof(ErrorMessage), typeof(string),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets the last error message.
        /// </summary>
        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            private set => SetValue(ErrorMessageProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="KeepScreenOn"/> dependency property.
        /// </summary>
        public static readonly BindableProperty KeepScreenOnProperty = BindableProperty.Create(nameof(KeepScreenOn), typeof(bool),
            typeof(PlaybackControls), true, propertyChanged: KeepScreenOnPropertyChangedAsync);
        /// <summary>
        /// Gets or sets a value indicating whether the screen must be kept on when playing.
        /// </summary>
        public bool KeepScreenOn
        {
            get => (bool)GetValue(KeepScreenOnProperty);
            set => SetValue(KeepScreenOnProperty, value);
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
        /// Identifies the <see cref="ResourceManager"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ResourceManagerProperty = BindableProperty.Create(nameof(ResourceManager), typeof(ResourceManager),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the resource manager to localize strings.
        /// </summary>
        public ResourceManager ResourceManager
        {
            get => (ResourceManager)GetValue(ResourceManagerProperty) ?? Strings.ResourceManager;
            set => SetValue(ResourceManagerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowAndHideAutomatically"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ShowAndHideAutomaticallyProperty = BindableProperty.Create(nameof(ShowAndHideAutomatically),
            typeof(bool), typeof(PlaybackControls), true,
            propertyChanged: (bindable, oldValue, newValue) => ((PlaybackControls)bindable).OnShowAndHideAutomaticallyPropertyChanged());
        /// <summary>
        /// Gets or sets a value that indicates whether the controls are shown and hidden automatically.
        /// </summary>
        public bool ShowAndHideAutomatically
        {
            get => (bool)GetValue(ShowAndHideAutomaticallyProperty);
            set => SetValue(ShowAndHideAutomaticallyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsAudioTracksSelectionButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsAudioTracksSelectionButtonVisibleProperty = BindableProperty.Create(
            nameof(IsAudioTracksSelectionButtonVisible), typeof(bool), typeof(PlaybackControls), true,
            propertyChanged: IsAudioTracksSelectionButtonVisiblePropertyChanged);
        /// <summary>
        /// Gets or sets a value indicating whether the audio tracks selection button is shown.
        /// </summary>
        public bool IsAudioTracksSelectionButtonVisible
        {
            get => (bool)GetValue(IsAudioTracksSelectionButtonVisibleProperty);
            set => SetValue(IsAudioTracksSelectionButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsCastButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsCastButtonVisibleProperty = BindableProperty.Create(nameof(IsCastButtonVisible), typeof(bool),
            typeof(PlaybackControls), true, propertyChanged: IsCastButtonVisiblePropertyChangedAsync);
        /// <summary>
        /// Gets or sets a value indicating whether the cast button is shown.
        /// </summary>
        public bool IsCastButtonVisible
        {
            get => (bool)GetValue(IsCastButtonVisibleProperty);
            set => SetValue(IsCastButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsClosedCaptionsSelectionButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsClosedCaptionsSelectionButtonVisibleProperty = BindableProperty.Create(
            nameof(IsClosedCaptionsSelectionButtonVisible), typeof(bool), typeof(PlaybackControls), true,
            propertyChanged: IsClosedCaptionsSelectionButtonVisiblePropertyChanged);
        /// <summary>
        /// Gets or sets a value indicating whether the closed captions selection button is shown.
        /// </summary>
        public bool IsClosedCaptionsSelectionButtonVisible
        {
            get => (bool)GetValue(IsClosedCaptionsSelectionButtonVisibleProperty);
            set => SetValue(IsClosedCaptionsSelectionButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsPlayPauseButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsPlayPauseButtonVisibleProperty = BindableProperty.Create(nameof(IsPlayPauseButtonVisible),
            typeof(bool), typeof(PlaybackControls), true, propertyChanged: IsPlayPauseButtonVisiblePropertyChanged);
        /// <summary>
        /// Gets or sets a value indicating whether the play/pause button is shown.
        /// </summary>
        public bool IsPlayPauseButtonVisible
        {
            get => (bool)GetValue(IsPlayPauseButtonVisibleProperty);
            set => SetValue(IsPlayPauseButtonVisibleProperty, value);
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
        /// Identifies the <see cref="IsStopButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsStopButtonVisibleProperty = BindableProperty.Create(nameof(IsStopButtonVisible), typeof(bool),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets a value that indicates whether the stop button is shown.
        /// </summary>
        public bool IsStopButtonVisible
        {
            get => (bool)GetValue(IsStopButtonVisibleProperty);
            set => SetValue(IsStopButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsAspectRatioButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsAspectRatioButtonVisibleProperty = BindableProperty.Create(nameof(IsAspectRatioButtonVisible),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value indicating whether the aspect ratio button is shown.
        /// </summary>
        public bool IsAspectRatioButtonVisible
        {
            get => (bool)GetValue(IsAspectRatioButtonVisibleProperty);
            set => SetValue(IsAspectRatioButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsRewindButtonVisible"/> dependency property.
        /// </summary>s
        public static readonly BindableProperty IsRewindButtonVisibleProperty = BindableProperty.Create(nameof(IsRewindButtonVisible),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value indicating whether the rewind button is shown.
        /// </summary>
        public bool IsRewindButtonVisible
        {
            get => (bool)GetValue(IsRewindButtonVisibleProperty);
            set => SetValue(IsRewindButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSeekButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsSeekButtonVisibleProperty = BindableProperty.Create(nameof(IsSeekButtonVisible),
            typeof(bool), typeof(PlaybackControls), true);
        /// <summary>
        /// Gets or sets a value indicating whether the seek button is shown.
        /// </summary>
        public bool IsSeekButtonVisible
        {
            get => (bool)GetValue(IsSeekButtonVisibleProperty);
            set => SetValue(IsSeekButtonVisibleProperty, value);
        }

        /// <summary>
        /// Called when the <see cref="Element.Parent"/> property has changed.
        /// </summary>
        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent != null && !Initialized)
            {
                Initialized = true;
                OnApplyTemplate();
            }
        }

        private void OnApplyTemplate()
        {
            AudioTracksSelectionButton = SetClickEventHandler(nameof(AudioTracksSelectionButton), AudioTracksSelectionButton_ClickedAsync);
            CastButton = SetClickEventHandler(nameof(CastButton), CastButton_ClickedAsync);
            ClosedCaptionsSelectionButton = SetClickEventHandler(nameof(ClosedCaptionsSelectionButton), ClosedCaptionsSelectionButton_ClickedAsync);
            PlayPauseButton = SetClickEventHandler(nameof(PlayPauseButton), PlayPauseButton_Clicked);
            SetClickEventHandler("StopButton", StopButton_Clicked);
            SetClickEventHandler("AspectRatioButton", AspectRatioButton_ClickedAsync);
            ControlsPanel = this.FindChild<VisualElement>(nameof(ControlsPanel));
            SeekBar = this.FindChild<Slider>(nameof(SeekBar));
            RemainingTimeLabel = this.FindChild<Label>(nameof(RemainingTimeLabel));
            ElapsedTimeLabel = this.FindChild<Label>(nameof(ElapsedTimeLabel));
            AspectRatioLabel = this.FindChild<Label>(nameof(AspectRatioLabel));
            SetClickEventHandler("RewindButton", RewindButton_Clicked);
            SetClickEventHandler("SeekButton", SeekButton_Clicked);

            if (SeekBar != null)
            {
                Manager.Get<SeekBarManager>().SeekBarMaximum = SeekBar.Maximum;
                SeekBar.ValueChanged += SeekBar_ValueChanged;
            }

            VisualStateManager.GoToState(PlayPauseButton, Manager.Get<StateManager>().IsPlaying ? PauseState : PlayState);
            UpdateSeekAvailability();
            UpdateAudioTracksSelectionAvailability();
            UpdateClosedCaptionsTracksSelectionAvailability();
            UpdatePauseAvailability();
            UpdateTime();
            OnBuffering();
        }

        private static void IsAudioTracksSelectionButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).UpdateAudioTracksSelectionAvailability();
        }

        private static void IsClosedCaptionsSelectionButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).UpdateClosedCaptionsTracksSelectionAvailability();
        }

        private static void IsPlayPauseButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).UpdatePauseAvailability();
        }

        private static void IsCastButtonVisiblePropertyChangedAsync(BindableObject bindable, object oldValue, object newValue)
        {
            var playbackControls = (PlaybackControls)bindable;
            playbackControls.Manager.Get<CastRenderersDiscoverer>().Enabled = (bool)newValue;
        }

        private static void KeepScreenOnPropertyChangedAsync(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).UpdateKeepScreenOn((bool)newValue);
        }

        private static void LibVLCPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).Manager.LibVLC = (LibVLC)newValue;
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).Manager.MediaPlayer = (LibVLCSharp.Shared.MediaPlayer)newValue;
        }

        private void OnBuffering()
        {
            BufferingProgress = Manager.Get<BufferingProgressNotifier>().BufferingProgress;
        }

        private void OnAudioTracksChanged(object sender, EventArgs e)
        {
            UpdateAudioTracksSelectionAvailability();
        }

        private void OnSubtitlesTracksChanged(object sender, EventArgs e)
        {
            UpdateClosedCaptionsTracksSelectionAvailability();
        }

        private async void AudioTracksSelectionButton_ClickedAsync(object sender, EventArgs e)
        {
            await SelectTrackAsync(Manager.Get<AudioTracksManager>(), ResourceManager.GetString(nameof(Strings.AudioTracks)));
        }

        private async void CastButton_ClickedAsync(object sender, EventArgs e)
        {
            Manager.Get<AutoHideNotifier>().Enabled = false;
            try
            {
                var renderersDiscoverer = Manager.Get<CastRenderersDiscoverer>();
                var renderers = renderersDiscoverer.Renderers;
                var mediaPlayer = MediaPlayer;
                if (!RemoteRendering && renderers.Count() == 1)
                {
                    mediaPlayer.SetRenderer(renderers.First());
                    RemoteRendering = true;
                }
                else
                {
                    var result = await this.FindAncestor<Page>()?.DisplayActionSheet(ResourceManager.GetString(nameof(Strings.CastTo)),
                        Cancel, Disconnect, renderers.Select(r => r.Name).OrderBy(r => r).ToArray());
                    if (result != null)
                    {
                        var rendererName = renderers.FirstOrDefault(r => r.Name == result);
                        if (rendererName != null)
                        {
                            mediaPlayer.SetRenderer(rendererName);
                            RemoteRendering = true;
                        }
                        else if (result == Disconnect)
                        {
                            mediaPlayer.SetRenderer(null);
                            RemoteRendering = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
            finally
            {
                OnShowAndHideAutomaticallyPropertyChanged();
            }
            Show();
        }

        private async void ClosedCaptionsSelectionButton_ClickedAsync(object sender, EventArgs e)
        {
            await SelectTrackAsync(Manager.Get<SubtitlesTracksManager>(), ResourceManager.GetString(nameof(Strings.ClosedCaptions)), true);
        }

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
        {
            Manager.Get<StateManager>().TogglePause();
        }

        private void StopButton_Clicked(object sender, EventArgs e)
        {
            Manager.Get<StateManager>().Stop();
        }

        private async void AspectRatioButton_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                var aspectRatioManager = Manager.Get<AspectRatioManager>();
                aspectRatioManager.AspectRatio = aspectRatioManager.AspectRatio == AspectRatio.Original ? AspectRatio.BestFit :
                    aspectRatioManager.AspectRatio + 1;

                AspectRatioLabel.Text = Strings.ResourceManager.GetString($"{nameof(AspectRatio)}{aspectRatioManager.AspectRatio}");
                await AspectRatioLabel.FadeTo(1);
                await AspectRatioLabel.FadeTo(0, 2000);
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
        }

        private void RewindButton_Clicked(object sender, EventArgs e)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }

            mediaPlayer.Time -= SEEK_OFFSET;
        }

        private void SeekButton_Clicked(object sender, EventArgs e)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                return;
            }

            mediaPlayer.Time += SEEK_OFFSET;
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
            Show();

            try
            {
                eventHandler?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
        }

        private void OnPlaying()
        {
            VisualStateManager.GoToState(PlayPauseButton, PauseState);
        }

        private void OnStoppedOrPaused()
        {
            VisualStateManager.GoToState(PlayPauseButton, PlayState);
        }

        private string GetTrackName(string trackName, int trackId, int currentTrackId)
        {
            return trackId == currentTrackId ? $"{trackName} *" : trackName;
        }

        private async Task SelectTrackAsync(TracksManager manager, string popupTitle, bool addDeactivateRow = false)
        {
            var tracks = manager.Tracks;
            if (tracks == null)
            {
                return;
            }

            try
            {
                var currentTrackId = manager.CurrentTrackId;
                var index = 0;
                IEnumerable<string> tracksNames = tracks.Select(t =>
                {
                    index += 1;
                    return GetTrackName(t.Name, currentTrackId, index);
                }).OrderBy(n => n);
                if (addDeactivateRow)
                {
                    tracksNames = new[] { GetTrackName(ResourceManager.GetString(nameof(Strings.Disable)), -1, currentTrackId) }
                        .Union(tracksNames);
                }

                var trackName = await this.FindAncestor<Page>()?.DisplayActionSheet(popupTitle, null, null, tracksNames.ToArray());
                if (trackName != null)
                {
                    var found = false;
                    index = 0;
                    foreach (var trackDescription in tracks)
                    {
                        index += 1;
                        if (GetTrackName(trackDescription.Name, currentTrackId, index) == trackName)
                        {
                            found = true;
                            manager.CurrentTrackId = trackDescription.Id;
                            break;
                        }
                    }
                    if (!found)
                    {
                        manager.CurrentTrackId = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
        }

        private void UpdateTracksSelectionButtonAvailability(Button tracksSelectionButton, string state)
        {
            if (tracksSelectionButton != null)
            {
                VisualStateManager.GoToState(tracksSelectionButton, state);
            }
        }

        private void UpdateTracksSelectionAvailability(TracksManager tracksManager, Button tracksSelectionButton,
            bool isTracksSelectionButtonVisible, string availableState, string unavailableState, int count)
        {
            if (tracksSelectionButton != null)
            {
                var c = tracksManager.Tracks?.Where(t => t.Id != -1).Count();
                UpdateTracksSelectionButtonAvailability(tracksSelectionButton, isTracksSelectionButtonVisible &&
                    tracksManager.Tracks?.Where(t => t.Id != -1).Count() >= count ? availableState : unavailableState);
            }
        }

        private void UpdateAudioTracksSelectionAvailability()
        {
            UpdateTracksSelectionAvailability(Manager.Get<AudioTracksManager>(), AudioTracksSelectionButton, IsAudioTracksSelectionButtonVisible,
                AudioSelectionAvailableState, AudioSelectionUnavailableState, 2);
        }

        private void UpdateClosedCaptionsTracksSelectionAvailability()
        {
            UpdateTracksSelectionAvailability(Manager.Get<SubtitlesTracksManager>(), ClosedCaptionsSelectionButton,
                IsClosedCaptionsSelectionButtonVisible, ClosedCaptionsSelectionAvailableState, ClosedCaptionsSelectionUnavailableState, 1);
        }

        private void ShowError()
        {
            ErrorMessage = string.Format(ResourceManager.GetString(nameof(Strings.ErrorWithMedia)), Manager.Get<StateManager>().MediaResourceLocator);
        }

        private void UpdateKeepScreenOn(bool keepScreenOn)
        {
            Manager.Get<DeviceAwakeningManager>().KeepDeviceAwake = keepScreenOn;
        }

        private void UpdatePauseAvailability()
        {
            var playPauseButton = PlayPauseButton;
            if (playPauseButton != null)
            {
                VisualStateManager.GoToState(playPauseButton, IsPlayPauseButtonVisible &&
                    Manager.Get<StateManager>().PlayPauseAvailable ? PauseAvailableState : PauseUnavailableState);
            }
        }

        private void UpdateSeekAvailability()
        {
            var seekBar = SeekBar;
            if (seekBar != null)
            {
                VisualStateManager.GoToState(seekBar, IsSeekEnabled &&
                    Manager.Get<SeekBarManager>().Seekable ? SeekAvailableState : SeekUnavailableState);
            }
        }

        private void UpdateCastAvailability()
        {
            var castButton = CastButton;
            if (castButton != null)
            {
                VisualStateManager.GoToState(castButton,
                    Manager.Get<CastRenderersDiscoverer>().CastAvailable ? CastAvailableState : CastUnavailableState);
            }
        }

        private void SeekBar_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Show();
            Manager.Get<SeekBarManager>().SetSeekBarPosition(e.NewValue);
        }

        private void UpdateTime()
        {
            try
            {
                var position = Manager.Get<SeekBarManager>().Position;
                if (SeekBar != null)
                {
                    SeekBar.ValueChanged -= SeekBar_ValueChanged;
                    try
                    {
                        SeekBar.Value = position.SeekBarPosition;
                    }
                    finally
                    {
                        SeekBar.ValueChanged += SeekBar_ValueChanged;
                    }
                }
                if (RemainingTimeLabel != null)
                {
                    RemainingTimeLabel.Text = position.RemainingTimeText;
                }
                if (ElapsedTimeLabel != null)
                {
                    ElapsedTimeLabel.Text = position.ElapsedTimeText;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(ex);
            }
        }

        /// <summary>
        /// Show an error message box.
        /// </summary>
        /// <param name="ex">The exception to show.</param>
        protected virtual void ShowErrorMessageBox(Exception ex)
        {
            var error = ResourceManager.GetString(nameof(Strings.Error));
            Device.BeginInvokeOnMainThread(() => this.FindAncestor<Page>().DisplayAlert(error, ex?.GetBaseException().Message ?? error,
                ResourceManager.GetString(nameof(Strings.OK))));
        }

        private void OnShowAndHideAutomaticallyPropertyChanged()
        {
            Manager.Get<AutoHideNotifier>().Enabled = ShowAndHideAutomatically;
        }

        /// <summary>
        /// Shows the tranport controls if they're hidden
        /// </summary>
        public void Show()
        {
            Manager.Get<AutoHideNotifier>().Show();
        }

        /// <summary>
        /// Hides the playback controls if they're shown
        /// </summary>
        public void Hide()
        {
            Manager.Get<AutoHideNotifier>().Hide();
        }

        private async Task FadeInAsync()
        {
            var controlsPanel = ControlsPanel;
            if (controlsPanel != null)
            {
                controlsPanel.IsVisible = true;
                if (controlsPanel.Opacity != 1)
                {
                    var systemUI = SystemUI;
                    if (systemUI != null)
                    {
                        systemUI.ShowSystemUI();
                    }
                    await controlsPanel.FadeTo(1);
                }
            }
        }

        private async Task FadeOutAsync()
        {
            var controlsPanel = ControlsPanel;
            if (controlsPanel != null)
            {
                if (await controlsPanel.FadeTo(0, 1000) && controlsPanel.Opacity == 0)
                {
                    controlsPanel.IsVisible = false;
                }
            }

            var systemUI = SystemUI;
            if (systemUI != null)
            {
                systemUI.HideSystemUI();
            }
        }
    }
}
