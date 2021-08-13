using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibVLCSharp.Forms.Shared
{
    /// <summary>
    /// Represents an object that uses a <see cref="LibVLCSharp.Shared.MediaPlayer"/> to render audio and video to the display.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerElement : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerElement"/> class.
        /// </summary>
        public MediaPlayerElement()
        {
            InitializeComponent();
            LoadEqualizerControls();
        }

        private bool Initialized { get; set; }

        /// <summary>
        /// Identifies the <see cref="LibVLC"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LibVLCProperty = BindableProperty.Create(nameof(LibVLC), typeof(LibVLC),
            typeof(MediaPlayerElement), propertyChanged: LibVLCPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
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
            typeof(LibVLCSharp.Shared.MediaPlayer), typeof(MediaPlayerElement), propertyChanged: MediaPlayerPropertyChanged);
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private static readonly BindableProperty PlaybackControlsProperty = BindableProperty.Create(nameof(PlaybackControls),
            typeof(PlaybackControls), typeof(MediaPlayerElement), propertyChanged: PlaybackControlsPropertyChanged);
        /// <summary>
        /// Gets or sets the playback controls for the media.
        /// </summary>
        public PlaybackControls PlaybackControls
        {
            get => (PlaybackControls)GetValue(PlaybackControlsProperty);
            set => SetValue(PlaybackControlsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VideoView"/> dependency property
        /// </summary>
        private static readonly BindableProperty VideoViewProperty = BindableProperty.Create(nameof(VideoView), typeof(VideoView),
         typeof(MediaPlayerElement), propertyChanged: VideoViewPropertyChanged);
        /// <summary>
        /// Gets or sets the video view.
        /// </summary>
        public VideoView? VideoView
        {
            get => (VideoView)GetValue(VideoViewProperty);
            private set => SetValue(VideoViewProperty, value);
        }

        private static readonly BindableProperty EnableRendererDiscoveryProperty = BindableProperty.Create(nameof(EnableRendererDiscovery),
            typeof(bool), typeof(PlaybackControls), true, propertyChanged: EnableRendererDiscoveryPropertyChanged);

        /// <summary>
        /// Enable or disable renderer discovery
        /// </summary>
        public bool EnableRendererDiscovery
        {
            get => (bool)GetValue(EnableRendererDiscoveryProperty);
            set => SetValue(EnableRendererDiscoveryProperty, value);
        }


        private void OnVideoViewChanged(VideoView videoView)
        {
            if (videoView != null)
            {
                videoView.MediaPlayer = MediaPlayer;
                var playbackControls = PlaybackControls;
                if (playbackControls != null)
                {
                    playbackControls.VideoView = videoView;
                }
            }
        }

        private void OnLibVLCChanged(LibVLC libVLC)
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.LibVLC = libVLC;
            }
        }

        private void OnMediaPlayerChanged(LibVLCSharp.Shared.MediaPlayer mediaPlayer)
        {
            var videoView = VideoView;
            if (videoView != null)
            {
                videoView.MediaPlayer = mediaPlayer;
            }
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.MediaPlayer = mediaPlayer;

                // Defines Equalizer button click event
                var equalizerButton = PlaybackControls.GetEqualizerButton();
                if (equalizerButton != null)
                {
                    equalizerButton.Clicked += OpenEqualizerView;
                }

                // Check if the Equalizer must be set to the media
                if(MediaPlayer.Length > 0 && EqualizerUtils.IsEqualizerEnable())
                {
                    EqualizerUtils.SetEqualizerToMediaPlyer(MediaPlayer, new Equalizer((uint)EqualizerUtils.GetSavedPresetIndex()));
                }
            }
        }

        private void OnPlayControlsChanged(PlaybackControls playbackControls)
        {
            if (playbackControls != null)
            {
                playbackControls.IsCastButtonVisible = EnableRendererDiscovery;
                playbackControls.LibVLC = LibVLC;
                playbackControls.MediaPlayer = MediaPlayer;
                playbackControls.VideoView = VideoView;                
            }
        }

        private void OnEnableRendererDiscoveryChanged(bool enableRendererDiscovery)
        {
            var playbackControls = PlaybackControls;
            if (playbackControls != null)
            {
                playbackControls.IsCastButtonVisible = enableRendererDiscovery;
            }
        }

        private static void VideoViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnVideoViewChanged((VideoView)newValue);
        }

        private static void LibVLCPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnLibVLCChanged((LibVLC)newValue);
        }

        private static void MediaPlayerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnMediaPlayerChanged((LibVLCSharp.Shared.MediaPlayer)newValue);
        }

        private static void PlaybackControlsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnPlayControlsChanged((PlaybackControls)newValue);
        }

        private static void EnableRendererDiscoveryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((MediaPlayerElement)bindable).OnEnableRendererDiscoveryChanged((bool)newValue);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Element.Parent"/> of an element is set. 
        /// Implement this method in order to add behavior when the element is added to a parent.
        /// </summary>
        /// <remarks>Implementors must call the base method.</remarks>
        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null && !Initialized)
            {
                Initialized = true;

                if (VideoView == null)
                {
                    VideoView = new VideoView();
                }

                if (PlaybackControls == null)
                {
                    PlaybackControls = new PlaybackControls();
                }

                var application = Application.Current;
                application.PageAppearing += PageAppearing;
                application.PageDisappearing += PageDisappearing;
            }
        }

        private void PageAppearing(object sender, Page e)
        {
            if (e == this.FindAncestor<Page?>())
            {
                MessagingCenter.Subscribe<LifecycleMessage>(this, "OnSleep", m =>
                {
                    var applicationProperties = Application.Current.Properties;
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        applicationProperties[$"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_Position"] = mediaPlayer.Position;
                        applicationProperties[$"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_IsPlaying"] = mediaPlayer.State == VLCState.Playing;
                        mediaPlayer.Stop();
                    }
                    VideoView = null;
                });
                MessagingCenter.Subscribe<LifecycleMessage>(this, "OnResume", m =>
                {
                    VideoView = new VideoView();
                    var mediaPlayer = MediaPlayer;
                    if (mediaPlayer != null)
                    {
                        var applicationProperties = Application.Current.Properties;
                        if (applicationProperties.TryGetValue($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_IsPlaying", out var play) && play is true)
                        {
                            mediaPlayer.Play();
                            mediaPlayer.Position = applicationProperties.TryGetValue($"VLC_{mediaPlayer.NativeReference}_MediaPlayerElement_Position", out var position)
                                && position is float p ? p : 0;
                        }
                    }
                });
            }
        }

        private void PageDisappearing(object sender, Page e)
        {
            if (e == this.FindAncestor<Page?>())
            {
                MessagingCenter.Unsubscribe<LifecycleMessage>(this, "OnSleep");
                MessagingCenter.Unsubscribe<LifecycleMessage>(this, "OnResume");
            }
        }

        private void GestureRecognized(object sender, EventArgs e)
        {
            if(EqualizerControls != null && EqualizerControls.IsVisible)
            {
                EqualizerControls.IsVisible = false;
            }
            PlaybackControls.Show();
        }

        private Frame? EqualizerControls { get; set; }
        private Switch? EnableEqualizerSwitch { get; set; }
        private Picker? PresetsDataPicker { get; set; }
        private Slider? PreampSlider { get; set; }
        private BindableStackLayout? FrequenciesLayout { get; set; }
        private Equalizer? MediaEqualizer;
        private Preset? SelectedPreset { get; set; }
        private Switch? SnapBandsSwitch { get; set; }
        // Prevents EnableEqualizerIfDisable method to trigger when opening the equalizer overlay.
        private bool FirstLoading = true;

        private void LoadEqualizerControls()
        {
            EqualizerControls = this.FindChild<Frame?>(nameof(EqualizerControls));
            EnableEqualizerSwitch = this.FindChild<Switch?>(nameof(EnableEqualizerSwitch));
            PresetsDataPicker = this.FindChild<Picker?>(nameof(PresetsDataPicker));
            PreampSlider = this.FindChild<Slider?>(nameof(PreampSlider));
            FrequenciesLayout = this.FindChild<BindableStackLayout?>(nameof(FrequenciesLayout));
            SnapBandsSwitch = this.FindChild<Switch?>(nameof(SnapBandsSwitch));
            if (EnableEqualizerSwitch != null && PresetsDataPicker != null && PreampSlider != null)
            {
                EnableEqualizerSwitch.Toggled += EnableEqualizerSwitchToggled;
                PresetsDataPicker.SelectedIndexChanged += SelectedPresetIndexChanged;
                PreampSlider.ValueChanged += PreampSliderValuechanged;
            }
        }

        /// <summary>
        /// Occurs when a preset is selected.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void SelectedPresetIndexChanged(object sender, EventArgs e)
        {
            EnableEqualizerIfDisable();
            var picker = (Picker)sender;
            if (!FirstLoading && FrequenciesLayout != null && picker.SelectedIndex >= 0)
            {
                SelectedPreset = (Preset)picker.SelectedItem;
                if (SelectedPreset.Bands != null)
                {
                    FrequenciesLayout.ItemsSource = new ObservableCollection<Band>(SelectedPreset.Bands);
                }

                EqualizerUtils.SetEqualizerToMediaPlyer(MediaPlayer, new Equalizer((uint)SelectedPreset.PresetId));
                EqualizerUtils.SaveEqualizerState(true);
                EqualizerUtils.SavePreset(SelectedPreset.PresetId);
            }
        }

        /// <summary>
        /// Occurs when a new value of the amplification is set.
        /// </summary>
        /// <param name="sender">Arguments</param>
        /// <param name="e">Sender</param>
        private void PreampSliderValuechanged(object sender, ValueChangedEventArgs e)
        {
            EnableEqualizerIfDisable();
            if(MediaEqualizer != null && !FirstLoading)
            {
                MediaEqualizer.SetPreamp((float)e.NewValue);
                EqualizerUtils.SetEqualizerToMediaPlyer(MediaPlayer, MediaEqualizer);
            }
        }

        private void SetPresetsItemsSource(int presetIndex)
        {
            if(MediaEqualizer != null && PresetsDataPicker != null && PreampSlider != null)
            {
                var allPresets = EqualizerUtils.LoadAllPresets();
                PresetsDataPicker.BindingContext = this;
                PresetsDataPicker.ItemsSource = allPresets;
                PresetsDataPicker.ItemDisplayBinding = new Binding("Name");
                SelectedPreset = allPresets.First(p => p.PresetId == presetIndex);
                PresetsDataPicker.SelectedIndex = SelectedPreset.PresetId;

                PreampSlider.BindingContext = SelectedPreset;
                PreampSlider.SetBinding(Slider.ValueProperty, nameof(SelectedPreset.Preamp));

                if (FrequenciesLayout != null)
                {
                    FrequenciesLayout.ItemsSource = new ObservableCollection<Band>(SelectedPreset
                        .Bands.OrderBy(b => b.BandId));
                }
            }
        }

        // TODO: When a band amplification changes, apply the new value to the equalizer
        private void BandAmplificationValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Apply snap band action
        }

        private void EnableEqualizerSwitchToggled(object sender, ToggledEventArgs e)
        {
           if(e.Value == true)
           {      
                if (SelectedPreset != null && MediaEqualizer != null && !FirstLoading)
                {
                    MediaEqualizer = new Equalizer((uint)SelectedPreset.PresetId);
                    MediaEqualizer.SetPreamp((float)SelectedPreset.Preamp);
                    EqualizerUtils.SaveEqualizerState(true);
                    EqualizerUtils.SavePreset(SelectedPreset.PresetId);
                    EqualizerUtils.SetEqualizerToMediaPlyer(MediaPlayer, MediaEqualizer);
                }
            }
           else
           {
                MediaPlayer.UnsetEqualizer();
                EqualizerUtils.SaveEqualizerState(false);
           }
        }

        /// <summary>
        /// This method is called when the user try to change the preset or the amplifaction value 
        /// while the equalizer is disable.
        /// </summary>
        private void EnableEqualizerIfDisable()
        {
            if (EnableEqualizerSwitch != null && SelectedPreset != null && !EnableEqualizerSwitch.IsToggled
                && !FirstLoading)
            {
                EnableEqualizerSwitch.IsToggled = true;
                EqualizerUtils.SaveEqualizerState(true);
                EqualizerUtils.SavePreset(SelectedPreset.PresetId);
                MediaEqualizer = new Equalizer((uint)SelectedPreset.PresetId);
            }
        }


        private void OpenEqualizerView(object sender, EventArgs e)
        {
            /*var eq = new Equalizer(3);
            System.Diagnostics.Debug.WriteLine("******"+eq.PresetName(3)+"*******");
            for (var i=0; i<10; i++)
            {
                System.Diagnostics.Debug.WriteLine(eq.Amp((uint)i));
                System.Diagnostics.Debug.WriteLine("-------------------");

            }*/
            FirstLoading = true;
            if (MediaPlayer.Length > 0 && EqualizerControls != null)
            {
                var enableEqualizer = EqualizerUtils.IsEqualizerEnable();
                var presetIndex = enableEqualizer ? EqualizerUtils.GetSavedPresetIndex() : 0;
                MediaEqualizer = new Equalizer((uint)presetIndex);
                SetPresetsItemsSource(presetIndex);
                EqualizerControls.IsVisible = true;
                if (EnableEqualizerSwitch != null && enableEqualizer)
                    EnableEqualizerSwitch.IsToggled = true;
            }
            FirstLoading = false;
        }
    }
}
