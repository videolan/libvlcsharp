using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            var mainColor = Resources[nameof(MainColor)];
            MainColor = mainColor == null ? Color.Transparent : (Color)mainColor;
            var buttonColor = Resources[nameof(ButtonColor)];
            ButtonColor = buttonColor == null ? Color.Transparent : (Color)buttonColor;
            ButtonStyle = Resources[nameof(ButtonStyle)] as Style;
            ControlsPanelStyle = Resources[nameof(ControlsPanelStyle)] as Style;
            SeekBarStyle = Resources[nameof(SeekBarStyle)] as Style;
            RemainingTimeLabelStyle = Resources[nameof(RemainingTimeLabelStyle)] as Style;
            BufferingProgressBarStyle = Resources[nameof(BufferingProgressBarStyle)] as Style;

            FadeOutTimer = new Timer(obj => FadeOut());
            SeekBarTimer = new Timer(obj => UpdatePosition());

            DeviceDisplay.MainDisplayInfoChanged += (sender, e) => UpdateZoom();
        }

        private VisualElement ControlsPanel { get; set; }
        private Button AudioTracksSelectionButton { get; set; }
        private Button CastButton { get; set; }
        private Button ClosedCaptionsSelectionButton { get; set; }
        private Button PlayPauseButton { get; set; }
        private Slider SeekBar { get; set; }
        private Label RemainingTimeLabel { get; set; }

        private bool FadeOutEnabled { get; set; } = true;
        private Timer FadeOutTimer { get; }
        private bool SeekBarTimerEnabled { get; set; }
        private Timer SeekBarTimer { get; set; }

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

        // TODO Add ForeColor property

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
        /// Identifies the <see cref="ButtonStyle"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ButtonStyleProperty = BindableProperty.Create(nameof(ButtonStyle), typeof(Style),
            typeof(PlaybackControls));
        /// <summary>
        /// Gets or sets the buttons style.
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
        /// Gets or sets the remaining time label style.
        /// </summary>
        public Style RemainingTimeLabelStyle
        {
            get => (Style)GetValue(RemainingTimeLabelStyleProperty);
            set => SetValue(RemainingTimeLabelStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LibVLCProperty = BindableProperty.Create(nameof(LibVLC), typeof(LibVLC), typeof(PlaybackControls));
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
        /// Identifies the <see cref="IsAudioTracksSelectionButtonVisible"/> dependency property.
        /// </summary>
        public static readonly BindableProperty IsAudioTracksSelectionButtonVisibleProperty = BindableProperty.Create(
            nameof(IsAudioTracksSelectionButtonVisible), typeof(bool), typeof(PlaybackControls),
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
            typeof(PlaybackControls), true);
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
            nameof(IsClosedCaptionsSelectionButtonVisible), typeof(bool), typeof(PlaybackControls),
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
        public static readonly BindableProperty IsPlayPauseEnabledProperty = BindableProperty.Create(nameof(IsPlayPauseEnabled), typeof(bool),
            typeof(PlaybackControls), true);
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
                AudioTracksSelectionButton = SetClickEventHandler(nameof(AudioTracksSelectionButton), AudioTracksSelectionButton_Clicked, true);
                CastButton = SetClickEventHandler(nameof(CastButton), CastButton_Clicked);
                ClosedCaptionsSelectionButton = SetClickEventHandler(nameof(ClosedCaptionsSelectionButton), ClosedCaptionsSelectionButton_Clicked,
                    true);
                PlayPauseButton = SetClickEventHandler(nameof(PlayPauseButton), PlayPauseButton_Clicked);
                SetClickEventHandler("StopButton", StopButton_Clicked, true);
                SetClickEventHandler("ZoomButton", ZoomButton_Clicked, true);
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

        private Button SetClickEventHandler(string name, EventHandler eventHandler, bool fadeIn = false)
        {
            var button = this.FindChild<Button>(name);
            if (button != null)
            {
                button.Clicked += (sender, e) => OnButtonClicked(sender, e, eventHandler, fadeIn);
            }
            return button;
        }

        private void OnButtonClicked(object sender, EventArgs e, EventHandler eventHandler, bool fadeIn = false)
        {
            if (fadeIn)
            {
                _ = FadeInAsync();
            }
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                try
                {
                    eventHandler?.Invoke(sender, e);
                }
                catch (Exception)
                {
                    //TODO Manage errors
                }
            }
        }

        private IEnumerable<MediaTrack> GetMediaTracks(MediaPlayer mediaPlayer, TrackType trackType)
        {
            return mediaPlayer?.Media?.Tracks.Where(t => t.TrackType == trackType);
        }

        private string GetTrackName(string trackName, int trackId, int currentTrackId)
        {
            return trackId == currentTrackId ? $"{trackName} *" : trackName;
        }

        private string GetTrackName(MediaTrack mediaTrack, int currentTrackId)
        {
            var trackName = mediaTrack.Description ?? mediaTrack.Language ?? $"Track {mediaTrack.Id}";
            return GetTrackName(trackName, mediaTrack.Id, currentTrackId);
        }

        private async Task SelectTrack(TrackType trackType, string popupTitle, Func<MediaPlayer, int> getCurrentTrackId,
            Action<MediaPlayer, int> setCurrentTrackId, bool addDeactivateRow = false)
        {
            var mediaPlayer = MediaPlayer;
            var mediaTracks = GetMediaTracks(mediaPlayer, trackType);
            if (mediaTracks == null)
            {
                return;
            }

            var currentTrackId = getCurrentTrackId(mediaPlayer);
            IEnumerable<string> mediaTracksNames = mediaTracks.Select(t => GetTrackName(t, currentTrackId)).OrderBy(n => n);
            if (addDeactivateRow)
            {
                mediaTracksNames = new[] { GetTrackName("Disable", -1, currentTrackId) }.Union(mediaTracksNames);
            }

            var mediaTrack = await this.GetParentPage()?.DisplayActionSheet(popupTitle, null, null, mediaTracksNames.ToArray());
            if (mediaTrack != null)
            {
                var found = false;
                foreach (var mt in mediaTracks)
                {
                    if (GetTrackName(mt, currentTrackId) == mediaTrack)
                    {
                        found = true;
                        setCurrentTrackId(mediaPlayer, mt.Id);
                        break;
                    }
                }
                if (!found)
                {
                    setCurrentTrackId(mediaPlayer, -1);
                }
            }
        }

        private async void AudioTracksSelectionButton_Clicked(object sender, EventArgs e)
        {
            await SelectTrack(TrackType.Audio, "Audio tracks", m => m.AudioTrack, (m, id) => m.SetAudioTrack(id));
        }

        private async void ClosedCaptionsSelectionButton_Clicked(object sender, EventArgs e)
        {
            await SelectTrack(TrackType.Text, "Closed captions tracks", m => m.Spu, (m, id) => m.SetSpu(id), true);
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

        private void StopButton_Clicked(object sender, EventArgs e)
        {
            MediaPlayer?.Stop();
        }

        private void ZoomButton_Clicked(object sender, EventArgs e)
        {
            Zoom = !Zoom;
        }

        private async Task RotateElement(VisualElement element, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                await element.RotateTo(360, 800);
                await element.RotateTo(0, 0);
            }
        }

        private async void CastButton_Clicked(object sender, EventArgs e)
        {
            var libVLC = LibVLC;
            if (libVLC != null)
            {
                var mediaPlayer = MediaPlayer;
                if (mediaPlayer != null)
                {
                    CastButton.Clicked -= CastButton_Clicked;
                    FadeOutEnabled = false;
                    try
                    {
                        _ = FadeInAsync();
                        var cancellationTokenSource = new CancellationTokenSource();
                        _ = RotateElement(CastButton, cancellationTokenSource.Token);
                        IEnumerable<RendererItem> renderers;
                        try
                        {
                            renderers = await Chromecast.FindRenderersAsync(libVLC);
                        }
                        finally
                        {
                            cancellationTokenSource.Cancel();
                        }
                        var rendererName = await this.GetParentPage()?.DisplayActionSheet("Cast to", null, null,    // TODO String in resx and add a ResourceManager dependency property ?
                            renderers.OrderBy(r => r.Name).Select(r => r.Name).ToArray());
                        if (rendererName != null)
                        {
                            mediaPlayer.SetRenderer(renderers.First(r => r.Name == rendererName));
                        }
                    }
                    finally
                    {
                        CastButton.Clicked += CastButton_Clicked;
                        FadeOutEnabled = true;
                    }
                }
            }
            _ = FadeInAsync();
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
            SeekBarTimerEnabled = true;
            SeekBarTimer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
        }

        private void UpdatePosition()
        {
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer != null)
            {
                mediaPlayer.Position = (float)(SeekBar.Value / SeekBar.Maximum);
            }

            SeekBarTimerEnabled = false;
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
                oldMediaPlayer.ESAdded -= MediaPlayer_TracksChanged;
                oldMediaPlayer.ESDeleted -= MediaPlayer_TracksChanged;
                oldMediaPlayer.LengthChanged -= MediaPlayer_LengthChanged;
                oldMediaPlayer.MediaChanged -= MediaPlayer_MediaChanged;
                //TODO oldMediaPlayer.PausableChanged -= MediaPlayer_PausableChanged;
                oldMediaPlayer.Paused -= MediaPlayer_Paused;
                oldMediaPlayer.Playing -= MediaPlayer_Playing;
                oldMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                //TODO oldMediaPlayer.SeekableChanged -= MediaPlayer_SeekableChanged;
                oldMediaPlayer.Stopped -= MediaPlayer_Stopped;
                oldMediaPlayer.Vout -= MediaPlayer_VoutChanged;
            }

            if (newMediaPlayer != null)
            {
                newMediaPlayer.Buffering += MediaPlayer_Buffering;
                newMediaPlayer.ESAdded += MediaPlayer_TracksChanged;
                newMediaPlayer.ESDeleted += MediaPlayer_TracksChanged;
                newMediaPlayer.EndReached += MediaPlayer_EndReached;
                newMediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
                newMediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
                //TODO newMediaPlayer.PausableChanged += MediaPlayer_PausableChanged;
                newMediaPlayer.Paused += MediaPlayer_Paused;
                newMediaPlayer.Playing += MediaPlayer_Playing;
                newMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                //TODO newMediaPlayer.SeekableChanged += MediaPlayer_SeekableChanged;
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
            if (SeekBarTimerEnabled)
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

        private void SetTracksSelectionButtonVisible(Button tracksSelectionButton, bool isVisible)
        {
            if (tracksSelectionButton != null)
            {
                tracksSelectionButton.IsVisible = isVisible;
            }
        }

        private void SetTracksSelectionButtonVisible(MediaPlayer mediaPlayer, Button tracksSelectionButton, bool isTracksSelectionButtonVisible,
            TrackType trackType, int count)
        {
            if (tracksSelectionButton != null)
            {
                SetTracksSelectionButtonVisible(tracksSelectionButton, isTracksSelectionButtonVisible &&
                    GetMediaTracks(mediaPlayer, trackType).Count() >= count);
            }
        }

        private void OnAudioTracksChanged()
        {
            SetTracksSelectionButtonVisible(MediaPlayer, AudioTracksSelectionButton, IsAudioTracksSelectionButtonVisible, TrackType.Audio, 2);
        }

        private void OnClosedCaptionsTracksChanged()
        {
            SetTracksSelectionButtonVisible(MediaPlayer, ClosedCaptionsSelectionButton, IsClosedCaptionsSelectionButtonVisible, TrackType.Text, 1);
        }

        private void MediaPlayer_TracksChanged(object sender, EventArgs e)
        {
            OnAudioTracksChanged();
            OnClosedCaptionsTracksChanged();
        }

        private void MediaPlayer_MediaChanged(object sender, MediaPlayerMediaChangedEventArgs e)
        {
            SetTracksSelectionButtonVisible(AudioTracksSelectionButton, false);
            SetTracksSelectionButtonVisible(ClosedCaptionsSelectionButton, false);
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
                case VLCState.Stopped:
                    UpdateSeekBar(TimeSpan.Zero);
                    goto case VLCState.Paused;
                case VLCState.Paused:
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

        private static void IsAudioTracksSelectionButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).OnAudioTracksChanged();
        }

        private static void IsClosedCaptionsSelectionButtonVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PlaybackControls)bindable).OnAudioTracksChanged();
        }

        private void FadeOut()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await ControlsPanel.FadeTo(0, 1000) && ControlsPanel.Opacity == 0)
                {
                    ControlsPanel.IsVisible = false;
                }
            });
        }

        /// <summary>
        /// Controls fade in.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FadeInAsync()
        {
            FadeOutTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            ControlsPanel.IsVisible = true;
            if (ControlsPanel.Opacity != 1)
            {
                await ControlsPanel.FadeTo(1);
            }
            if (FadeOutEnabled && ShowAndHideAutomatically && MediaPlayer?.State == VLCState.Playing)
            {
                FadeOutTimer.Change(TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
            }
        }
    }
}
