using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FontAwesome;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.MediaPlayerElement;
using LibVLCSharp.Shared.Structures;
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
    public abstract partial class PlaybackControlsBase : Control
    {
        private const string PlayState = "PlayState";
        private const string PauseState = "PauseState";
        private const string DisabledState = "Disabled";
        private const string NormalState = "Normal";
        private const string ErrorState = "Error";
        private const string BufferingState = "Buffering";
        private const string ControlPanelFadeInState = "ControlPanelFadeIn";
        private const string ControlPanelFadeOutState = "ControlPanelFadeOut";
        private const string PlayPauseAvailableState = "PlayPauseAvailable";
        private const string PlayPauseUnavailableState = "PlayPauseUnavailable";
        private const string StopAvailableState = "StopAvailable";
        private const string StopUnavailableState = "StopUnavailable";
        private const string AudioSelectionAvailableState = "AudioSelectionAvailable";
        private const string AudioSelectionUnavailableState = "AudioSelectionUnavailable";
        private const string CCSelectionAvailableState = "CCSelectionAvailable";
        private const string CCSelectionUnavailableState = "CCSelectionUnavailable";
        private const string SeekBarAvailableState = "SeekBarAvailable";
        private const string SeekBarUnavailableState = "SeekBarUnavailable";
        private const string ZoomAvailableState = "ZoomAvailable";
        private const string ZoomUnavailableState = "ZoomUnavailable";
        private const string VolumeAvailableState = "VolumeAvailable";
        private const string VolumeUnavailableState = "VolumeUnavailable";
        private const string CompactModeState = "CompactMode";
        private const string NormalModeState = "NormalMode";

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackControlsBase"/> class
        /// </summary>
        public PlaybackControlsBase()
        {
            DefaultStyleKey = typeof(PlaybackControls);
            Manager = new MediaPlayerElementManager(new DispatcherAdapter(Dispatcher), new DisplayInformation(), new DisplayRequestAdapter());
            Manager.Get<AspectRatioManager>().AspectRatioChanged += AspectRatioChanged;
            var autoHideManager = Manager.Get<AutoHideManager>();
            autoHideManager.Shown += (sender, e) => VisualStateManager.GoToState(this, ControlPanelFadeInState, true);
            autoHideManager.Hidden += (sender, e) => VisualStateManager.GoToState(this, ControlPanelFadeOutState, true);
            autoHideManager.Enabled = ShowAndHideAutomatically;
            var audioTrackManager = Manager.Get<AudioTracksManager>();
            audioTrackManager.TracksCleared += OnTracksCleared;
            audioTrackManager.TrackSelected += OnTrackSelected;
            audioTrackManager.TrackAdded += OnTrackAdded;
            audioTrackManager.TrackDeleted += OnTrackDeleted;
            var subTitlesTrackManager = Manager.Get<SubtitlesTracksManager>();
            subTitlesTrackManager.TracksCleared += OnTracksCleared;
            subTitlesTrackManager.TrackSelected += OnTrackSelected;
            subTitlesTrackManager.TrackAdded += OnTrackAdded;
            subTitlesTrackManager.TrackDeleted += OnTrackDeleted;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PlaybackControlsBase()
        {
            Manager.Dispose();
        }

        private MediaPlayerElementManager Manager { get; }

        private bool HasError { get; set; }
        private MenuFlyout? ZoomMenu { get; set; }
        private IDictionary<MenuFlyout, TracksMenu> TracksMenus { get; } = new Dictionary<MenuFlyout, TracksMenu>();

        /// <summary>
        /// Gets the <see cref="ResourceLoader"/>
        /// </summary>
        protected ResourceLoader ResourceLoader => ResourceLoader.GetForCurrentView("LibVLCSharp.Uno/Resources");

        private FrameworkElement? LeftSeparator { get; set; }
        private FrameworkElement? RightSeparator { get; set; }
        private TextBlock? ErrorTextBlock { get; set; }
        private Slider? VolumeSlider { get; set; }
        private Slider? ProgressSlider { get; set; }
        private TextBlock? TimeElapsedElement { get; set; }
        private TextBlock? TimeRemainingElement { get; set; }
        private FrameworkElement? TimeTextGrid { get; set; }

        private Control? VolumeMuteButton { get; set; }
        private FrameworkElement? PlayPauseButton { get; set; }
        private FrameworkElement? PlayPauseButtonOnLeft { get; set; }
        private Control? StopButton { get; set; }
        private FrameworkElement? CastButton { get; set; }
        private Button? ZoomButton { get; set; }
        private FrameworkElement? FullWindowButton { get; set; }

        // I don't understand why there is an exception in UWP when I define the property like this : public LibVLCSharp.Uno.VideoView? VideoView
        // I introduced an useless LibVLCSharp.Uno.IVideoView interface as a workaround and it works, it's very strange...
        /// <summary>
        /// Identifies the <see cref="VideoView"/> dependency property
        /// </summary>
        public static readonly DependencyProperty VideoViewProperty = DependencyProperty.Register(nameof(VideoView), typeof(IVideoView),
            typeof(PlaybackControlsBase), new PropertyMetadata(null,
                (d, args) => ((PlaybackControlsBase)d).Manager.VideoView = (IVideoControl)args.NewValue));
        /// <summary>
        /// Gets or sets the video view
        /// </summary>
        public IVideoView? VideoView
        {
            get => (IVideoView)GetValue(VideoViewProperty);
            set => SetValue(VideoViewProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property
        /// </summary>
        public static readonly DependencyProperty LibVLCProperty = DependencyProperty.Register(nameof(LibVLC), typeof(LibVLC),
            typeof(PlaybackControlsBase), new PropertyMetadata(null));
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
            typeof(PlaybackControlsBase), new PropertyMetadata(null, (d, args) =>
            ((PlaybackControlsBase)d).OnMediaPlayerChanged((Shared.MediaPlayer)args.OldValue, (Shared.MediaPlayer)args.NewValue)));
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
            typeof(bool), typeof(PlaybackControlsBase),
            new PropertyMetadata(true, (d, args) => ((PlaybackControlsBase)d).OnShowAndHideAutomaticallyPropertyChanged()));
        /// <summary>
        /// Gets or sets a value that indicates whether the controls are shown and hidden automatically
        /// </summary>
        public bool ShowAndHideAutomatically
        {
            get => (bool)GetValue(ShowAndHideAutomaticallyProperty);
            set => SetValue(ShowAndHideAutomaticallyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="KeepDeviceAwake"/> dependency property
        /// </summary>
        public static readonly DependencyProperty KeepDeviceAwakeProperty = DependencyProperty.Register(nameof(KeepDeviceAwake), typeof(bool),
            typeof(PlaybackControlsBase),
            new PropertyMetadata(true, async (d, args) => await ((PlaybackControlsBase)d).OnKeepDeviceAwakePropertyChangedAsync()));
        /// <summary>
        /// Gets or sets a value indicating whether the device should be kept awake
        /// </summary>
        public bool KeepDeviceAwake
        {
            get => (bool)GetValue(KeepDeviceAwakeProperty);
            set => SetValue(KeepDeviceAwakeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsCompact"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsCompactProperty = DependencyProperty.Register(nameof(IsCompact), typeof(bool),
            typeof(PlaybackControlsBase),
            new PropertyMetadata(false, (d, args) => ((PlaybackControlsBase)d).OnIsCompactPropertyChanged()));
        /// <summary>
        /// Gets or sets a value that indicates whether playback controls are shown on one row instead of two
        /// </summary>
        public bool IsCompact
        {
            get => (bool)GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CCSelectionButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CCSelectionButtonContentProperty = DependencyProperty.Register(nameof(CCSelectionButtonContent),
            typeof(object), typeof(PlaybackControlsBase),
            new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.ClosedCaptioning)));
        /// <summary>
        /// Gets or sets the closed captions selection button content
        /// </summary>
        public object? CCSelectionButtonContent
        {
            get => GetValue(CCSelectionButtonContentProperty);
            set => SetValue(CCSelectionButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AudioTracksSelectionButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty AudioTracksSelectionButtonContentProperty = DependencyProperty.Register(
            nameof(AudioTracksSelectionButtonContent), typeof(object), typeof(PlaybackControlsBase),
            new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.FileAudio)));
        /// <summary>
        /// Gets or sets the audio tracks selection button content
        /// </summary>
        public object? AudioTracksSelectionButtonContent
        {
            get => GetValue(AudioTracksSelectionButtonContentProperty);
            set => SetValue(AudioTracksSelectionButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsPlayPauseButtonVisible"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsPlayPauseButtonVisibleProperty = DependencyProperty.Register(nameof(IsPlayPauseButtonVisible),
            typeof(bool), typeof(PlaybackControlsBase),
            new PropertyMetadata(true, (d, args) => ((PlaybackControlsBase)d).UpdatePlayPauseAvailability()));
        /// <summary>
        /// Gets or sets a value indicating whether the play/pause button is shown
        /// </summary>
        public bool IsPlayPauseButtonVisible
        {
            get => (bool)GetValue(IsPlayPauseButtonVisibleProperty);
            set => SetValue(IsPlayPauseButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PlayButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PlayButtonContentProperty = DependencyProperty.Register(nameof(PlayButtonContent), typeof(object),
            typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.Play)));
        /// <summary>
        /// Gets or sets the play button content
        /// </summary>
        public object? PlayButtonContent
        {
            get => GetValue(PlayButtonContentProperty);
            set => SetValue(PlayButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PlayButtonOnLeftContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PlayButtonOnLeftContentProperty = DependencyProperty.Register(nameof(PlayButtonOnLeftContent),
            typeof(object), typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.Play)));
        /// <summary>
        /// Gets or sets the play button on left content
        /// </summary>
        public object? PlayButtonOnLeftContent
        {
            get => GetValue(PlayButtonOnLeftContentProperty);
            set => SetValue(PlayButtonOnLeftContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PauseButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PauseButtonContentProperty = DependencyProperty.Register(nameof(PauseButtonContent), typeof(object),
            typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.Pause)));
        /// <summary>
        /// Gets or sets the pause button content
        /// </summary>
        public object? PauseButtonContent
        {
            get => GetValue(PauseButtonContentProperty);
            set => SetValue(PauseButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PauseButtonOnLeftContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PauseButtonOnLeftContentProperty = DependencyProperty.Register(nameof(PauseButtonOnLeftContent),
            typeof(object), typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.Pause)));
        /// <summary>
        /// Gets or sets the pause button on left content
        /// </summary>
        public object? PauseButtonOnLeftContent
        {
            get => GetValue(PauseButtonOnLeftContentProperty);
            set => SetValue(PauseButtonOnLeftContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsStopButtonVisible"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStopButtonVisibleProperty { get; } = DependencyProperty.Register(nameof(IsStopButtonVisible), typeof(bool),
            typeof(PlaybackControlsBase), new PropertyMetadata(false, (d, e) => ((PlaybackControlsBase)d).UpdateStopButton()));
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
            typeof(PlaybackControlsBase), new PropertyMetadata(true, (d, e) => ((PlaybackControlsBase)d).UpdateStopButton()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can stop the media playback.
        /// </summary>
        public bool IsStopEnabled
        {
            get => (bool)GetValue(IsStopEnabledProperty);
            set => SetValue(IsStopEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StopButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty StopButtonContentProperty = DependencyProperty.Register(nameof(StopButtonContent), typeof(object),
            typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.Stop)));
        /// <summary>
        /// Gets or sets the font icon for stop
        /// </summary>
        public object? StopButtonContent
        {
            get => (FontIcon)GetValue(StopButtonContentProperty);
            set => SetValue(StopButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsVolumeButtonVisible"/> dependency property.
        /// </summary>
        public static DependencyProperty IsVolumeButtonVisibleProperty { get; } = DependencyProperty.Register(nameof(IsVolumeButtonVisible),
            typeof(bool), typeof(PlaybackControlsBase),
            new PropertyMetadata(false, (d, e) => ((PlaybackControlsBase)d).UpdateVolumeButton()));
        /// <summary>
        /// Gets or sets a value indicating whether the stop button is shown.
        /// </summary>
        public bool IsVolumeButtonVisible
        {
            get => (bool)GetValue(IsVolumeButtonVisibleProperty);
            set => SetValue(IsVolumeButtonVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsVolumeEnabled"/> dependency property.
        /// </summary>
        public static DependencyProperty IsVolumeEnabledProperty { get; } = DependencyProperty.Register(nameof(IsVolumeEnabled), typeof(bool),
            typeof(PlaybackControlsBase), new PropertyMetadata(true, (d, e) => ((PlaybackControlsBase)d).UpdateVolumeButton()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can stop the media playback.
        /// </summary>
        public bool IsVolumeEnabled
        {
            get => (bool)GetValue(IsVolumeEnabledProperty);
            set => SetValue(IsVolumeEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VolumeButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty VolumeButtonContentProperty = DependencyProperty.Register(nameof(VolumeButtonContent),
            typeof(object), typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.VolumeUp)));
        /// <summary>
        /// Gets or sets the volume button content
        /// </summary>
        public object? VolumeButtonContent
        {
            get => GetValue(VolumeButtonContentProperty);
            set => SetValue(VolumeButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AudioButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty AudioButtonContentProperty = DependencyProperty.Register(nameof(AudioButtonContent), typeof(object),
            typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.VolumeUp)));
        /// <summary>
        /// Gets or sets the audio button content
        /// </summary>
        public object? AudioButtonContent
        {
            get => GetValue(AudioButtonContentProperty);
            set => SetValue(AudioButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VolumeMuteButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty VolumeMuteButtonContentProperty = DependencyProperty.Register(nameof(VolumeMuteButtonContent),
            typeof(object), typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.VolumeMute)));
        /// <summary>
        /// Gets or sets the volume mute button content
        /// </summary>
        public object? VolumeMuteButtonContent
        {
            get => GetValue(VolumeMuteButtonContentProperty);
            set => SetValue(VolumeMuteButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AudioMuteButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty AudioMuteButtonContentProperty = DependencyProperty.Register(nameof(AudioMuteButtonContent),
            typeof(object), typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.VolumeMute)));
        /// <summary>
        /// Gets or sets the audio mute button content
        /// </summary>
        public object? AudioMuteButtonContent
        {
            get => GetValue(AudioMuteButtonContentProperty);
            set => SetValue(AudioMuteButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsZoomButtonVisible"/> dependency property.
        /// </summary>
        public static DependencyProperty IsZoomButtonVisibleProperty { get; } = DependencyProperty.Register(nameof(IsZoomButtonVisible), typeof(bool),
            typeof(PlaybackControlsBase), new PropertyMetadata(true, (d, e) => ((PlaybackControlsBase)d).UpdateZoomButton()));
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
            typeof(PlaybackControlsBase), new PropertyMetadata(true, (d, e) => ((PlaybackControlsBase)d).UpdateZoomButton()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can zoom the media.
        /// </summary>
        public bool IsZoomEnabled
        {
            get => (bool)GetValue(IsZoomEnabledProperty);
            set => SetValue(IsZoomEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ZoomButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ZoomButtonContentProperty = DependencyProperty.Register(nameof(ZoomButtonContent), typeof(object),
            typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateSolidFontIcon(FontAwesomeIcons.Expand)));
        /// <summary>
        /// Gets or sets the aspect ratio button content
        /// </summary>
        public object? ZoomButtonContent
        {
            get => GetValue(ZoomButtonContentProperty);
            set => SetValue(ZoomButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CastButtonContent"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CastButtonContentProperty = DependencyProperty.Register(nameof(CastButtonContent), typeof(object),
            typeof(PlaybackControlsBase), new PropertyMetadata(FontAwesome.CreateBrandsFontIcon(FontAwesomeIcons.Chromecast)));
        /// <summary>
        /// Gets or sets the cast button content
        /// </summary>
        public object? CastButtonContent
        {
            get => GetValue(CastButtonContentProperty);
            set => SetValue(CastButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSeekBarVisible"/> dependency property
        /// </summary>
        public static DependencyProperty IsSeekBarVisibleProperty { get; } = DependencyProperty.Register(nameof(IsSeekBarVisible), typeof(bool), typeof(PlaybackControls),
            new PropertyMetadata(true, (d, e) => ((PlaybackControls)d).UpdateSeekBarVisibility()));
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
        public static DependencyProperty IsSeekBarEnabledProperty { get; } = DependencyProperty.Register(nameof(IsSeekBarEnabled), typeof(bool), typeof(PlaybackControls),
            new PropertyMetadata(true, (d, e) => ((PlaybackControls)d).UpdateSeekAvailability()));
        /// <summary>
        /// Gets or sets a value indicating whether a user can use the seek bar to find a location in the media
        /// </summary>
        public bool IsSeekBarEnabled
        {
            get => (bool)GetValue(IsSeekBarEnabledProperty);
            set => SetValue(IsSeekBarEnabledProperty, value);
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

            LeftSeparator = GetTemplateChild(nameof(LeftSeparator)) as FrameworkElement;
            RightSeparator = GetTemplateChild(nameof(RightSeparator)) as FrameworkElement;
            ErrorTextBlock = GetTemplateChild(nameof(ErrorTextBlock)) as TextBlock;

            ProgressSlider = GetTemplateChild(nameof(ProgressSlider)) as Slider;
            if (ProgressSlider != null)
            {
                ProgressSlider.Minimum = 0;
                ProgressSlider.Maximum = 100;
                ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;
            }
            TimeElapsedElement = GetTemplateChild(nameof(TimeElapsedElement)) as TextBlock;
            TimeRemainingElement = GetTemplateChild(nameof(TimeRemainingElement)) as TextBlock;
            TimeTextGrid = GetTemplateChild(nameof(TimeTextGrid)) as FrameworkElement;

            VolumeMuteButton = GetTemplateChild(nameof(VolumeMuteButton)) as Control;
            PlayPauseButton = GetTemplateChild(nameof(PlayPauseButton)) as FrameworkElement;
            PlayPauseButtonOnLeft = GetTemplateChild(nameof(PlayPauseButtonOnLeft)) as FrameworkElement;
            StopButton = GetTemplateChild(nameof(StopButton)) as Control;
            CastButton = GetTemplateChild(nameof(CastButton)) as FrameworkElement;
            ZoomButton = GetTemplateChild(nameof(ZoomButton)) as Button;
            FullWindowButton = GetTemplateChild(nameof(FullWindowButton)) as FrameworkElement;
            var audioMuteButton = GetTemplateChild("AudioMuteButton");
            VolumeSlider = GetTemplateChild(nameof(VolumeSlider)) as Slider;
            if (VolumeSlider != null)
            {
                VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            }
            if (GetTemplateChild("VolumeFlyout") is Flyout volumeFlyout)
            {
                volumeFlyout.Opened += VolumeFlyout_Opened;
                volumeFlyout.Closed += VolumeFlyout_Closed;
            }
            var audioTracksSelectionButton = GetTemplateChild("AudioTracksSelectionButton") as Button;
            var ccSelectionButton = GetTemplateChild("CCSelectionButton") as Button;
            AddTracksMenu(audioTracksSelectionButton, Manager.Get<AudioTracksManager>(), AudioSelectionAvailableState, AudioSelectionUnavailableState);
            AddTracksMenu(ccSelectionButton, Manager.Get<SubtitlesTracksManager>(), CCSelectionAvailableState, CCSelectionUnavailableState, true);
            AddAspectRatioMenu(ZoomButton);

            SetButtonClick(PlayPauseButton, PlayPauseButton_Click);
            SetButtonClick(PlayPauseButtonOnLeft, PlayPauseButton_Click);
            SetButtonClick(StopButton, StopButton_Click);
            SetButtonClick(audioMuteButton, AudioMuteButton_Click);

            SetToolTip(StopButton, "Stop");
            SetToolTip(VolumeMuteButton, "ShowVolumeMenu");
            SetToolTip(audioMuteButton, "Mute");
            SetToolTip(audioTracksSelectionButton, "ShowAudioSelectionMenu");
            SetToolTip(ccSelectionButton, "ShowClosedCaptionMenu");

            UpdateZoomButton();
            Manager.Initialize();
            Reset();
        }

        /// <summary>
        /// Sets the value of the <see cref="ToolTipService.ToolTipProperty"/> for an object
        /// </summary>
        /// <param name="element">the object to which the attached property is written</param>
        /// <param name="resource">resource string</param>
        /// <param name="args">an object array that contains zero or more objects to format</param>
        protected abstract void SetToolTip(DependencyObject? element, string resource, params string[] args);

        private void UpdateControl(Control? control, bool enabled)
        {
            if (control != null)
            {
                control.IsEnabled = enabled;
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

        #region Aspect ratio

        private void AddAspectRatioMenu(Button? zoomButton)
        {
            if (zoomButton != null)
            {
                SetToolTip(ZoomButton, "AspectRatio");

                var menuFlyout = new MenuFlyout();
                ZoomMenu = menuFlyout;
                zoomButton.Flyout = menuFlyout;
                var menuItems = menuFlyout.Items;
                var mediaPlayer = MediaPlayer;
                var currentAspectRatio = Manager.Get<AspectRatioManager>().AspectRatio;
                foreach (AspectRatio aspectRatio in Enum.GetValues(typeof(AspectRatio)))
                {
                    var menuItem = new ToggleMenuFlyoutItem()
                    {
                        Text = ResourceLoader.GetString($"{nameof(AspectRatio)}{aspectRatio}"),
                        Tag = aspectRatio,
                        IsChecked = aspectRatio == currentAspectRatio
                    };
                    menuItem.Click += AspectRatioMenuItem_Click;
                    menuItems.Add(menuItem);
                }
            }
        }

        private void AspectRatioMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CheckMenuItem(ZoomMenu!, sender);
            Manager.Get<AspectRatioManager>().AspectRatio = (AspectRatio)((FrameworkElement)sender).Tag;
        }

        private void AspectRatioChanged(object sender, EventArgs e)
        {
            if (ZoomMenu != null)
            {
                CheckMenuItem(ZoomMenu, ZoomMenu.Items.First(i => i.Tag.Equals(((AspectRatioManager)sender).AspectRatio)));
            }
        }

        #endregion

        #region Auto hide

        private void VolumeFlyout_Opened(object sender, object e)
        {
            Manager.Get<AutoHideManager>().Enabled = false;
        }

        private void VolumeFlyout_Closed(object sender, object e)
        {
            OnShowAndHideAutomaticallyPropertyChanged();
        }

        private void OnPointerMoved(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void OnShowAndHideAutomaticallyPropertyChanged()
        {
            Manager.Get<AutoHideManager>().Enabled = ShowAndHideAutomatically;
        }

        /// <summary>
        /// Shows the tranport controls if they're hidden
        /// </summary>
        public void Show()
        {
            Manager.Get<AutoHideManager>().Show();
        }

        /// <summary>
        /// Hides the playback controls if they're shown
        /// </summary>
        public void Hide()
        {
            Manager.Get<AutoHideManager>().Hide();
        }

        #endregion

        #region Device awakening

        private Task OnKeepDeviceAwakePropertyChangedAsync()
        {
            return Manager.Get<DeviceAwakeningManager>().KeepDeviceAwakeAsync(KeepDeviceAwake);
        }

        #endregion

        #region Audio / Subtitles tracks

        private void AddTracksMenu(Button? trackButton, TracksManager manager, string availableStateName, string unavailableStateName,
            bool addNoneItem = false)
        {
            if (trackButton != null)
            {
                var menuFlyout = new MenuFlyout();
                trackButton.Flyout = menuFlyout;
                var tracksMenu = new TracksMenu(manager, availableStateName, unavailableStateName, addNoneItem);
                TracksMenus.Add(menuFlyout, tracksMenu);
                if (addNoneItem)
                {
                    AddNoneItem(menuFlyout);
                }
            }
        }

        private void AddNoneItem(MenuFlyout menuflyout)
        {
            AddTrack(menuflyout, null, ResourceLoader.GetString("None"));
        }

        private void AddTrack(MenuFlyout menuflyout, int? trackId, string? trackName, bool subTitle = false)
        {
            if (menuflyout == null)
            {
                return;
            }

            var menuItems = menuflyout.Items;
            if (trackId == null && menuItems.Any(i => i.Tag == null) || trackId != null && menuItems.Any(i => trackId.Equals(i.Tag)))
            {
                return;
            }

            var menuItem = new ToggleMenuFlyoutItem()
            {
                Text = trackName,
                Tag = trackId
            };
            menuItem.Click += TrackMenuItem_Click;
            menuItems.Add(menuItem);

            if (menuItems.Count == 2)
            {
                var firstMenuItem = (menuItems.FirstOrDefault() as ToggleMenuFlyoutItem);
                if (subTitle || firstMenuItem?.Tag != null)
                {
                    firstMenuItem!.IsChecked = true;
                }
                else
                {
                    menuItem.IsChecked = true;
                }
                VisualStateManager.GoToState(this, TracksMenus[menuflyout].AvailableStateName, true);
            }
        }

        private void CheckMenuItem(MenuFlyout menuFlyout, object menuItem)
        {
            foreach (var item in menuFlyout.Items)
            {
                if (item is ToggleMenuFlyoutItem toggleMenuFlyoutItem)
                {
                    var isChecked = (item == menuItem);
                    if (toggleMenuFlyoutItem.IsChecked != isChecked)
                    {
                        toggleMenuFlyoutItem.IsChecked = isChecked;
                    }
                }
            }
        }

        private void TrackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menu = TracksMenus.FirstOrDefault(kvp => kvp.Key.Items.Contains(sender));
            if (!menu.Equals(default(KeyValuePair<MenuFlyout, TracksMenu>)))
            {
                CheckMenuItem(menu.Key, sender);
                menu.Value.Manager.CurrentTrackId = ((int?)((MenuFlyoutItemBase)sender).Tag) ?? -1;
            }
        }

        private KeyValuePair<MenuFlyout, TracksMenu> GetTracksMenu(object sender)
        {
            return TracksMenus.First(kvp => kvp.Value.Manager == sender);
        }

        private void OnTracksCleared(object sender, EventArgs e)
        {
            var manager = (TracksManager)sender;
            var tracksMenukeyValuePair = GetTracksMenu(manager);
            var menuFlyout = tracksMenukeyValuePair.Key;
            var tracksMenu = tracksMenukeyValuePair.Value;
            menuFlyout.Items.Clear();
            if (tracksMenu.HasNoneItem)
            {
                AddNoneItem(menuFlyout);
            }
            VisualStateManager.GoToState(this, tracksMenu.UnavailableStateName, true);

            var tracks = manager.Tracks;
            if (tracks != null)
            {
                foreach (var track in tracks)
                {
                    AddTrack(menuFlyout, manager, track);
                }
            }
        }

        private void AddTrack(MenuFlyout menuFlyout, TracksManager manager, TrackDescription? trackDescription)
        {
            if (!string.IsNullOrWhiteSpace(trackDescription?.Name))
            {
                AddTrack(menuFlyout, trackDescription?.Id, trackDescription?.Name, manager == Manager.Get<SubtitlesTracksManager>());
            }
        }

        private void OnTrackSelected(object sender, MediaPlayerESSelectedEventArgs e)
        {
            var menuFlyout = GetTracksMenu(sender).Key;
            var id = e.Id;
            CheckMenuItem(menuFlyout, menuFlyout.Items.FirstOrDefault(mi => id == -1 && mi.Tag == null || id.Equals(mi.Tag)));
        }

        private void OnTrackAdded(object sender, MediaPlayerESAddedEventArgs e)
        {
            var manager = (TracksManager)sender;
            AddTrack(GetTracksMenu(manager).Key, manager, manager.GetTrackDescription(e.Id));
        }

        private void OnTrackDeleted(object sender, MediaPlayerESDeletedEventArgs e)
        {
            var tracksMenu = GetTracksMenu(sender);
            var menuFlyout = tracksMenu.Key;
            if (menuFlyout == null)
            {
                return;
            }

            var menuItems = menuFlyout.Items;
            var menuItem = menuItems.FirstOrDefault(mi => e.Id.Equals(mi.Tag));
            if (menuItem != null)
            {
                var isChecked = menuItem is ToggleMenuFlyoutItem toggleMenuFlyoutItem && toggleMenuFlyoutItem.IsChecked;
                menuItems.Remove(menuItem);
                if (isChecked && menuItems.FirstOrDefault() is ToggleMenuFlyoutItem firstMenuItem)
                {
                    firstMenuItem.IsChecked = true;
                }
                if (menuItems.Count < 2)
                {
                    VisualStateManager.GoToState(this, tracksMenu.Value.UnavailableStateName, true);
                }
            }
        }

        #endregion

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

        private void AudioMuteButton_Click(object sender, RoutedEventArgs e)
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                mediaPlayer.ToggleMute();
                UpdateMuteState();
            }
        }

        private void OnMediaPlayerChanged(Shared.MediaPlayer oldValue, Shared.MediaPlayer newValue)
        {
            Manager.MediaPlayer = newValue;

            if (oldValue != null)
            {
                oldValue.Buffering -= MediaPlayer_BufferingAsync;
                oldValue.EncounteredError -= MediaPlayer_EncounteredErrorAsync;
                oldValue.EndReached -= MediaPlayer_UpdateStateAsync;
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
                newValue.EncounteredError += MediaPlayer_EncounteredErrorAsync;
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

        private async void MediaPlayer_EncounteredErrorAsync(object sender, EventArgs e)
        {
            await DispatcherRunAsync(() => UpdateState(VLCState.Error));
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

        private void Reset()
        {
            HasError = false;
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
            bool? playState;
            state ??= MediaPlayer?.State ?? VLCState.NothingSpecial;
            switch (state)
            {
                case VLCState.Error:
                    var error = LibVLC?.LastLibVLCError ?? string.Empty;
                    HasError = !string.IsNullOrWhiteSpace(error);
                    if (!HasError)
                    {
                        goto case VLCState.Stopped;
                    }
                    statusState = ErrorState;
                    playState = true;
                    if (ErrorTextBlock != null)
                    {
                        ErrorTextBlock.Text = error;
                    }
                    break;
                case VLCState.Stopped:
                case VLCState.Ended:
                case VLCState.NothingSpecial:
                    UpdatePlayPauseAvailability(true);
                    UpdateSeekBarPosition();
                    UpdateSeekAvailability(false);
                    goto case VLCState.Paused;
                case VLCState.Paused:
                    if (HasError)
                    {
                        return;
                    }
                    statusState = DisabledState;
                    playState = true;
                    break;
                case VLCState.Buffering:
                    statusState = BufferingState;
                    playState = null;
                    break;
                default:
                    HasError = false;
                    statusState = NormalState;
                    playState = false;
                    break;
            }

            VisualStateManager.GoToState(this, statusState, true);
            UpdatePlayPauseState(playState);
            UpdateStopButton();
        }

        private void UpdateSeekBarVisibility()
        {
            VisualStateManager.GoToState(this, IsSeekBarVisible ? SeekBarAvailableState : SeekBarUnavailableState, true);
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

        private void UpdateVolumeButton()
        {
            VisualStateManager.GoToState(this, IsVolumeButtonVisible ? VolumeAvailableState : VolumeUnavailableState, true);
            UpdateControl(VolumeMuteButton, IsVolumeEnabled);
        }

        private void UpdateStopButton()
        {
            var state = MediaPlayer?.State;
            VisualStateManager.GoToState(this, IsStopButtonVisible && MediaPlayer?.Media != null ? StopAvailableState : StopUnavailableState, true);
            UpdateControl(StopButton, IsStopEnabled && state != null && state != VLCState.Ended && state != VLCState.Stopped);
        }

        private void UpdateZoomButton()
        {
            VisualStateManager.GoToState(this, IsZoomButtonVisible ? ZoomAvailableState : ZoomUnavailableState, true);
            UpdateControl(ZoomButton, IsZoomEnabled);
        }

        private void UpdatePlayPauseState(bool? playState)
        {
            if (playState is bool isPaused)
            {
                var playPauseToolTip = isPaused ? "Play" : "Pause";
                SetToolTip(PlayPauseButton, playPauseToolTip);
                SetToolTip(PlayPauseButtonOnLeft, playPauseToolTip);
                VisualStateManager.GoToState(this, isPaused ? PlayState : PauseState, true);
            }
        }

        private void UpdatePlayPauseAvailability(bool? pausable = null)
        {
            var mediaPlayer = MediaPlayer;
            var state = mediaPlayer?.State;
            var stopped = state != VLCState.Opening && state != VLCState.Playing && state != VLCState.Buffering;
            var playPauseButtonVisible = IsPlayPauseButtonVisible && mediaPlayer != null && mediaPlayer.Media != null &&
                (stopped || (pausable ?? mediaPlayer?.CanPause) == true);
            var playPauseAvailableState = playPauseButtonVisible ? PlayPauseAvailableState : PlayPauseUnavailableState;
            if (IsCompact || playPauseButtonVisible)
            {
                VisualStateManager.GoToState(this, playPauseAvailableState, true);
                OnIsCompactPropertyChanged();
            }
            else
            {
                OnIsCompactPropertyChanged();
                VisualStateManager.GoToState(this, playPauseAvailableState, true);
            }
            UpdatePlayPauseState(stopped);
        }

        private void OnIsCompactPropertyChanged()
        {
            VisualStateManager.GoToState(this, IsCompact ? CompactModeState : NormalModeState, true);
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
